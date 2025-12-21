using System.Linq;
using System.Threading.Tasks;
using Falc.Identity.Admin.Api.Dtos;
using Falc.Identity.Admin.Infrastructure.EntityFramework.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Falc.Identity.Admin.Api.Controllers;

[ApiController]
[Route("api/sign-up")]
public class SignUpController(UserManager<UserIdentity> userManager) : ControllerBase
{
    [HttpPost("users")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(type: typeof(HttpValidationProblemDetails), statusCode: StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SignUp([FromBody] SignUpRequestDto request)
    {
        var user = new UserIdentity
        {
            UserName = request.EmailAddress,
            Email = request.EmailAddress
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return Created();
        }

        var errors = result.Errors
            .GroupBy(e => e.Code)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        return BadRequest(new HttpValidationProblemDetails(errors));
    }
}
