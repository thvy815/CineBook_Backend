using AuthService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AuthService.Domain.DTOs;

namespace AuthService.Application.Client
{
	public class UserProfileClient : IUserProfileClient
	{
		private readonly HttpClient _httpClient;

		public UserProfileClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task CreateUserProfileAsync(Guid userId, UserProfileCreateDTO dto)
		{
            var response = await _httpClient.PostAsJsonAsync($"api/UserProfile/by-user/{userId}", dto);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();

                throw new Exception(
                    $"UserProfileService error {(int)response.StatusCode}: {errorBody}"
                );
            }
        }
	}
}
