using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;

// using Configuration.Options;
using Models.DTO;
using Configuration.Options;

namespace DbContext;

public class JWTService
{
    private readonly JwtOptions _jwtOptions;

    public JWTService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    //Create a list of claims to encrypt into the JWT token
    private IEnumerable<Claim> CreateClaims(LoginResponse usrSession, out Guid TokenId)
    {
        TokenId = Guid.NewGuid();

        IEnumerable<Claim> claims = new Claim[] {
            //used to carry the loginUserSessionDto in the token
            new Claim("UserId", usrSession.UserId.ToString()),
            new Claim("UserRole", usrSession.UserRole.ToString()),
            new Claim("UserName", usrSession.UserName),

            //used by Microsoft.AspNetCore.Authentication and used in the HTTP request pipeline
            new Claim(ClaimTypes.Role, usrSession.UserRole.ToString()),
            new Claim(ClaimTypes.NameIdentifier, TokenId.ToString()),
            new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTimeMinutes).ToString("MMM ddd dd yyyy HH:mm:ss tt"))
        };
        return claims;
    }

    public JwtUserToken CreateJwtUserToken(LoginResponse _usrSession)
    {
        if (_usrSession == null) throw new ArgumentException($"{nameof(_usrSession)} cannot be null");

        var _userToken = new JwtUserToken();
        Guid tokenId = Guid.Empty;

        //get the key from user-secrets and set token expiration time
        var key = System.Text.Encoding.ASCII.GetBytes(_jwtOptions.IssuerSigningKey);
        DateTime expireTime = DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTimeMinutes);

        //generate the token, including my own defined claims, expiration time, signing credentials
        var JWToken = new JwtSecurityToken(issuer: _jwtOptions.ValidIssuer,
            audience: _jwtOptions.ValidAudience,
            claims: CreateClaims(_usrSession, out tokenId),
            notBefore: new DateTimeOffset(DateTime.UtcNow).DateTime,
            expires: expireTime,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

        //generate a JWT user token with some unencrypted information as well
        _userToken.TokenId = tokenId;
        _userToken.EncryptedToken = new JwtSecurityTokenHandler().WriteToken(JWToken);
        _userToken.ExpireTime = expireTime;
        _userToken.UserRole = _usrSession.UserRole.ToString();
        _userToken.UserName = _usrSession.UserName;
        _userToken.UserId = _usrSession.UserId.Value;

        return _userToken;
    }

    public LoginResponse DecodeToken(string _encryptedtoken)
    {
        if (_encryptedtoken == null) return null;

        var _decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(_encryptedtoken);

        var _usr = new LoginResponse();
        foreach (var claim in _decodedToken.Claims)
        {
            switch (claim.Type)
            {
                case "UserId":
                    _usr.UserId = Guid.Parse(claim.Value);
                    break;
                case "UserName":
                    _usr.UserName = claim.Value;
                    break;
                case "UserRole":
                    _usr.UserRole = claim.Value;
                    break;
            }
        }
        return _usr;
    }
}