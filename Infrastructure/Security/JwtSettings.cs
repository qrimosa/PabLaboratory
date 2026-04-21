using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

public class JwtSettings(IConfiguration configuration)
{
    private const string Section = "Jwt";

    public string Issuer => configuration[$"{Section}:Issuer"] ?? throw new Exception("Issuer missing");
    public string Audience => configuration[$"{Section}:Audience"] ?? throw new Exception("Audience missing");
    public string Secret => configuration[$"{Section}:SecretKey"] ?? throw new Exception("Secret missing");
    
    // Explicitly parse to avoid default 0 values
    public int ExpirationInMinutes => int.TryParse(configuration[$"{Section}:ExpiryInMinutes"], out var val) ? val : 60;
    public int RefreshTokenDays => int.TryParse(configuration[$"{Section}:RefreshTokenDays"], out var val) ? val : 7;

    public SymmetricSecurityKey GetSymmetricKey() =>
        new(Encoding.UTF8.GetBytes(Secret));
}