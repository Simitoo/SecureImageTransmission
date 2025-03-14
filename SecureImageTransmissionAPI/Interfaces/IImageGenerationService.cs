using SecureImageTransmissionAPI.Common;

namespace SecureImageTransmissionAPI.Interfaces
{
    public interface IImageGenerationService
    {
        Result<string> StartGenerating(int width, int height, string format);
        void StopGenerating();
    }
}
