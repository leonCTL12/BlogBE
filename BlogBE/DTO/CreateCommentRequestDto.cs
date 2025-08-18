namespace BlogBE.DTO;

public class CreateCommentRequestDto
{
    public string Content { get; set; } = string.Empty;

    public int PostId { get; set; }
}