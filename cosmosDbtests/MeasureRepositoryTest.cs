using System.Linq;
using System.Threading.Tasks;
using NFluent;
using Xunit;

namespace cosmosDbtests
{
    public class MeasureRepositoryTest : IClassFixture<CosmosDbFixture>
    {
        private readonly CosmosDbFixture _fixture;

        public MeasureRepositoryTest(CosmosDbFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetMeasuresTest()
        {
            var expectedMeasures = new[] {
                new Measure{ Code = "I", Name = "Current", Unit = "A" },
                new Measure{ Code = "U", Name = "Voltage", Unit = "V" }
            };

            var subject = new MeasureRepository(
                _fixture.DocumentClient,
                _fixture.DocumentCollectionUri);

            var actual = (await subject.GetMeasures()).ToArray();
            Check.That(actual).HasSize(expectedMeasures.Length);
            for (int i = 0; i < expectedMeasures.Length; i++)
            {
                Check.That(actual[i]).HasFieldsWithSameValues(expectedMeasures[i]);
            }
        }
    }
}