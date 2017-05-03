﻿using IntoTravel.Data.BaseClasses;
using IntoTravel.Data.Models.AzureStorage.Blob;
using IntoTravel.Data.Repositories.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IntoTravel.Data.Repositories.Implementations
{
    public class SiteFilesRepository : BaseBlobFiles, ISiteFilesRepository
    {
        const string FolderFileName = "_.txt";
        const string ContainerName = "sitecontent";
        private readonly string _connectionString;
        private readonly CloudStorageAccount _storageAccount;

        public SiteFilesRepository(string connectionString)
        {
            _connectionString = connectionString;
            _storageAccount = CloudStorageAccount.Parse(_connectionString);

            var blobClient = _storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(ContainerName);

            // todo: run async elsewhere
            Task.Run(async () => {
                await CreateIfNotExists(container);
                await SetCorsAsync(blobClient);
            });
        }

        private async Task CreateIfNotExists(CloudBlobContainer container)
        {
            if (await container.CreateIfNotExistsAsync())
            {
                await SetPublicContainerPermissionsAsync(container);
            }
        }

        public SiteFileDirectory ListFiles(string prefix = null)
        {
            try
            {
                var directory = new SiteFileDirectory();
                var blobClient = _storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(ContainerName);

                if (prefix != null && prefix.StartsWith("/"))
                {
                    prefix = prefix.Remove(0, 1);
                }

                foreach (IListBlobItem item in container.ListBlobsSegmentedAsync(
                                                            prefix, false, BlobListingDetails.None, int.MaxValue, null, null, null).Result.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        var blob = (CloudBlockBlob)item;
                        var file = blob.Uri.ToString();

                        if (!file.EndsWith(FolderFileName))
                        {
                            directory.FileItems.Add(new SiteFileItem
                            {
                                FilePath = blob.Uri.ToString(),
                                IsFolder = false
                            });
                        }
                        else if (item.Parent.Prefix != prefix)
                        {
                            AddDirectory(directory, item.Parent);
                        }
                    }
                    else if (item.GetType() == typeof(CloudPageBlob))
                    {
                        var pageBlob = (CloudPageBlob)item;

                        // todo: find out when this is used
                        throw new Exception("CloudPageBlob");
                    }
                    else if (item.GetType() == typeof(CloudBlobDirectory))
                    {
                        AddDirectory(directory, item);
                    }
                }

                return directory;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private static void AddDirectory(SiteFileDirectory directory, IListBlobItem item)
        {
            var cloudBlobDirectory = (CloudBlobDirectory)item;
            var folderName = cloudBlobDirectory.Uri.ToString().Split('/')[cloudBlobDirectory.Uri.ToString().Split('/').Length - 2];
            var pathFromRoot = new Uri(cloudBlobDirectory.Uri.ToString()).LocalPath.Replace(string.Format("/{0}", ContainerName), string.Empty);

            directory.FileItems.Add(new SiteFileItem
            {
                FilePath = cloudBlobDirectory.Uri.ToString(),
                IsFolder = true,
                FolderName = folderName,
                FolderPathFromRoot = pathFromRoot
            });
        }

        public async Task DeleteFileAsync(string blobPath)
        {
            try
            {
                var blobClient = _storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(ContainerName);

                if (IsFolderPath(blobPath))
                {
                    blobPath = string.Format("{0}{1}", blobPath, FolderFileName);

                    if (blobPath.StartsWith("/"))
                        blobPath = blobPath.Remove(0, 1);
                }

                var endIndexOfCotainerName = blobPath.IndexOf(ContainerName) + ContainerName.Length;

                var path = blobPath.Substring(endIndexOfCotainerName, blobPath.Length - endIndexOfCotainerName).TrimStart('/');

                var blockBlob = await container.GetBlobReferenceFromServerAsync(path);

                await blockBlob.DeleteAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private List<IListBlobItem> GetDirContents(string prefix = null)
        {
            var directory = new SiteFileDirectory();
            var blobClient = _storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(ContainerName);

            if (prefix != null && prefix.StartsWith("/"))
            {
                prefix = prefix.Remove(0, 1);
            }

            
            var allInDir = container.ListBlobsSegmentedAsync(prefix, true, BlobListingDetails.None, int.MaxValue, null, null, null).Result.Results;

            return allInDir.ToList();
        }

        public async Task DeleteFolderAsync(string folderPath)
        {
            var allInDir = GetDirContents(folderPath);

            foreach (var item in allInDir)
            {
                await DeleteFileAsync(item.Uri.ToString());
            }
        }
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public async Task UploadAsync(IFormFile file, string directory = null)
        {
            try
            {
                var fileName = CleanFileName(file.FileName);

                if (fileName == FolderFileName)
                    return;

                var filePath = fileName;

                if (!string.IsNullOrWhiteSpace(directory))
                {
                    filePath = directory + filePath;

                    if (filePath.StartsWith("/"))
                    {
                        filePath = filePath.Remove(0, 1);
                    }
                }

                var blobClient = _storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(ContainerName);
                var blockBlob = container.GetBlockBlobReference(filePath);

                var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                await blockBlob.UploadFromStreamAsync(memoryStream);

                var extension = Path.GetExtension(file.FileName).ToLower().Replace(".", string.Empty);

                await SetPropertiesAsync(blockBlob, extension);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        public async Task CreateFolderAsync(string folderPath, string directory = null)
        {
            try
            {
                folderPath = folderPath.Replace("/", string.Empty);

                var memoryStream = new MemoryStream();
                memoryStream.Seek(0, SeekOrigin.Begin);
                TextWriter tw = new StreamWriter(memoryStream);

                tw.WriteLine(folderPath);

                var blobClient = _storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(ContainerName);
                var path = string.Format("{0}/{1}", folderPath, FolderFileName);

                if (!string.IsNullOrWhiteSpace(directory))
                {
                    path = directory + path;

                    if (path.StartsWith("/"))
                        path = path.Remove(0, 1);
                }

                var blockBlob = container.GetBlockBlobReference(path);
                await blockBlob.UploadFromStreamAsync(memoryStream);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private static bool IsFolderPath(string blobPath)
        {
            return blobPath.EndsWith("/");
        }


    }
}
