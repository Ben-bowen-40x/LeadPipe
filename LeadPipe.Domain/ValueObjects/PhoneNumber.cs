using System.Text.RegularExpressions;

namespace LeadPipe.Domain.ValueObjects;

public partial class PhoneNumber
{
    #region Public
    public override string ToString()
    {
        return $"Phone Number: {Number}";
    }

    private bool? _isDefault;
    public bool IsDefault
    {
        get { return _isDefault ??= Number == Default || Number == 1111111111 || Number > 9999999999; }
    }

    public static readonly long Default = 0;

    public long Number { get; }

    public PhoneNumber(PhoneNumber number)
    {
        Number = number.Number;
    }

    public PhoneNumber(long number)
    {
        Number = ValidateNumericalInput(number);
    }

    public PhoneNumber(string? number)
    {
        Number = ValidateStringInput(number);
    }

    public static bool TryParse(string? number, out PhoneNumber result)
    {
        try
        {
            result = new(number);
            return true;
        }
        catch { }
        result = new(Default);
        return false;
    }
    #endregion

    #region Private
    long ValidateStringInput(string? number)
    {
        if (string.IsNullOrWhiteSpace(number))
            return Default;
        var split = number.Split("ex");
        if (split.Length == 0)
            return Default;

        string clean = NonDigitChar().Replace(split[0], string.Empty);
        long result = StrToLong(clean);
        return result;
    }

    long ValidateNumericalInput(long number)
    {
        return StrToLong(number.ToString());
    }

    static readonly char[] _unacceptable = { '0', '1' };
    static long StrToLong(string number)
    {
        bool small = number.Length < 10;
        if (small) return Default;

        bool unacceptable = _unacceptable.Contains(number[^10]);
        bool parses = long.TryParse(number[^10..], out long result);

        if (parses && !unacceptable) return result;
        return Default;
    }

    [GeneratedRegex(@"\D")]
    private static partial Regex NonDigitChar();
    #endregion

    #region Equality

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj is not PhoneNumber) return false;
        return Equals(obj as PhoneNumber);
    }

    public bool Equals(PhoneNumber? that)
    {
        if (ReferenceEquals(this, that)) return true;
        if (that is null) return false;

        return Number == that.Number;
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }

    public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
    {
        return !(left == right);
    }

    #endregion
}
