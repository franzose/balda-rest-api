using System.Text.Json.Serialization;

namespace Balda.Tests.Support
{
    public sealed class UserCredentials
    {
        [JsonPropertyName("username")] public string UserName { get; set; }
        [JsonPropertyName("password")] public string Password { get; set; }
    }
}