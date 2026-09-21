using System;

namespace PayRollApi.Application.DTOs;

// The refresh token is never included here — it travels only as an httpOnly cookie.
public record LoggedUser(string AccessToken, DateTime AccessTokenExpiresAtUtc, string Username);
