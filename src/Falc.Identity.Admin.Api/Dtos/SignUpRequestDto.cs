using System.ComponentModel.DataAnnotations;

namespace Falc.Identity.Admin.Api.Dtos;

public record SignUpRequestDto(
    [Required]
    [EmailAddress]
    string EmailAddress,

    [Required]
    string Password);
