namespace SecureImageTransmissionAPI.Models
{
    public class ImageModel
    {
        public ImageModel(byte[] data, string formart)
        {
            Id = Guid.NewGuid();
            Data = data;
            Format = formart;
            CreatedAt = DateTime.UtcNow;
            FileName = ImageNameFormatting();
        }

        public Guid Id { get; }
        public byte[] Data { get; }
        public string Format { get; }
        public string FileName { get; }
        public DateTime CreatedAt { get; }

        public string ToBase64()
        {
            return $"data:image/{Format};base64,{Convert.ToBase64String(Data)}";
        }
        private string ImageNameFormatting()
        {
            return $"{CreatedAt:yyyyMMddHHmmssfff}-{Id}.{Format}";
        }
    }
}
