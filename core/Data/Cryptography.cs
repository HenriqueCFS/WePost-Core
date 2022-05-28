using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace core;

public class SigningConfigurations
{
    public SecurityKey Key { get; }
    public SigningCredentials SigningCredentials { get; }

    public SigningConfigurations()
    {
        using (var provider = new RSACryptoServiceProvider(2048))
        {
            Key = new RsaSecurityKey(provider.ExportParameters(true));
        }

        SigningCredentials = new SigningCredentials(
            Key, SecurityAlgorithms.RsaSha256Signature);
    }
};

public class TokenConfigurations
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int Seconds { get; set; }
};

