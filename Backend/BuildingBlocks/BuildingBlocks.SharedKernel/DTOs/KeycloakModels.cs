namespace BuildingBlocks.SharedKernel.DTOs;

public static class KeycloakModels
{
    public sealed class CreateUser
    {
        public required string Username { get; init; }

        public required string Email { get; init; }

        public required string FirstName { get; init; }

        public required string LastName { get; init; }

        public required string Password { get; init; }

        public bool Enabled { get; init; } = true;

        public bool EmailVerified { get; init; }

        public IDictionary<string, ICollection<string>>? Attributes { get; init; }
    }
}