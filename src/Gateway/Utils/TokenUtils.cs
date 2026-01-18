namespace Gateway.Utils;

public static class TokenUtils
{
    public static string CreateGatewayToken(ClaimsPrincipal user, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["CLIENT_SECRET"] ?? string.Empty)
        );
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            audience: configuration["AUDIENCES"],
            claims: user.Claims,
            expires: DateTime.Now.AddMinutes(5),
            issuer: configuration["CLIENT_ID"],
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
