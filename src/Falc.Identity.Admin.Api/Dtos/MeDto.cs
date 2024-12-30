namespace Falc.Identity.Admin.Api.Dtos;

public record MeDto(
    string Id,
    string UserName,
    string Email, 
    string? FirstName,
    string? LastName);