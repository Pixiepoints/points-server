using System.Linq;
using System.Text.RegularExpressions;
using AElf;
using AElf.Cryptography;
using AElf.Types;
using Google.Protobuf;

namespace PointsServer.Common;

public static class VerifyHelper
{
    public static bool VerifyEmail(string address)
    {
        // string emailRegex =
        //     @"([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+";

        var emailRegex = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        var emailReg = new Regex(emailRegex);
        return emailReg.IsMatch(address.Trim());
    }

    public static bool VerifyPhone(string phoneNumber)
    {
        var phoneRegex = @"^1[0-9]{10}$";
        var emailReg = new Regex(phoneRegex);
        return emailReg.IsMatch(phoneNumber.Trim());
    }


    public static bool IsPhone(string input)
    {
        var pattern = @"^\+\d+$";
        return Regex.IsMatch(input, pattern);
    }

    public static bool IsEmail(string input)
    {
        return input.Count(c => c == '@') == 1;
    }
}