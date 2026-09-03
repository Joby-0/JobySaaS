using Models.DTO;

namespace Services;

public interface IMediaService
{
    Task<ServiceResult<List<MediaListDTO>>> GetMediaListAsync(
        Guid organizationId,
        Guid requestUserId,
        int pageNumber,
        int pageSize);

    Task<ServiceResult<MediaDetailsDTO>> GetMediaDetailsAsync(
        Guid organizationId,
        Guid mediaId,
        Guid requestUserId);
}
