using System.Net;
using Xunit;

namespace MedicalClinicSystem.Tests
{
    public class HealthTests : IClassFixture<TestAppFactory>
    {
        private readonly HttpClient _client;

        public HealthTests(TestAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Health_Returns_200()
        {
            var response = await _client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

