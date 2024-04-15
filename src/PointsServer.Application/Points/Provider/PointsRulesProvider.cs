using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PointsServer.Options;
using Volo.Abp.DependencyInjection;

namespace PointsServer.Points.Provider;

public interface IPointsRulesProvider
{
    Task<Dictionary<string, Dictionary<string, PointsRules>>> GetAllPointsRulesAsync();
    Task<PointsRules> GetPointsRulesAsync(string dappName, string action);
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
            throw new Exception("invalid dappName points rules");
        }

        if (!actionPointsRulesDic.TryGetValue(action, out var pointsRules))
        {
            throw new Exception("invalid action points rules");
        }

        return pointsRules;
    }
}