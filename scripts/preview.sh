#!/usr/bin/env bash
# Run this working tree as a preview on :5055, against its OWN database.
#
#   scripts/preview.sh               build + (re)start the preview
#   scripts/preview.sh --refresh-db  also re-copy the live data into MeDotNet_Preview
#
# Why its own database: Program.cs applies EF migrations on startup. A preview that
# pointed at the live "MeDotNet" database would migrate production the moment a
# branch with a new migration started. MeDotNet_Preview is a copy taken with
# BACKUP ... COPY_ONLY (which does not disturb the live backup chain).
#
# Never touches medotnet-app-1 (:5000, live). SA_PASSWORD is read from .env and
# passed to sqlcmd via SQLCMDPASSWORD, so it is never echoed or put on a command line.
set -euo pipefail
cd "$(dirname "$0")/.."

SQL=medotnet-sqlserver-1
PREVIEW_DB=MeDotNet_Preview
set -a; . ./.env; set +a

sql() {
    docker exec -e SQLCMDPASSWORD="$SA_PASSWORD" "$SQL" \
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -b -h -1 -W -Q "SET NOCOUNT ON; $1"
}

exists=$(sql "SELECT COUNT(*) FROM sys.databases WHERE name = N'$PREVIEW_DB'")
if [[ "${1:-}" == "--refresh-db" || "$exists" == "0" ]]; then
    echo "Copying live data into $PREVIEW_DB..."
    # RESTORE needs exclusive access: stop the preview and drop any other sessions first.
    docker rm -f medotnet-preview >/dev/null 2>&1 || true
    if [[ "$exists" != "0" ]]; then
        sql "ALTER DATABASE [$PREVIEW_DB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE" >/dev/null
    fi
    sql "BACKUP DATABASE [MeDotNet] TO DISK = N'/var/opt/mssql/data/preview-src.bak' WITH COPY_ONLY, INIT" >/dev/null
    sql "RESTORE DATABASE [$PREVIEW_DB] FROM DISK = N'/var/opt/mssql/data/preview-src.bak'
         WITH REPLACE,
         MOVE N'MeDotNet' TO N'/var/opt/mssql/data/$PREVIEW_DB.mdf',
         MOVE N'MeDotNet_log' TO N'/var/opt/mssql/data/${PREVIEW_DB}_log.ldf'" >/dev/null
    sql "ALTER DATABASE [$PREVIEW_DB] SET MULTI_USER" >/dev/null
fi

echo "Building medotnet-preview:dev..."
docker build -q -t medotnet-preview:dev . >/dev/null

mkdir -p dataprotection-keys-preview
docker rm -f medotnet-preview >/dev/null 2>&1 || true
docker run -d --name medotnet-preview --network medotnet_internal -p 5055:8080 \
    -e ASPNETCORE_ENVIRONMENT=Production \
    -e ASPNETCORE_URLS=http://+:8080 \
    --env-file <(printf 'ConnectionStrings__DefaultConnection=Server=sqlserver;Database=%s;User Id=sa;Password=%s;TrustServerCertificate=True\n' "$PREVIEW_DB" "$SA_PASSWORD") \
    -e "Site__CanonicalUrl=${SITE_CANONICAL_URL:-https://jasonobject.work}" \
    -v "$PWD/dataprotection-keys-preview:/keys" \
    medotnet-preview:dev >/dev/null

for _ in $(seq 1 30); do
    code=$(curl -s -o /dev/null -w '%{http_code}' http://127.0.0.1:5055/ || true)
    [[ "$code" == "200" ]] && { echo "Preview up: http://192.168.4.22:5055 (database $PREVIEW_DB)"; exit 0; }
    sleep 2
done
echo "Preview did not answer 200 within 60s; see: docker logs medotnet-preview" >&2
exit 1
