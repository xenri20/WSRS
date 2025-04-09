using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WSRS_Student.Helper;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var attribute = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>();

        return attribute?.Name ?? enumValue.ToString();
    }
}
