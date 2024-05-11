using System.Collections.Generic;
using System.Threading.Tasks;
using PointsServer.Points.Dtos;
using Volo.Abp.Application.Dtos;

namespace PointsServer.Points;

public interface IPointsService
{
    Task<PagedResultDto<RankingListDto>> GetRankingListAsync(GetRankingListInput input);
    Task<RankingDetailDto> GetRankingDetailAsync(GetRankingDetailInput input);
    Task<GetPointsEarnedListDto> GetPointsEarnedListAsync(GetPointsEarnedListInput input);
    Task<PointsEarnedDetailDto> GetPointsEarnedDetailAsync(GetPointsEarnedDetailInput input);
    Task<MyPointDetailsDto> GetMyPointsAsync(GetMyPointsInput input);
    Task<List<PointsListDto>> GetPointsListAsync(GetPointsListInput input);
    Task<List<RelationshipDto>> GetRelationshipListAsync(GetRelationshipInput input);
}