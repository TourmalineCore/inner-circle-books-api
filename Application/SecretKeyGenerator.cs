namespace Application;

public static class SecretKeyGenerator
{
    private static readonly Random _random = new Random();
    private const string CHARS = "abcdefghijklmnopqrstuvwxyz0123456789";

    public static string Generate()
    {
        return new string(Enumerable
            .Repeat(CHARS, 4)
            .Select(s => s[_random.Next(s.Length)])
            .ToArray());
    }
}