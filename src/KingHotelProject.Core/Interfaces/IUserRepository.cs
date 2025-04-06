﻿using KingHotelProject.Core.Entities;

namespace KingHotelProject.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<User> GetByUserNameAsync(string userName);
        Task<User> GetByEmailAsync(string email);
    }
}