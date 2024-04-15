using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PointsServer.DApps;
using PointsServer.DApps.Dtos;
using Volo.Abp;

namespace PointsServer.Controllers;

[RemoteService]
[Area("app")]
[ControllerName("DAppController")]
[Route("api/app/dapps")]
public class DAppController : PointsServerController
{
    private readonly IDAppService _dAppService;

    public DAppController(IDAppService dAppService)
    {
        _dAppService = dAppService;
    }

    [HttpPost]
    public async Task<List<DAppDto>> GetDAppListAsync(GetDAppListInput input)
    {
        return await _dAppService.GetDAppListAsync(input);
    }

    [HttpGet("roles")]
    public async Task<List<RoleDto>> GetRolesAsync()
    {
        return await _dAppService.GetRolesAsync();
    }
}