using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PointsServer.Points;
using PointsServer.Points.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace PointsServer.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("DAppController")]
[Route("api/app/points")]
public class PointsController : PointsServerController
{
    private readonly IPointsService _pointsService;

    public PointsController(IPointsService pointsService)
    {
        _pointsService = pointsService;
    }

    [HttpPost("ranking")]
    public async Task<PagedResultDto<RankingListDto>> GetRankingListAsync(GetRankingListInput input)
    {
        return await _pointsService.GetRankingListAsync(input);
    }

    [HttpPost("ranking/detail")]
    public async Task<RankingDetailDto> GetRankingDetailAsync(GetRankingDetailInput input)
    {
        return await _pointsService.GetRankingDetailAsync(input);
    }

    [HttpPost("earned/list")]
    public async Task<PagedResultDto<PointsEarnedListDto>> GetPointsEarnedListAsync(GetPointsEarnedListInput input)
    {
        return await _pointsService.GetPointsEarnedListAsync(input);
    }

    [HttpPost("earned/detail")]
    public async Task<PointsEarnedDetailDto> GetPointsEarnedDetailAsync(GetPointsEarnedDetailInput input)
    {
        return await _pointsService.GetPointsEarnedDetailAsync(input);
    }

    [HttpGet("my/points")]
    public async Task<MyPointDetailsDto> GetMyPointsAsync(GetMyPointsInput input)
    {
        return await _pointsService.GetMyPointsAsync(input);
    }
    
    [HttpPost("list")]
    public async Task<List<PointsListDto>> GetPointsListAsync(GetPointsListInput input)
    {
        return await _pointsService.GetPointsListAsync(input);
    }
    
    [HttpPost("relationship")]
    public async Task<List<RelationshipDto>> GetRelationshipListAsync(GetRelationshipInput input)
    {
        return await _pointsService.GetRelationshipListAsync(input);
    }
    
    [HttpPost("all/list")]
    public async Task<List<PointsListDto>> GetAllPointsListAsync(GetAllPointsListInput input)
    {
        return await _pointsService.GetAllPointsListAsync(input);
    }
}