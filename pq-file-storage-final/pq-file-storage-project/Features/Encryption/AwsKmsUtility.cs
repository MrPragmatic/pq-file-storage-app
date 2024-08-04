using Amazon.KeyManagementService.Model;
using Amazon.KeyManagementService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Amazon;
using Amazon.Runtime;

namespace pq_file_storage_project.Features.Encryption
{
    // AwsKmsUtility utility class is responsible for generating and decrypting data keys using AWS KMS service
    // as part of envelope encryption
    public class AwsKmsUtility
    {
        // private static CreateKmsClient method is used to create an instance of AmazonKeyManagementServiceClient
        private static AmazonKeyManagementServiceClient CreateKmsClient()
        {
            // Load the configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Get the AWS Region, Access Key and Secret Key from the configuration
            var awsRegionString = configuration["AWS:Region-endpoint"];
            var accessKey = configuration["AWS:Aws_access_key_id"];
            var secretKey = configuration["AWS:Aws_secret_access_key"];

            // Check if the AWS Region, Access key and secret key are configured properly
            if (string.IsNullOrEmpty(awsRegionString) || string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                // throw an exception if the AWS Region, access key or secret key is not configured properly
                throw new Exception("AWS Region, Access Key or Secret Key is not configured properly.");
            }

            // create an instance of RegionEndpoint using the relevant AWS Region
            var awsRegion = RegionEndpoint.GetBySystemName(awsRegionString);

            // create an instance of BasicAWSCredentials using the access key and secret Key
            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            // create an instance of AmazonKeyManagementServiceConfig using the AWS Region
            var kmsClientConfig = new AmazonKeyManagementServiceConfig
            {
                // Set the AWS Region
                RegionEndpoint = awsRegion,
                // Ensure HTTPS is used
                UseHttp = false,
            };
            // return an instance of AmazonKeyManagementServiceClient using the credentials and kmsClientConfig
            return new AmazonKeyManagementServiceClient(credentials, kmsClientConfig);
        }

        // GenerateDataKeyAsync method is used to generate a data key using the specified AWS KMS keyId and encryptionContext
        public static async Task<(byte[], string)> GenerateDataKeyAsync(string keyId, string encryptionContext)
        {
            try
            {
                // create an instance of AmazonKeyManagementServiceClient
                using (var kmsClient = CreateKmsClient())
                {
                    // create an instance of GenerateDataKeyRequest using the keyId, keySpec and encryptionContext
                    var generateDataKeyRequest = new GenerateDataKeyRequest
                    {
                        KeyId = keyId,
                        KeySpec = "AES_256",
                        EncryptionContext = new Dictionary<string, string>
                        {
                            { "FileName", encryptionContext }
                        }
                    };

                    // generate a data key using the specified keyId, keySpec and encryptionContext
                    var generateDataKeyResponse = await kmsClient.GenerateDataKeyAsync(generateDataKeyRequest);

                    // Return the plaintext key and the encrypted key blob
                    return (generateDataKeyResponse.Plaintext.ToArray(), Convert.ToBase64String(generateDataKeyResponse.CiphertextBlob.ToArray()));
                }
            }
            // catch possible Amazon key management service exception
            catch (AmazonKeyManagementServiceException e)
            {
                // throw an exception with the error message
                throw new Exception($"Error generating data key: {e.Message}");
            }
            // catch any other exception
            catch (Exception e)
            {
                // throw an exception with the error message
                throw new Exception($"Unknown error occurred: {e.Message}");
            }
        }

        // DecryptDataKeyAsync method is used to decrypt the encrypted key using the specified encryptionContext
        public static async Task<byte[]> DecryptDataKeyAsync(string encryptedKeyBase64, string encryptionContext)
        {
            try
            {
                // convert the encrypted key from base64 to byte array
                byte[] encryptedKey = Convert.FromBase64String(encryptedKeyBase64);

                // create an instance of AmazonKeyManagementServiceClient
                using (var kmsClient = CreateKmsClient())
                {
                    // Load the configuration settings from the appsettings.json file
                    var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                    // Get the KMS key ID from the configuration settings
                    var kmsKeyId = configuration["AWS:Kms-key-id"];

                    // create an instance of DecryptRequest using the encrypted key and encryptionContext
                    var decryptRequest = new DecryptRequest
                    {
                        CiphertextBlob = new MemoryStream(encryptedKey),
                        KeyId = kmsKeyId,
                        EncryptionContext = new Dictionary<string, string>
                        {
                            { "FileName", encryptionContext }
                        }
                    };

                    // decrypt the encrypted key using the specified encryptionContext
                    var decryptResponse = await kmsClient.DecryptAsync(decryptRequest);
                    // return the decrypted key as byte array
                    return decryptResponse.Plaintext.ToArray();
                }
            }
            // catch possible Amazon key management service exception
            catch (AmazonKeyManagementServiceException e)
            {
                // throw an exception with the error message
                throw new Exception($"Error retrieving keys from KMS: {e.Message}");
            }
            // catch any other exception
            catch (Exception e)
            {
                // throw an exception with the error message
                throw new Exception($"Unknown error occurred: {e.Message}");
            }
        }
    }
}
