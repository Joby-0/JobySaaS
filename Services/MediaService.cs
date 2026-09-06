using DbModels;
using DbRepos;
using Models.DTO;
using Microsoft.AspNetCore.Http;

namespace Services;

public class MediaService : IMediaService
{
    private readonly MediaDbRepo _repo;
    private readonly OrganizationDbRepo _organizationRepo;
    private readonly SocialAccountDbRepo _socialAccountRepo;
    private readonly IPublishQueue _publishQueue;

    public MediaService(MediaDbRepo repo, OrganizationDbRepo organizationRepo, SocialAccountDbRepo socialAccountRepo, IPublishQueue publishQueue)
    {
        _repo = repo;
        _organizationRepo = organizationRepo;
        _socialAccountRepo = socialAccountRepo;
        _publishQueue = publishQueue;
    }

    public async Task<ServiceResult<List<MediaListDTO>>> GetMediaListAsync(Guid organizationId, Guid requestUserId, int pageNumber, int pageSize)
    {
        var access = await EnsureOrganizationAccessAsync(organizationId, requestUserId);
        if (!access.Success)
        {
            return ServiceResult<List<MediaListDTO>>.Fail(access.Error!);
        }

        if (pageNumber < 1 || pageSize < 1)
        {
            return ServiceResult<List<MediaListDTO>>.Fail("Page number and page size must be greater than zero.");
        }

        var media = await _repo.GetMediaListAsync(organizationId, pageNumber, pageSize);
        return ServiceResult<List<MediaListDTO>>.Ok("Media retrieved successfully.", media.Select(ToListDto).ToList());
    }

    public async Task<ServiceResult<MediaDetailsDTO>> GetMediaDetailsAsync(Guid organizationId, Guid mediaId, Guid requestUserId)
    {
        var access = await EnsureOrganizationAccessAsync(organizationId, requestUserId);
        if (!access.Success)
        {
            return ServiceResult<MediaDetailsDTO>.Fail(access.Error!);
        }

        var media = await _repo.GetMediaDetailsAsync(organizationId, mediaId);
        if (media is null)
        {
            return ServiceResult<MediaDetailsDTO>.Fail("Media could not be found.");
        }

        return ServiceResult<MediaDetailsDTO>.Ok(
            "Media retrieved successfully.",
            ToDetailsDto(media));
    }

    public async Task<ServiceResult<Guid>> CreateMediaAsync(Guid organizationId, IFormFile file, string title, string description, Guid requestUserId)
    {
        var access = await EnsureOrganizationAccessAsync(organizationId, requestUserId);
        if (!access.Success)
            return ServiceResult<Guid>.Fail(access.Error!);

        if (file is null || file.Length == 0)
            return ServiceResult<Guid>.Fail("No media file was uploaded.");

        await using var input = file.OpenReadStream();
        await using var buffer = new MemoryStream();
        await input.CopyToAsync(buffer);

        var media = await _repo.CreateAsync(new MediaDbM
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            FileContent = buffer.ToArray(),
            FileName = Path.GetFileName(file.FileName),
            MimeType = file.ContentType,
            FileSize = file.Length,
            Title = title,
            Description = description,
            CreatedAt = DateTime.UtcNow
        });

        return ServiceResult<Guid>.Ok("Media uploaded successfully.", media.Id);
    }

    public async Task<ServiceResult<Guid>> PublishMediaAsync(Guid organizationId, Guid mediaId, List<Guid> socialAccountIds, Guid requestUserId)
    {
        var access = await EnsureOrganizationAccessAsync(organizationId, requestUserId);
        if (!access.Success)
            return ServiceResult<Guid>.Fail(access.Error!);

        if (socialAccountIds is null || socialAccountIds.Count == 0)
            return ServiceResult<Guid>.Fail("At least one social account must be supplied.");

        var distinctIds = socialAccountIds.Distinct().ToList();
        if (distinctIds.Count != socialAccountIds.Count)
            return ServiceResult<Guid>.Fail("Duplicate social account IDs are not allowed.");

        var media = await _repo.GetByIdAsync(organizationId, mediaId);
        if (media is null)
            return ServiceResult<Guid>.Fail("Media could not be found in this organization.");

        var accounts = await _socialAccountRepo.GetByIdsInOrganizationAsync(organizationId, distinctIds);
        if (accounts.Count != distinctIds.Count)
            return ServiceResult<Guid>.Fail("One or more selected social accounts do not belong to this organization.");

        var now = DateTime.UtcNow;
        var job = new PublishJobDbM
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            MediaId = mediaId,
            Status = Models.PublishJobStatus.Pending,
            CreatedAt = now,
            Accounts = accounts.Select(account => new PublishJobAccountDbM
            {
                Id = Guid.NewGuid(),
                SocialAccountId = account.Id,
                Status = Models.PublishJobAccountStatus.Pending,
                CreatedAt = now
            }).ToList()
        };

        await _repo.CreatePublishJobAsync(job);
        await _publishQueue.EnqueueAsync(job.Id);
        return ServiceResult<Guid>.Ok("Media publishing job queued.", job.Id);
    }

    private async Task<ServiceResult<bool>> EnsureOrganizationAccessAsync(Guid organizationId, Guid requestUserId)
    {
        var membership = await _organizationRepo.GetUserOrganizationAsync(organizationId, requestUserId);
        return membership is null
            ? ServiceResult<bool>.Fail("You do not have access to this organization.")
            : ServiceResult<bool>.Ok(string.Empty, true);
    }

    private static MediaListDTO ToListDto(MediaDbM media)
    {
        var videos = media.SocialVideoDbMs ?? new List<SocialVideoDbM>();
        return new MediaListDTO
        {
            Id = media.Id,
            ThumbnailUrl = media.ThumbnailUrl,
            Title = media.Title,
            Description = media.Description,
            Duration = media.Duration,
            CreatedAt = media.CreatedAt,
            SocialAccountCount = videos.Count,
            SocialPlatforms = videos.Select(video => video.Platform).Distinct().ToList()
        };
    }

    private static MediaDetailsDTO ToDetailsDto(MediaDbM media)
    {
        var videos = media.SocialVideoDbMs ?? new List<SocialVideoDbM>();
        var platforms = videos.Select(video => video.Platform).ToHashSet();
        var accounts = media.OrganizationDbM?.SocialAccountDbMs ?? new List<SocialAccountDbM>();

        return new MediaDetailsDTO
        {
            Id = media.Id,
            ThumbnailUrl = media.ThumbnailUrl,
            Title = media.Title,
            Description = media.Description,
            Duration = media.Duration,
            CreatedAt = media.CreatedAt,
            SocialAccounts = accounts
                .Where(account => platforms.Contains(account.Platform))
                .Select(account => new SocialAccountDto
                {
                    Id = account.Id,
                    Platform = account.Platform,
                    AccountName = account.Username,
                    CostumUrl = account.CostumUrl,
                    ProfileImageUrl = account.ProfileImageUrl,
                    Followers = account.Followers,
                    Status = account.Status,
                    LastSync = account.LastSync
                }).ToList(),
            SocialVideos = videos.Cast<Models.ISocialVideo>().ToList()
        };
    }
}
