namespace Erp.Application.Auth
{
    public sealed record AuthResult(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken, DateTime RefreshTokenExpiresAtUtc, UserDto user);
}
