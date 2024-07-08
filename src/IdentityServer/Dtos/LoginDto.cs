namespace IdentityServer.Dtos;

public record LoginDto
{
    public string NationalId { get; init; }
    public string Password { get; set; }
}
