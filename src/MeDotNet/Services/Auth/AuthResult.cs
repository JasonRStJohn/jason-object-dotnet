namespace MeDotNet.Services.Auth;

public record AuthResult(bool Success, string? ErrorMessage = null);
