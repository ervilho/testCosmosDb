using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace cosmosDbtests
{
    public class MeasureRepository {
        private readonly DocumentClient _documentClient;
        private readonly Uri _documentCollectionUri;

        public MeasureRepository(DocumentClient documentClient, Uri documentCollectionUri)
        {
            _documentClient = documentClient;
            _documentCollectionUri = documentCollectionUri;
        }

        public async Task<IEnumerable<Measure>> GetMeasures()
        {
            var query = _documentClient.CreateDocumentQuery(_documentCollectionUri, "SELECT * FROM c WHERE c.pk = 'measure' ORDER BY c.Code");
            var measures = await query.AsDocumentQuery().ExecuteNextAsync<Measure>();
            return measures;
        }
    }
}