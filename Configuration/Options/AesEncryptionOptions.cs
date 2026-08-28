namespace Configuration.Options;

public class AesEncryptionOptions
{
    public const string Position = "AesEncryption";
    public string Key { get; set; }
    public string Salt { get; set; }
    public int Iterations { get; set; }

    public byte[] KeyHash { get; private set; }

    public void HashKey(Func<int, string, byte[]> hasher)
    {
        KeyHash = hasher.Invoke(32, Key); // 32 = AES-256, matches the Blazor project's choice
    }
}