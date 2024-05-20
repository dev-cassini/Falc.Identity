// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.Reflection;
using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Falc.Identity.Server.Pages;

[AllowAnonymous]
public class Index(IdentityServerLicense? license = null) : PageModel
{
    public IActionResult OnGet()
    {
        if (User.IsAuthenticated())
        {
            return Page();
        }

        return Redirect("/Account/Login");
    }

    public string Version
    {
        get => typeof(Duende.IdentityServer.Hosting.IdentityServerMiddleware).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion.Split('+').First()
            ?? "unavailable";
    }
    public IdentityServerLicense? License { get; } = license;
}
