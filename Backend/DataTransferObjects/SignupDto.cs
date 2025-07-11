namespace Backend.DataTransferObjects;

public class SignupDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }


}