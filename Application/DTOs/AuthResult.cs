using System;

namespace PayRollApi.Application.DTOs;

// Internal to the auth flow — never serialize this directly to a response body.
public sealed record AuthResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RawRefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    int AdminUserId,
    string Username);
