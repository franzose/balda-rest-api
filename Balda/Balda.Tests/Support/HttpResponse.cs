using System.Text.Json.Serialization;

namespace Balda.Tests.Support
{
    public sealed class HttpResponse
    {
        [JsonPropertyName("message")] public string Message { get; set; }

        /// <summary>
        /// The field from the "problem" response 
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail
        {
            set => Message = value;
        }
    }
}