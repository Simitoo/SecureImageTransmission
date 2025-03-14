namespace SecureImageTransmissionAPI.Common
{
    public class Result<T>
    {     
        public bool IsSuccess { get; }
        public string? Message { get; }
        public T? Value { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        private Result(string message)
        {
            IsSuccess = false;
            Message = message;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string message) => new(message);
    }
}
