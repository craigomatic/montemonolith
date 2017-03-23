using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteMonolith.Framework.Storage
{
    public class BlobStorageServiceContext<T> : IContext<T> where T : IEntity
    {
        public IEnumerable<T> Entities
        {
            get
            {
                //note this is inefficient, we're just doing this for demonstration purposes :)
                var entities = _CloudBlobClient.GetContainerReference(_ContainerName).ListBlobs();

                var toReturn = new List<T>();

                foreach (var item in entities)
                {
                    var lastSegment = item.Uri.Segments[item.Uri.Segments.Length - 1];
                    var itemId = lastSegment.Substring(0, lastSegment.IndexOf("."));

                    toReturn.Add(this.Read(itemId));
                }

                return toReturn;
            }
        }

        private CloudBlobClient _CloudBlobClient;

        private string _ContainerName;

        public BlobStorageServiceContext(CloudBlobClient cloudBlobClient, string containerName = "monteresults")
        {
            _CloudBlobClient = cloudBlobClient;
            _ContainerName = containerName;

            _CloudBlobClient.GetContainerReference(containerName).CreateIfNotExists();
        }

        public void Create(T result)
        {
            var blobName = $"{Uri.EscapeDataString(result.Id)}.json";
            var json = JsonConvert.SerializeObject(result);
            var container = _CloudBlobClient.GetContainerReference(_ContainerName);
            var blob = container.GetBlockBlobReference(blobName);

            blob.UploadText(json);
        }

        public void Delete(T result)
        {
            var blobName = $"{Uri.EscapeDataString(result.Id)}.json";
            var container = _CloudBlobClient.GetContainerReference(_ContainerName);
            container.GetBlockBlobReference(blobName).DeleteIfExists();
        }

        public T Read(string id)
        {
            var blobName = $"{Uri.EscapeDataString(id)}.json";
            
            var container = _CloudBlobClient.GetContainerReference(_ContainerName);
            var blob = container.GetBlockBlobReference(blobName);
            var json = blob.DownloadText();

            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Update(T result)
        {
            //just replace the file given it's arbitrary json on disk 
            this.Create(result);
        }
    }
}
