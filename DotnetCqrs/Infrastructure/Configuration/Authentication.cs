// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Security.Cryptography;
// using System.Text;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
//
//
// namespace DotnetCqrs.Infrastructure.Configuration;
//
// internal static class AuthenticationConfiguration
// {
//     private static readonly string JwtIssuer =
//         Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "https://issuer.stone.one";
//
//     private static readonly string JwtAudience =
//         Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "https://audience.stone.one";
//
//
//     private static readonly SymmetricSecurityKey Key =
//         new(
//             Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ??
//                                    "secret.stone.one"));
//
//     private static readonly double JwtExpired =
//         double.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRED") ?? "100000");
//
//     private static readonly double JwtRefreshExpired =
//         double.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRED") ?? "100000");
//
//     public static void AppAuthentication(this IServiceCollection service)
//     {
//         service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//             .AddJwtBearer(options =>
//             {
//                 options.Audience = JwtAudience;
//                 options.TokenValidationParameters = new TokenValidationParameters
//                 {
//                     ValidIssuer = JwtIssuer,
//                     IssuerSigningKey = Key
//                 };
//             });
//         service.AddAuthorization();
//     }
//
//
//     public static Token GenerateToken(int id, string type)
//     {
//         var expired = DateTime.Now.AddSeconds(JwtExpired);
//         var refreshToken = Guid.NewGuid().ToString();
//         var accessToken = new JwtSecurityTokenHandler()
//             .WriteToken(
//                 new JwtSecurityToken(JwtIssuer,
//                     JwtAudience,
//                     [
//                         new Claim("Id", id.ToString()),
//                         new Claim(ClaimTypes.Role, type),
//                         // new Claim("CompanyId", companyId.ToString() ?? "0"),
//                         new Claim("RefreshToken", refreshToken)
//                     ],
//                     expires: expired,
//                     signingCredentials: new SigningCredentials(Key, SecurityAlgorithms.HmacSha256)
//                 )
//             );
//         var notBefore = DateTime.Now.AddSeconds(JwtRefreshExpired);
//         return new Token(accessToken, refreshToken, expired, notBefore);
//     }
//
//     private static byte[] GetSalt()
//     {
//         var salt = new byte[16];
//         using var random = RandomNumberGenerator.Create();
//         random.GetBytes(salt);
//         return salt;
//     }
//
//     public static string HashPassword(string password)
//     {
//         var salt = GetSalt();
//         using var hmac = new HMACSHA256(salt);
//         var passwordBytes = Encoding.UTF8.GetBytes(password);
//         var hashedBytes = hmac.ComputeHash(passwordBytes);
//         var saltedHash = new byte[salt.Length + hashedBytes.Length];
//         Array.Copy(salt, 0, saltedHash, 0, salt.Length);
//         Array.Copy(hashedBytes, 0, saltedHash, salt.Length, hashedBytes.Length);
//         return Convert.ToBase64String(saltedHash);
//     }
//
//     public static bool VerifyPassword(string inputPassword, string storedHash)
//     {
//         var saltedHashBytes = Convert.FromBase64String(storedHash);
//         var salt = saltedHashBytes.Take(16).ToArray();
//         var storedPasswordHash = saltedHashBytes.Skip(16).ToArray();
//
//         using var hmac = new HMACSHA256(salt);
//         var inputPasswordBytes = Encoding.UTF8.GetBytes(inputPassword);
//         var computedHash = hmac.ComputeHash(inputPasswordBytes);
//
//         return computedHash.SequenceEqual(storedPasswordHash);
//     }
//
//     public static Auth GetClaimFromExpiredToken(string accessToken)
//     {
//         var tokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = JwtIssuer,
//             ValidAudience = JwtAudience,
//             IssuerSigningKey = Key
//         };
//         var tokenHandler = new JwtSecurityTokenHandler();
//         var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out _);
//         return new Auth(principal.Claims);
//     }
// }
//
// public record Token(string AccessToken, string RefreshToken, DateTime Expired, DateTime NotBefore);
//
// public class Auth
// {
//     public Auth(IEnumerable<Claim> claims)
//     {
//         var dict = claims.ToDictionary(k => k.Type, v => v.Value);
//         Id = int.Parse(dict["Id"]);
//         RefreshToken = dict.GetValueOrDefault("RefreshToken") ?? "";
//     }
//
//     public int Id { get; }
//     public string RefreshToken { get; }
// }
//