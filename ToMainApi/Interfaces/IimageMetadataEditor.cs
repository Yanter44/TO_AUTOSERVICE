using ToMainApi.Common;

namespace ToMainApi.Interfaces
{
    public interface IimageMetadataEditor
    {
        Task<ServiceResponse<string>> ProcessImage(Stream photo);
    }
}
