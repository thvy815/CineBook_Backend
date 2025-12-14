using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.DTOs
{
    public class UserListResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public static UserListResponse FromEntity(User user)
        {
            return new UserListResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role?.Name,
                Status = user.Status,
                CreatedAt = user.CreatedAt
            };
        }
    }

    public class PagedResponse<T>
    {
        public List<T> Data { get; set; } = new();
        public int Page { get; set; }
        public int Size { get; set; }
        public long TotalElements { get; set; }
        public int TotalPages { get; set; }
    }
}
