namespace BlogBE.DTO;

public record UpdatePostRequestDto(
    string Title,
    string Content
);