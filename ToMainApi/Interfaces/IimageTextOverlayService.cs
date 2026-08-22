namespace ToMainApi.Interfaces
{
    public interface IimageTextOverlayService
    {
        Task<string> AddText(Stream photo, string text);
    }
}
