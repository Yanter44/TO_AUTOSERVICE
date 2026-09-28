using ToMainApi.Common;
using ToMainApi.Models.Dtos.Pagination;
using ToMainApi.Models.Dtos.PhotoGallery;

namespace ToMainApi.Interfaces
{
    public interface IPhotoGalleryService
    {
        Task<ServiceResponse<PagedResponse<PhotoInGalleryDto>>> GetPhotoGallery(PaginationDto paginationModel);
        Task<ServiceResponse<bool>> DeletePhotoFromPhotoGallery(int photoId);
        Task<ServiceResponse<PhotoInGalleryDto>> AddNewPhotoToGallery(AddNewPhotoToGalleryDto model);
    }
}
