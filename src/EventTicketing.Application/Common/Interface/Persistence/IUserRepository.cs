using EventTicketing.Domain.Entities;
namespace EventTicketing.Application.Common.Interface.Persistence;

public interface IUserRepository
{
    User? GetUserByEmail(string email);
    void AddUser(User user);
}