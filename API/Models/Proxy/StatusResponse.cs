using System.Text.Json.Serialization;

namespace Knab.Assignment.API.Models.Proxy
{
    public class StatusResponse
    {
        [JsonConstructor]
        public StatusResponse(int errorCode, string? errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        [JsonPropertyName("error_code")]
        public int ErrorCode { get; }

        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; }
    }
}