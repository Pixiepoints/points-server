using System.Threading.Tasks;
using AElf;
using AElf.Types;
using Points.Contracts.Point;
using Portkey.Contracts.CA;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PointsServer.Points;

[Collection(PointsServerTestConsts.CollectionDefinitionName)]
public class PointsServiceTest : PointsServerApplicationTestBase
{
    public PointsServiceTest(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task GetMyPointsAsyncTest()
    {
        var t = ByteArrayHelper.HexStringToByteArray(
            "0a220a203b27b8b1f3bb09af3229f26e14f8092946de4bdb089bbd30a63c045ed0ff333312220a2088881d4350a8c77c59a42fc86bbcd796b129e086da7e61d24fb86a6cbb6b2f3b18f9d788332204420ac9792a124d616e61676572466f727761726443616c6c32630a220a20764c0233b7d4559057b46ceb9210d2cf269e82fc73449054ab84081f954c73f812220a20978ed506e8a894ed1425a6d4b968a620daeae7e0c6deb4bdc9405475e30169381a044a6f696e22130a11736368726f64696e67657261692e636f6d82f10441179c534405eb283b709f4fd570c191fa1959ff890173fc822ff1d9dd1d0f151a24dceb713f8bc2fdacfbe0204abc30522e9ce1da657c45b8a826dd6f8916600d00");
        var t1 = Transaction.Parser.ParseFrom(t);
        var p = ManagerForwardCallInput.Parser.ParseFrom(t1.Params);
        var i = JoinInput.Parser.ParseFrom(p.Args);
        p.ShouldNotBeNull();
    }
}