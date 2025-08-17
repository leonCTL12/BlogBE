namespace BlogBE.DTO;

public record LoginResponseDto(
    int UserId,
    string UserName,
    string Email,
    string Token,
    DateTime TokenExpiration
);