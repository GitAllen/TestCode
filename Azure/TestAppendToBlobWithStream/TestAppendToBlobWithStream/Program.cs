using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TestAppendToBlobWithStream
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("mycontainer");
            container.CreateIfNotExists();

            var blob = container.GetBlockBlobReference("myblob");
            blob.StreamMinimumReadSizeInBytes = 1024 * 1024;


            using (var buffer = new MemoryStream())
            {
                int i = 0;
                while (i < 1000)
                {
                    i++;
                    string line = string.Format("Will be append to line {0}{1}", i, Environment.NewLine);

                    var bytes = Encoding.Default.GetBytes(line);
                    buffer.Write(bytes, 0, bytes.Length);
                    if (buffer.Length > 1024)
                    {
                        AppendToBlob(blob, buffer);
                    }
                }
            }
        }

        static void AppendToBlob(CloudBlockBlob blob, MemoryStream stream)
        {
            List<string> blockIds = new List<string>();
            try
            {
                blockIds.AddRange(blob.DownloadBlockList().Select(b => b.Name));
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode != (int)HttpStatusCode.NotFound)
                {
                    throw;
                }
                Console.WriteLine("Blob does not yet exist. Creating...");
                blob.Container.CreateIfNotExists();
            }

            var newId = Convert.ToBase64String(Encoding.Default.GetBytes(blockIds.Count.ToString()));
            blob.PutBlock(newId, stream, null);
            blockIds.Add(newId);
            blob.PutBlockList(blockIds);

            Console.WriteLine("New contents after buffer flush:");
            Console.WriteLine(blob.DownloadText());
        }
    }
}
