using System.Net;
using System.Net.Http.Json;
using MedicalClinicSystem.Application.DTOs.Identity;
using Xunit;

namespace MedicalClinicSystem.Tests
{
    public class AuthFlowTests : IClassFixture<TestAppFactory>
    {
        private readonly HttpClient _client;

        public AuthFlowTests(TestAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_Returns_Tokens()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                UserNameOrEmail = "admin",
                Password = "Admin@123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var payload = await response.Content.ReadFromJsonAsync<ApiEnvelope<LoginResponseDto>>();
            Assert.NotNull(payload);
            Assert.True(payload!.Success);
            Assert.False(string.IsNullOrWhiteSpace(payload.Data?.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(payload.Data?.RefreshToken));
        }

        [Fact]
        public async Task Refresh_Rotates_Token()
        {
            var login = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                UserNameOrEmail = "admin",
                Password = "Admin@123"
            });

            var loginPayload = await login.Content.ReadFromJsonAsync<ApiEnvelope<LoginResponseDto>>();
            var refreshToken = loginPayload!.Data!.RefreshToken;

            var refresh = await _client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequestDto
            {
                RefreshToken = refreshToken
            });

            Assert.Equal(HttpStatusCode.OK, refresh.StatusCode);

            var refreshPayload = await refresh.Content.ReadFromJsonAsync<ApiEnvelope<LoginResponseDto>>();
            Assert.NotNull(refreshPayload);
            Assert.True(refreshPayload!.Success);
            Assert.NotEqual(refreshToken, refreshPayload.Data!.RefreshToken);
        }
    }

    public class ApiEnvelope<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}

