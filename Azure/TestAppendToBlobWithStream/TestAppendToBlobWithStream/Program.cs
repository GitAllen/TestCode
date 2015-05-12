using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TestAppendToBlobWithStream
{
    class Program
    {
        private const int MaxBlockSize = 4 * 1024 * 1024;
        private static int _offset = 0;
        private static MemoryStream _currentStream = new MemoryStream();
        private static BlockingCollection<MemoryStream> _bufferQueue = new BlockingCollection<MemoryStream>();
        private static CloudBlockBlob _blob;

        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("mycontainer");
            container.CreateIfNotExists();
            _blob = container.GetBlockBlobReference("myblob.txt");
            if (!_blob.Exists())
            {
                using (var s = new MemoryStream())
                {
                    _blob.UploadFromStream(s);
                }
            }

            Task.Factory.StartNew(GenerateContent);
            Task.Factory.StartNew(AppendStreamToBlob);
            Task.WaitAll();

            Console.ReadLine();
        }

        private static void GenerateContent()
        {
            int i = 0;
            while (i < 100)
            {
                i++;
                string line = string.Format("Content of line {0}{1}", i, Environment.NewLine);

                var bytes = Encoding.Default.GetBytes(line);
                AddBufferToQueue(bytes);
            }
        }

        private static void AddBufferToQueue(byte[] bytes)
        {
            if (_offset + bytes.Length > MaxBlockSize)
            {
                var ms = new MemoryStream();
                _currentStream.Position = 0;
                _currentStream.CopyTo(ms);
                _bufferQueue.Add(ms);
                Console.WriteLine("New buffer added to queue successfully");
                _currentStream.SetLength(0);
                _offset = 0;
            }

            _currentStream.Write(bytes, 0, bytes.Length);
            _offset += bytes.Length;

        }

        private static void AppendStreamToBlob()
        {
            foreach (var stream in _bufferQueue.GetConsumingEnumerable())
            {
                var blockIdList = new List<string>();
                blockIdList.AddRange(_blob.DownloadBlockList().Select(b => b.Name));

                var newBlockId = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace('+', 'A').Replace('\\', 'B').Replace('/', 'C');
                stream.Position = 0;
                _blob.PutBlock(newBlockId, stream, null);
                blockIdList.Add(newBlockId);
                _blob.PutBlockList(blockIdList);
                stream.Close();

                Console.WriteLine("Block {0} is appended successfully", newBlockId);
            }
        }
    }
}
