using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Dtos;

public record RegisterDto
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string NationalId { get; init; }
    
    [Required]
    public string PhoneNumber { get; init; }
    public string Email { get; init; }
    public string Password { get; set; }
    public string? ReturnUrl { get; set; }
    public string SubjectId { get; set; }
}
