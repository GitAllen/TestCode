using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TestAppendToBlobWithStream
{
    public class BlobAppender
    {
        private const int MaxBlockSize = 4 * 1024 * 1024;
        private const int MaxBufferTimeInSecond = 5;
        private string _storageConnectionString;
        private int _bufferOffset = 0;
        private DateTime _lastBufferTime = DateTime.Now;
        private MemoryStream _currentStream = new MemoryStream();
        private BlockingCollection<MemoryStream> _bufferQueue = new BlockingCollection<MemoryStream>();
        private CloudBlockBlob _blob;

        public BlobAppender(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        public void Run()
        {
            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
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

            Task.Factory.StartNew(() => GenerateContentAsync());
            Task.Factory.StartNew(() => AppendStreamToBlobAsync());
            Task.WaitAll();
        }


        private async Task GenerateContentAsync()
        {
            int i = 0;
            while (i < 10000000)
            {
                i++;
                string line = string.Format("Content of line {0}{1}", i, Environment.NewLine);

                var bytes = Encoding.Default.GetBytes(line);
                await AddBufferToQueueAsync(bytes);
                await Task.Delay(100);
            }
        }

        private async Task AddBufferToQueueAsync(byte[] bytes)
        {
            if (_bufferOffset + bytes.Length > MaxBlockSize || (DateTime.Now - _lastBufferTime).TotalSeconds > MaxBufferTimeInSecond)
            {
                var ms = new MemoryStream();
                _currentStream.Position = 0;
                await _currentStream.CopyToAsync(ms);
                _bufferQueue.Add(ms);
                Console.WriteLine("New buffer added to queue successfully");
                _currentStream.SetLength(0);
                _bufferOffset = 0;
                _lastBufferTime = DateTime.Now;
            }

            await _currentStream.WriteAsync(bytes, 0, bytes.Length);
            _bufferOffset += bytes.Length;
        }

        private async Task AppendStreamToBlobAsync()
        {
            foreach (var stream in _bufferQueue.GetConsumingEnumerable())
            {
                var blockIdList = new List<string>();
                blockIdList.AddRange(_blob.DownloadBlockList().Select(b => b.Name));

                var newBlockId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                stream.Position = 0;
                await _blob.PutBlockAsync(newBlockId, stream, null);
                blockIdList.Add(newBlockId);
                await _blob.PutBlockListAsync(blockIdList);
                stream.Close();

                Console.WriteLine("Block {0} is appended successfully", newBlockId);
            }
        }
    }
}