
namespace Nixie.Utils;

/// <summary>
/// Hashes values to be used in consistent hash
/// </summary>
public class Hash
{
    public static int Get(string data)
    {
        return Math.Abs(data.GetHashCode());
    }

    public static int Get(int data)
    {
        return Math.Abs(data);
    }

    public static int Get(object data)
    {
        return Math.Abs(data.GetHashCode());
    }
}

