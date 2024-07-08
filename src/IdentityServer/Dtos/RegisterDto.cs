namespace IdentityServer.Dtos;

public record RegisterDto
{
    public string NationalId { get; init; }
    public string PhoneNumber { get; init; }
    public string Email { get; init; }
    public string Password { get; set; }
}
