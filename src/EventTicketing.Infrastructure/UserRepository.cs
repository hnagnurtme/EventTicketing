using EventTicketing.Application.Services.Authentication.Commands.Register;

namespace EventTicketing.Infrastructure;

using EventTicketing.Application.Common.Interface.Persistence;
using EventTicketing.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

public class UserRepository : IUserRepository
{
    // Dummy in-memory store
    private readonly List<User> _users = new();

    public User? GetUserByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email == email);
    }

    public void AddUser(User user)
    {
        _users.Add(user);
    }
}
