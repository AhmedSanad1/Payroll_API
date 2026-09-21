using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Application.DTOs;

public record LoginRequest
(
    string Username,
    string Password
);
