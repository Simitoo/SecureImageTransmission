namespace SecureImageTransmissionAPI.Models
{
    public class Notification
    {
        private string _sender;
        private string _content;

        public Notification(string sender, string content)
        {
            Id = Guid.NewGuid();
            Sender = sender;
            Content = content;
            SentAt = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public string Sender
        {
            get => _sender;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Sender cannot be null or empty");
                }
                _sender = value;
            }

        }
        public string Content
        {
            get => _content;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Content cannot be null or empty");
                }
                _content = value;
            }
        }
        public DateTime SentAt { get; private set; }
    }
}
