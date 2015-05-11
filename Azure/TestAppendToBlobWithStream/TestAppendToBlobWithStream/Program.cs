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
        private const int bufferSize = 1024 * 1024;

        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("mycontainer");
            container.CreateIfNotExists();

            var blob = container.GetBlockBlobReference("myblob.txt");
            blob.StreamMinimumReadSizeInBytes = 1024 * 1024;

            using (var buffer = new MemoryStream())
            {
                int i = 0;
                while (i < 1000000)
                {
                    i++;
                    string line = string.Format("Content of line {0}{1}", i, Environment.NewLine);

                    var bytes = Encoding.Default.GetBytes(line);
                    buffer.Write(bytes, 0, bytes.Length);
                    if (buffer.Length > bufferSize)
                    {
                        Console.WriteLine("line={0}", line);
                        AppendToBlob(blob, buffer);
                    }
                }
            }
        }

        static void AppendToBlob(CloudBlockBlob blob, MemoryStream stream)
        {
            var blockIdList = new List<string>();
            try
            {
                blockIdList.AddRange(blob.DownloadBlockList().Select(b => b.Name));
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode != (int)HttpStatusCode.NotFound)
                {
                    throw;
                }
                Console.WriteLine("Blob does not yet exist. Creating...");
                using (var s = new MemoryStream())
                {
                    blob.UploadFromStream(s);
                }
            }

            var newBlockId = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace('+', 'A').Replace('\\', 'B').Replace('/', 'C');
            stream.Position = 0;
            blob.PutBlock(newBlockId, stream, null);
            blockIdList.Add(newBlockId);
            blob.PutBlockList(blockIdList);

            stream.Position = 0;
            stream.SetLength(0);

            //Console.WriteLine("New contents after buffer flush:");
            //Console.WriteLine(blob.DownloadText());

            Console.WriteLine("Append completed successfully");
        }
    }
}
