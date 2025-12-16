using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cart.BLL.Middleware
{
    public class TokenLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", string.Empty);

                var jwtHandler = new JwtSecurityTokenHandler();
                if (jwtHandler.CanReadToken(token))
                {
                    var jwtToken = jwtHandler.ReadJwtToken(token);

                    var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value
                                   ?? jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                    var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                    var expiry = jwtToken.ValidTo;

                    Console.WriteLine($"[Token Log] User: {username}, Role: {role}, Expires: {expiry}");
                }
            }

            await _next(context);
        }
    }
}
