using ToMainApi.Common;

namespace ToMainApi.Interfaces
{
    public interface IimageMetadataEditor
    {
        Task<ServiceResponse<MemoryStream>> ProcessImage(Stream photo);
    }
}
