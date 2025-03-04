namespace SecureImageTransmissionAPI.Interfaces
{
    public interface IImageGenerationService
    {
        void StartGenerating(int width, int height, string format);
        void StopGenerating();
    }
}
