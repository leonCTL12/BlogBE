namespace BlogBE.DTO;

public record RegisterRequest(
    string Email,
    string Password,
    string DisplayName
);