namespace IdentityServer.Dtos;

public record LoginDto
{
    public string Email { get; set; }
    public string NationalId { get; init; }
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
}
