namespace BlogBE.DB;

public class User
{
    //Property named Id or [ClassName]Id is required for EF to work properly
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}