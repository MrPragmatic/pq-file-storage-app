using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// public static AesEncryptionUtility class is responsible for handling the AES-GCM encryption and decryption logic
// as part of envelope encryption
public static class AesEncryptionUtility
{
    // Initialize private static readonly KeySizeBytes, NonceSizeBytes, and TagSizeBytes
    // referring to: Brady, S. (2021). Securely encrypting and decrypting files in .NET Core with AES-GCM
    // from https://www.thomaslevesque.com/2021/02/21/securely-encrypting-and-decrypting-files-in-net-core-with-aes-gcm/
    // Accessed 1 August 2024
    // AES-256 key size is 32 bytes: 32 bytes * 8 bits/byte = 256 bits
    public static readonly int KeySizeBytes = 32;
    // nonce size 12 bytes * 8 bits/byte = 96 bits, recommended for AES-GCM
    public static readonly int NonceSizeBytes = 12;
    // tag size 16 bytes * 8 bits/byte = 128 bits, recommended for AES-GCM
    public static readonly int TagSizeBytes = 16;

    // EncryptFile method is responsible for encrypting the file
    public static void EncryptFile(string inputFilePath, string outputFilePath, byte[] key)
    {
        // if the inputFilePath is null or empty, throw an ArgumentNullException
        if (string.IsNullOrEmpty(inputFilePath))
            throw new ArgumentNullException(nameof(inputFilePath));
        // if the outputFilePath is null or empty, throw an ArgumentNullException
        if (string.IsNullOrEmpty(outputFilePath))
            throw new ArgumentNullException(nameof(outputFilePath));
        // if the key is null or the key length is not equal to KeySizeBytes, throw an ArgumentNullException
        if (key == null || key.Length != KeySizeBytes)
            throw new ArgumentNullException(nameof(key));

        // initialize nonce (or inititalization vector) and tag arrays with NonceSizeBytes and TagSizeBytes
        byte[] nonce = new byte[NonceSizeBytes];
        byte[] tag = new byte[TagSizeBytes];

        // using RNGCryptoServiceProvider to generate a random nonce
        /*
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(nonce);
        }*/
        // RNGCryptoServiceProvider() was apparently obsolete:
        // source Microsoft Docs: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rngcryptoserviceprovider?view=net-8.0
        // Accessed 1 August 2024
        // so had to switch to RandomNumberGenerator
        RandomNumberGenerator.Fill(nonce);

        // read the plaintext file content into a byte array
        byte[] plaintext = File.ReadAllBytes(inputFilePath);
        // initialize ciphertext array with the same length as the plaintext
        byte[] ciphertext = new byte[plaintext.Length];

        // using AesGcm to encrypt the plaintext
        // source Microsoft docs: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesgcm.encrypt?view=net-8.0
        // Accessed 1 August 2024
        using (AesGcm aesGcm = new AesGcm(key))
        {
            // encrypt the plaintext using the nonce, plaintext, ciphertext, and tag
            aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);
        }

        // write the nonce, ciphertext, and tag to the output file
        using (FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
        {
            // writing nonce for uniqueness
            // same plaintext will be different for every encryption
            fsOutput.Write(nonce, 0, nonce.Length);
            // writing ciphertext to the file
            fsOutput.Write(ciphertext, 0, ciphertext.Length);
            // writing authentication tag for integrity check
            fsOutput.Write(tag, 0, tag.Length);
        }
    }

    // public static void DecryptFile method is responsible for decrypting the file
    public static void DecryptFile(string inputFilePath, string outputFilePath, byte[] key)
    {
        // if the inputFilePath is null or empty, throw an ArgumentNullException
        if (string.IsNullOrEmpty(inputFilePath))
            throw new ArgumentNullException(nameof(inputFilePath));
        // if the outputFilePath is null or empty, throw an ArgumentNullException
        if (string.IsNullOrEmpty(outputFilePath))
            throw new ArgumentNullException(nameof(outputFilePath));
        // if the key is null or the key length is not equal to KeySizeBytes, throw an ArgumentNullException
        if (key == null || key.Length != KeySizeBytes)
            throw new ArgumentNullException(nameof(key));

        // read the file content into a byte array
        byte[] fileContent = File.ReadAllBytes(inputFilePath);

        // initialize nonce, tag, and ciphertext arrays
        byte[] nonce = new byte[NonceSizeBytes];
        byte[] tag = new byte[TagSizeBytes];
        // ciphertext length is the file content length minus the nonce and tag size
        byte[] ciphertext = new byte[fileContent.Length - NonceSizeBytes - TagSizeBytes];

        // copy the nonce, tag, and ciphertext from the file content
        Buffer.BlockCopy(fileContent, 0, nonce, 0, NonceSizeBytes);
        Buffer.BlockCopy(fileContent, fileContent.Length - TagSizeBytes, tag, 0, TagSizeBytes);
        Buffer.BlockCopy(fileContent, NonceSizeBytes, ciphertext, 0, ciphertext.Length);

        // initialize plaintext array with the same length as the ciphertext
        byte[] plaintext = new byte[ciphertext.Length];

        // using AesGcm to decrypt the ciphertext
        using (AesGcm aesGcm = new AesGcm(key))
        {
            // decrypt the ciphertext using the nonce, ciphertext, tag, and plaintext
            // which had been written to the encyrpted file
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        }

        // write the plaintext to the output file
        File.WriteAllBytes(outputFilePath, plaintext);
    }
}
