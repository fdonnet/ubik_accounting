﻿namespace Ubik.Security.Contracts.Users.Results
{
    public record UserStandardResult
    {
        public Guid Id { get; init; }
        public required string Firstname { get; init; }
        public required string Lastname { get; init; }
        public required string Email { get; init; }
        public bool IsActivated { get; init; } = true;
        public Guid Version { get; init; }
    }
}
