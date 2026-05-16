namespace Domain;

using System.Security.Cryptography;

public static class RandomPasswordGenerator
{
    public static string Generate()
    {
        const string CapitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string SmallLetters = "abcdefghijklmnopqrstuvwxyz";
        const string Numbers = "0123456789";
        const string SpecialChars = "!@#$%";
        const int Length = 16;
        const string AllChars = SpecialChars + CapitalLetters + SmallLetters + Numbers;

        var rng = RandomNumberGenerator.Create();
        var chars = new char[Length];

        chars[0] = CapitalLetters[RandomIndex(rng, CapitalLetters.Length)];
        chars[1] = SmallLetters[RandomIndex(rng, SmallLetters.Length)];
        chars[2] = Numbers[RandomIndex(rng, Numbers.Length)];
        chars[3] = SpecialChars[RandomIndex(rng, SpecialChars.Length)];

        for (int i = 4; i < Length; i++)
            chars[i] = AllChars[RandomIndex(rng, AllChars.Length)];

        return new (chars.OrderBy(_ => RandomIndex(rng, Length)).ToArray());
    }
    
    private static int RandomIndex(RandomNumberGenerator rng, int max)
    {
        var buffer = new byte[4];
        rng.GetBytes(buffer);
        return (int)(BitConverter.ToUInt32(buffer, 0) % max);
    }
}