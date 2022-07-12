
namespace Utils;
public static class NumbersExtensions
{
    public static T ToPositive<T>(this T number)
    where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        var zero = default(T);
        return number.CompareTo(zero) < 0 ? zero : number;
    }
}
