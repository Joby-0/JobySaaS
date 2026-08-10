using System.ComponentModel.DataAnnotations;

namespace Models.DTO;

public class LoginRequest
{
    public string UserNameOrEmail { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public string UserRole { get; set; }
    public JwtUserToken JwtToken { get; set; }
}

public class RegisterRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class RegisterResponse
{
   public bool Success { get; set; }
   public string Message { get; set; }
}

public class JwtUserToken
{
    public Guid TokenId { get; set; }

    public string EncryptedToken { get; set; }
    public DateTime ExpireTime { get; set; }

    //This will be the User part of the Claim, which can later be retrieved
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserRole { get; set; }
}
