using EventTicketing.Domain.Entities;

namespace EventTicketing.Application.Services.Authentication.Commands.Register;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    void AddUser(User user);
}
