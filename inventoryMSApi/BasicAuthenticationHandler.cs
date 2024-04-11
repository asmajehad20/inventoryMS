using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using inventoryMSLogic.src.BusinessLogicLayer;
using Microsoft.Extensions.Primitives;

/// <summary>
/// Handler for basic authentication.
/// </summary>
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TimeProvider timeProvider)
        : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        try
        {
            await Task.Yield();

            var authorizationHeader = Request.Headers.Authorization;

            if (StringValues.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            var authHeader = AuthenticationHeaderValue.Parse(authorizationHeader.ToString());
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter??"");
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

            if (AuthenticationManager.CheckUserCredentials(username, password))
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, username),
            };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
            }
            else
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }


}
