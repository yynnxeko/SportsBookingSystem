using AutoMapper;
using SportsBookingSystem.Application.DTOs.UserDtos;
using SportsBookingSystem.Application.Helpers;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SportsBookingSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserDto> CreateAsync(UserCreatedDto user)
        {
            var existingUser = await _userRepository.IsEmailExistedAsync(user.Email);
            if(existingUser) 
                throw new ArgumentException("Email already exists");

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = user.FullName,
                Email = user.Email,
                PasswordHash = AuthHelper.HashPassword(user.PasswordHash),
                Role = user.Role,
                WalletBalance = 0,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.CreateAsync(newUser);
            if (newUser == null)
                throw new ArgumentException("Failed to create user");

            return _mapper.Map<UserDto>(newUser);
        }
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var listUser = await _userRepository.GetAllAsync();
            return _mapper.Map<List<UserDto>>(listUser);
        }
        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !AuthHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = AuthHelper.GenerateJwtToken(user, _configuration);
            return token;
        }
    }
}
