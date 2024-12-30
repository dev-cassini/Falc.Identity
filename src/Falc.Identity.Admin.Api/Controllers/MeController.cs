using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Extensions;
using Falc.Identity.Admin.Api.Dtos;
using Falc.Identity.Admin.Infrastructure.EntityFramework.Entities.Identity;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;

namespace Falc.Identity.Admin.Api.Controllers;

[ApiController]
[OpenApiIgnore]
[Authorize(policy: "Me")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class MeController(UserManager<UserIdentity> userManager) : ControllerBase
{
    [HttpGet("users/me")]
    [ProducesResponseType(type: typeof(MeDto), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(type: typeof(HttpValidationProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetMyUser()
    {
        var user = await userManager.Users.SingleOrDefaultAsync(x => x.Id == User.GetSubjectId());
        if (user is null)
        {
            return BadRequest(new HttpValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Identity", ["Identity suggests you are not a user."] }
            }));
        }
        
        var userClaims = await userManager.GetClaimsAsync(user);
        var userDto = new MeDto(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            userClaims.SingleOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value,
            userClaims.SingleOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value);
        
        return Ok(userDto);
    }
}