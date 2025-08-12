namespace BlogBE.DTO;

public record RegisterUserDto(
    string Email,
    string Password,
    string DisplayName
);