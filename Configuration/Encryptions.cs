using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using Configuration.Options;

namespace Configuration;

public class Encryptions
{
    private readonly AesEncryptionOptions _aesOption;

    public Encryptions(IOptions<AesEncryptionOptions> aesOptions)
    {
        _aesOption = aesOptions.Value;

        if (string.IsNullOrEmpty(_aesOption.Key))
            throw new InvalidOperationException("AesEncryption:Key is missing from configuration.");
        if (string.IsNullOrEmpty(_aesOption.Salt))
            throw new InvalidOperationException("AesEncryption:Salt is missing from configuration.");

        _aesOption.HashKey(Pbkdf2HashToBytes);
    }

    public string AesEncryptToBase64<T>(T sourceToEncrypt)
    {
        string stringToEncrypt = JsonConvert.SerializeObject(sourceToEncrypt);
        byte[] dataset = Encoding.Unicode.GetBytes(stringToEncrypt);

        using SymmetricAlgorithm algorithm = Aes.Create();
        algorithm.Key = _aesOption.KeyHash;
        algorithm.GenerateIV(); // fresh, random IV every call

        using ICryptoTransform encryptor = algorithm.CreateEncryptor();
        byte[] encryptedBytes = encryptor.TransformFinalBlock(dataset, 0, dataset.Length);

        // Prepend IV so decrypt can read it back
        byte[] result = new byte[algorithm.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(algorithm.IV, 0, result, 0, algorithm.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, algorithm.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    public T AesDecryptFromBase64<T>(string encryptedBase64)
    {
        byte[] fullBytes = Convert.FromBase64String(encryptedBase64);

        using SymmetricAlgorithm algorithm = Aes.Create();
        algorithm.Key = _aesOption.KeyHash;

        byte[] iv = fullBytes[..algorithm.IV.Length];
        byte[] cipherBytes = fullBytes[algorithm.IV.Length..];

        using ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, iv);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        string decryptedString = Encoding.Unicode.GetString(decryptedBytes);
        return JsonConvert.DeserializeObject<T>(decryptedString);
    }

    public byte[] Pbkdf2HashToBytes(int nrBytes, string password)
    {
        byte[] registeredPasswordKeyDerivation = KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.UTF8.GetBytes(_aesOption.Salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: _aesOption.Iterations,
            numBytesRequested: nrBytes);

        return registeredPasswordKeyDerivation;
    }

    public string EncryptPasswordToBase64(string password)
    {
        //Hash a password using salt and streching
        byte[] encrypted = Pbkdf2HashToBytes(64, password);
        return Convert.ToBase64String(encrypted);
    }

}