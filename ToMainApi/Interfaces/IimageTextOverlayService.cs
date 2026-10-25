using ToMainApi.Common;

namespace ToMainApi.Interfaces
{
    public interface IimageTextOverlayService
    {
        Task<ServiceResponse<MemoryStream>> AddText(Stream photo, IReadOnlyList<string> lines);
    }
}
