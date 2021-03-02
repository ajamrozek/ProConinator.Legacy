using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using ProConinator.Web.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.WindowsAzure.Storage;

namespace ProConinator.Web.Data
{
    public class ProConinatorRepository : IProConinatorRepository
    {
        private CloudTable table;
        private CloudBlobContainer container;

        public ProConinatorRepository() { }
        public ProConinatorRepository(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient();
            this.table = tableClient.GetTableReference("ProConinator");
            this.table.CreateIfNotExists();

            var blobClient = storageAccount.CreateCloudBlobClient();
            this.container = blobClient.GetContainerReference("proconinator");
            this.container.CreateIfNotExists();

        }

        public ProConGroupModel Save(ProConGroupModel proConGroupModel)
        {
            proConGroupModel.Id = proConGroupModel.Id == Guid.Empty ? Guid.NewGuid() : proConGroupModel.Id;

            var document = JsonConvert.SerializeObject(proConGroupModel,
                Newtonsoft.Json.Formatting.Indented);

            var partitionKey = proConGroupModel.UserId;
            var rowKey = proConGroupModel.Id.ToString();

            UploadDocument(partitionKey, rowKey, document);

            return proConGroupModel;
        }

        public ProConGroupModel Load(string partitionKey, string rowKey)
        {
            var blobName = string.Format(@"procongroup\{0}\{1}.json", partitionKey, rowKey);
            var document = this.DownloadDocument(blobName);
            return JsonConvert.DeserializeObject<ProConGroupModel>(document);
        }

        public IEnumerable<ProConGroupModel> List(string partitionKey)
        {
            var listItems = this.container
                .GetDirectoryReference("procongroup/" + partitionKey).ListBlobs();

            IEnumerable<ProConGroupModel> result = listItems.OfType<CloudBlockBlob>()
                .Select(x => this.DownloadDocument(x.Name))
                .Select((document) => { 
                    var proConGroup = JsonConvert.DeserializeObject<ProConGroupModel>(document);
                    proConGroup.ProConLists = AddIdToProConItems(proConGroup.ProConLists.OrderByDescending(x => x.UpdatedDate == DateTime.MinValue ? x.CreatedDate : x.UpdatedDate ));
                    return proConGroup;
                });
            
            return result;
        }

        /// <summary>
        /// A retro fit function to add an Id to all ProCons so that a Delete operation can precisely identify the item to be deleted.
        /// </summary>
        /// <param name="proConLists"></param>
        /// <returns></returns>
        private IEnumerable<ProConListModel> AddIdToProConItems(IEnumerable<ProConListModel> proConLists)
        {
            foreach (var proConList in proConLists)
            {
                proConList.ProCons = AddIdToProConItems(proConList.ProCons);
            }
            return proConLists;
        }

        private IEnumerable<ProConItemModel> AddIdToProConItems(IEnumerable<ProConItemModel> proConItems)
        {         
            int id = 0;
            return proConItems.Select((proConItem) =>
                {
                    proConItem.Id = ++id;
                    return proConItem;                    
                });
        }

        private string DownloadDocument(string blobName)
        {
            var blockBlob = this.container.GetBlockBlobReference(blobName);

            using (var memory = new MemoryStream())
            using (var reader = new StreamReader(memory))
            {
                blockBlob.DownloadToStream(memory);
                memory.Seek(0, SeekOrigin.Begin);

                return reader.ReadToEnd();
            }
        }

        private void UploadDocument(string partitionKey, string rowKey, string document)
        {
            var filename = string.Format(@"procongroup\{0}\{1}.json", partitionKey, rowKey);
            var blockBlob = this.container.GetBlockBlobReference(filename);

            using (var memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(document);
                writer.Flush();
                memory.Seek(0, SeekOrigin.Begin);

                blockBlob.UploadFromStream(memory);
            }

            blockBlob.Properties.ContentType = "application/json";
            blockBlob.SetProperties();
        }

        public void DeleteDocument(string partitionKey, string rowKey)
        {
            var blobName = string.Format(@"procongroup\{0}\{1}.json", partitionKey, rowKey); 
            var blockBlob = this.container.GetBlockBlobReference(blobName); 
            blockBlob.Delete(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}