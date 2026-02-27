using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.DTOs.UserDtos
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;    

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public decimal WalletBalance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
