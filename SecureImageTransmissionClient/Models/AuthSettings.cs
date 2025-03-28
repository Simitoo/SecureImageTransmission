﻿namespace SecureImageTransmissionClient.Models
{
    public class AuthSettings
    {
        public string Domain { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty;
    }
}
