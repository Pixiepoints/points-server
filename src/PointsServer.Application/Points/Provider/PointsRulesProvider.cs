using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PointsServer.Options;
using PointsServer.Points.Dtos;
using Volo.Abp.DependencyInjection;

namespace PointsServer.Points.Provider;

public interface IPointsRulesProvider
{
    Task<Dictionary<string, Dictionary<string, PointsRules>>> GetAllPointsRulesAsync();
    Task<PointsRules> GetPointsRulesAsync(string dappName, string action);
    Task<List<EarnedPointDto>> AddAllPoints(List<EarnedPointDto> pointList, string dappName);
}

public class PointsRulesProvider : IPointsRulesProvider, ISingletonDependency
{
    private readonly IOptionsMonitor<PointsRulesOption> _pointsRulesOption;

    public PointsRulesProvider(IOptionsMonitor<PointsRulesOption> pointsRulesOption)
    {
        _pointsRulesOption = pointsRulesOption;
    }

    public async Task<Dictionary<string, Dictionary<string, PointsRules>>> GetAllPointsRulesAsync()
    {
        return _pointsRulesOption.CurrentValue.PointsRulesList
            .GroupBy(rule => rule.DappId)
            .ToDictionary(
                group => group.Key,
                group => group.ToDictionary(
                    rule => rule.Action,
                    rule => rule
                )
            );
    }

    public async Task<PointsRules> GetPointsRulesAsync(string dappName, string action)
    {
        var allPointsRulesDic = await GetAllPointsRulesAsync();
        if (!allPointsRulesDic.TryGetValue(dappName, out var actionPointsRulesDic))
        {
            return null;
        }

        return !actionPointsRulesDic.TryGetValue(action, out var pointsRules) ? null : pointsRules;
    }
    
    
    public async Task<List<EarnedPointDto>> AddAllPoints(List<EarnedPointDto> pointList, string dappName)
    {
        var result = new List<EarnedPointDto>(pointList);
        var currentActionList = pointList.Select(x => x.Action).ToList();
        var allPointsRulesDic = await GetAllPointsRulesAsync();
        if (!allPointsRulesDic.TryGetValue(dappName, out var actionPointsRulesDic))
        {
            throw new Exception("invalid dappName points rules");
        }
        
        var allSActionList = actionPointsRulesDic.Select(x => x.Key).ToList();
        foreach (var action in allSActionList)
        {
            if (!currentActionList.Contains(action) && action != "Default")
            {
                var rule = actionPointsRulesDic[action];
                result.Add(new EarnedPointDto()
                {
                    Action = action,
                    Symbol = rule.Symbol,
                    DisplayName = rule.DisplayNamePattern,
                });
            }
        }

        return result;
    }
}