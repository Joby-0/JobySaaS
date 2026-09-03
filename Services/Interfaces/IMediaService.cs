using Models.DTO;
using Microsoft.AspNetCore.Http;

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

    Task<ServiceResult<Guid>> CreateMediaAsync(
        Guid organizationId,
        IFormFile file,
        string title,
        string description,
        Guid requestUserId);

    Task<ServiceResult<bool>> PublishMediaAsync(
        Guid organizationId,
        Guid mediaId,
        List<Guid> socialAccountIds,
        Guid requestUserId);
}
