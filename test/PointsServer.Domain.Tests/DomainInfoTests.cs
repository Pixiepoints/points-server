using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using PointsServer.Common;
using PointsServer.Common.GraphQL;
using PointsServer.Points.Dtos;
using PointsServer.Points.Provider;
using Xunit;
using Xunit.Abstractions;

namespace PointsServer;

public class DomainInfoTests : PointsServerDomainTestBase
{
    //private IPointsProvider _pointsProvider;
    private IGraphQLClient _client;
    public DomainInfoTests(ITestOutputHelper output) : base(output)
    {
       // _pointsProvider = GetRequiredService<IPointsProvider>();
        _client = GetRequiredService<IGraphQLClient>();
    }

    [Fact]
    public async Task Domain_test()
    {
        var a =await Graphics();

    }


    public async Task<RankingDetailIndexerListDto> Graphics()
    {
        var _graphQlHelper = new GraphQlHelper(_client,null);
        GetOperatorPointsActionSumInput queryInput = new GetOperatorPointsActionSumInput
        {
            DappName = "405c38e39972c7d62f50cc6c1a5553364c50cad0c6676d7f368c2439aaacb862",
            Address = "21Xf9xaARFqzh1WdGEtMMBJYPLVHD6ma2DsuzcfuGjdEDCawPq",
            Domain = "schrodingerai.com",
            Role = OperatorRole.User
        };
        var indexerResult = await _graphQlHelper.QueryAsync<RankingDetailIndexerQueryDto>(new GraphQLRequest
        {
            Query =
                @"query($dappId:String!, $address:String!, $domain:String!, $role:IncomeSourceType){
                    getPointsSumByAction(input: {dappId:$dappId,address:$address,domain:$domain,role:$role}){
                        totalRecordCount,
                        data{
                        id,
                        address,
                        domain,
                        role,
                        dappId,
    					pointsName,
    					actionName,
    					amount,
    					createTime,
    					updateTime
                    }
                }
            }",
            Variables = new
            {
                dappId = queryInput.DappName, address = queryInput.Address, domain = queryInput.Domain,
                role = queryInput.Role
            }
        });
        return indexerResult.GetPointsSumByAction;
    }


}