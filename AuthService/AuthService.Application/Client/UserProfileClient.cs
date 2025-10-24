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
			var json = System.Text.Json.JsonSerializer.Serialize(dto);

			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await _httpClient.PostAsync($"api/UserProfile/by-user/{userId}", content);

			response.EnsureSuccessStatusCode(); // throw exception if failed
		}
	}
}
