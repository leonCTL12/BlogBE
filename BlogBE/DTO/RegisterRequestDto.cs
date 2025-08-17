namespace BlogBE.DTO;

public record RegisterRequestDto(
    string Email,
    string Password,
    string DisplayName
);