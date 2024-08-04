using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pq_file_storage_project.Features.Backup
{
    // AwsS3Utility class is responsible for handling the AWS S3 upload logic for backup functionality
    public class AwsS3Utility
    {
        // Initialize private readonly AmazonS3Client awsS3Client
        private readonly AmazonS3Client awsS3Client;

        // Initialize public AwsS3Utility constructor
        public AwsS3Utility()
        {
            // Initialize configuration with appsettings.json
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            // Retrieve AWS Region, Access Key, and Secret Key from appsettings.json
            var awsRegionString = configuration["AWS:Region-endpoint"];
            var accessKey = configuration["AWS:Aws_access_key_id"];
            var secretKey = configuration["AWS:Aws_secret_access_key"];

            // If AWS Region, Access Key, or Secret Key is null or empty
            if (string.IsNullOrEmpty(awsRegionString) || string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                // throw an exception if AWS Region, Access Key, or Secret Key is not configured properly
                throw new Exception("AWS Region, Access Key or Secret Key is not configured properly.");
            }

            // Retrieve AWS Region from the AWS Region string
            var awsRegion = Amazon.RegionEndpoint.GetBySystemName(awsRegionString);

            // Initialize AmazonS3Config with the AWS Region and ensure HTTPS is used
            var s3Config = new AmazonS3Config
            {
                // Set the AWS Region
                RegionEndpoint = awsRegion,
                // Ensure HTTPS is used
                UseHttp = false
            };
            // Initialize awsS3Client with the Access Key, Secret Key, and S3 Config
            awsS3Client = new AmazonS3Client(accessKey, secretKey, s3Config);
        }

        // public virtual async method UploadFileAsync is responsible for uploading the file to AWS S3 and called in UserSpaceViewModel
        public virtual async Task UploadFileAsync(string s3BucketName, string filePath, string fileName)
        {
            // initialize configuration with appsettings.json
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            // Retrieve the file location environment variable for Key parameter to be used with AWS S3 PutObjectRequest from appsettings.json
            string fileLocationInS3 = configuration["AWS:Key"];
            // combine the file path with the file name to set the file location in S3 with file name
            string fileLocationInS3WithFileName = fileLocationInS3 + fileName;

            // attempt to upload the file to S3
            try
            {
                // initialize PutObjectRequest with the S3 bucket Name, File path, and key in S3
                PutObjectRequest putObjectRequest = new PutObjectRequest
                {
                    BucketName = s3BucketName,
                    FilePath = filePath,
                    Key = fileLocationInS3WithFileName
                };

                // await the PutObjectAsync method to upload the file to S3
                PutObjectResponse response = await awsS3Client.PutObjectAsync(putObjectRequest);
            }
            // catch AmazonS3Excetion and throw an exception with the error message
            catch (AmazonS3Exception e)
            {
                throw new Exception($"Error uploading file to S3: {e.Message}");
            }
            // catch any other exception and throw and exception with the unknown error message
            catch (Exception e)
            {
                throw new Exception($"Unknown error: {e.Message}");
            }
        }
    }
}
