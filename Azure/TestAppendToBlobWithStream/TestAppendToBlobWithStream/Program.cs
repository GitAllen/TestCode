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


        static void Main(string[] args)
        {
            string storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var blobAppender = new BlobAppender(storageConnectionString);
            blobAppender.Run();

            Console.ReadLine();
        }


    }
}
