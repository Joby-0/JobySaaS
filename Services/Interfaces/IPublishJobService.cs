using Models.DTO;

namespace Services;

public interface IPublishJobService
{
    Task<ServiceResult<PublishJobDto>> GetStatusAsync(Guid organizationId, Guid jobId, Guid requestUserId);
}
