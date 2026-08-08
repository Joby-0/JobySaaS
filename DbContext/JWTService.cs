using Configuration;
using Microsoft.Extensions.Options;
using Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DbContext;

public class JWTService
{
    private readonly JwtOptions _jwtOptions;

    public JWTService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    private IEnumerable<Claim> CreateClaims(LoginResponse userSession, out Guid tokenId)
    {
        tokenId = Guid.NewGuid();

        return new Claim[]
        {
            new Claim("UserId", userSession.UserId.ToString()),
            new Claim("UserRole", userSession.UserRole),
            new Claim("UserName", userSession.UserName),
            new Claim(ClaimTypes.Role, userSession.UserRole),
            new Claim(ClaimTypes.NameIdentifier, tokenId.ToString()),
            new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTimeMinutes).ToString("O"))
        };
    }

    public JwtUserToken CreateJwtUserToken(LoginResponse userSession)
    {
        if (userSession == null)
            throw new ArgumentNullException(nameof(userSession));

        var tokenId = Guid.Empty;
        var key = Encoding.ASCII.GetBytes(_jwtOptions.IssuerSigningKey);
        var expireTime = DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTimeMinutes);

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtOptions.ValidIssuer,
            audience: _jwtOptions.ValidAudience,
            claims: CreateClaims(userSession, out tokenId),
            notBefore: DateTime.UtcNow,
            expires: expireTime,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

        return new JwtUserToken
        {
            TokenId = tokenId,
            EncryptedToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            ExpireTime = expireTime,
            UserRole = userSession.UserRole,
            UserName = userSession.UserName,
            UserId = userSession.UserId ?? Guid.Empty
        };
    }
}
