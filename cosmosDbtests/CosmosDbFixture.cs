using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFluent;

namespace cosmosDbtests
{
    public class CosmosDbFixture : IDisposable
    {
        private readonly string _database;
        private readonly string _collection;

        public CosmosDbFixture()
        {
            _database = "test";
            _collection = $"test_{Guid.NewGuid()}";
            DocumentCollectionUri = UriFactory.CreateDocumentCollectionUri(_database, _collection);

            DocumentClient = GetDocumentClient();
            CreateDatabaseAsync().Wait();
            CreateCollectionAsync().Wait();
            PopulateCollectionAsync().Wait();
        }

        public DocumentClient DocumentClient { get; }
        public Uri DocumentCollectionUri { get; }

        public void Dispose()
        {
            DeleteCollectionAsync().Wait();
            DocumentClient.Dispose();
        }

        private DocumentClient GetDocumentClient()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            return new DocumentClient(new Uri(configuration["cosmosDbEndpoint"]), configuration["cosmosDbAuthKey"]);
        }

        private async Task CreateDatabaseAsync()
        {
            var database = new Database { Id = _database };
            var response = await DocumentClient.CreateDatabaseIfNotExistsAsync(database);
            Check.That(new[] { response.StatusCode }).IsOnlyMadeOf(HttpStatusCode.OK, HttpStatusCode.Created);
        }

        private async Task CreateCollectionAsync()
        {
            var databaseUri = UriFactory.CreateDatabaseUri(_database);

            var pks = new Collection<string>(new[] { "/pk" });
            var partitionKeyDefinition = new PartitionKeyDefinition() { Paths = pks };
            var indexingPolicy = new IndexingPolicy(new Index[]
            {
                new RangeIndex(DataType.Number, -1),
                new RangeIndex(DataType.String, -1),
            });
            var collection = new DocumentCollection() { Id = _collection, PartitionKey = partitionKeyDefinition, IndexingPolicy = indexingPolicy };
            var options = new RequestOptions() { OfferThroughput = 400 };
            var response = await DocumentClient.CreateDocumentCollectionAsync(databaseUri, collection, options);
            var code = response.StatusCode;
            Check.That(code).IsEqualTo(HttpStatusCode.Created);
        }

        private async Task PopulateCollectionAsync()
        {
            var documents = ReadJsonFile();

            foreach (JObject document in documents)
            {
                var options = new RequestOptions() { PartitionKey = new PartitionKey((string)document.GetValue("pk")) };
                await DocumentClient.CreateDocumentAsync(DocumentCollectionUri, document, options);
            }
        }

        private async Task DeleteCollectionAsync()
        {
            var response = await DocumentClient.DeleteDocumentCollectionAsync(DocumentCollectionUri);
            Check.That(response.StatusCode).Equals(HttpStatusCode.NoContent);
        }

        private JArray ReadJsonFile()
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader("./measures.json"))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    return (JArray)serializer.Deserialize(jsonTextReader);
                }
            }
        }
    }
}