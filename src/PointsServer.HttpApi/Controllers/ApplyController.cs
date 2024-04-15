using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PointsServer.Apply;
using PointsServer.Apply.Dtos;
using Volo.Abp;

namespace PointsServer.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("ApplyController")]
[Route("api/app/apply")]
public class ApplyController : PointsServerController
{
    private readonly IApplyService _applyService;

    public ApplyController(IApplyService applyService)
    {
        _applyService = applyService;
    }

    [HttpPost("check")]
    public async Task<ApplyCheckResultDto> ApplyCheckAsync(ApplyCheckInput input)
    {
        return await _applyService.ApplyCheckAsync(input);
    }

    [Authorize]
    [HttpPost("confirm")]
    public async Task<ApplyConfirmDto> GetDAppListAsync(ApplyConfirmInput input)
    {
        return await _applyService.ApplyConfirmAsync(input);
    }

    [HttpPost("domain/check")]
    public async Task<DomainCheckDto> DomainCheckAsync(ApplyCheckInput input)
    {
        return await _applyService.DomainCheckAsync(input);
    }
    
    [Authorize]
    [HttpGet("internal/changeWorkerTime")]
    public async Task<bool> InternalChangeWorkerTimeAsync(long milliseconds)
    {
        return await _applyService.InternalChangeWorkerTimeAsync(milliseconds);
    }
    
    [Authorize]
    [HttpGet("internal/getWorkerTime")]
    public async Task<long> InternalGetWorkerTimeAsync()
    {
        return await _applyService.InternalGetWorkerTimeAsync();
    }
}