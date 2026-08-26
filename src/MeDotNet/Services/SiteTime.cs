namespace MeDotNet.Services;

// Timestamps are STORED in UTC (PublishedAt/CreatedAt). The site is authored from
// one place, so dates are DISPLAYED in Eastern (America/New_York), with DST handled
// by the zone data. This is a display concern only — never change what is stored.
public static class SiteTime
{
    private static readonly TimeZoneInfo Eastern =
        TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

    // e.g. "25 August 2026". EF Core returns DateTimes from SQL Server with
    // Kind=Unspecified; mark them UTC explicitly so the conversion is unambiguous.
    public static string FormatDate(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Eastern)
            .ToString("d MMMM yyyy");
}
