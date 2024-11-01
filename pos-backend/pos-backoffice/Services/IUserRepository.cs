using JwtManagerHandler.Models;
using pos_backoffice.Models;

namespace pos_backoffice.Services
{
    public interface IUserRepository
    {
        Task<AuthenticationResponse> Login(AuthenticationRequest authenticationRequest);

        Task<IEnumerable<User>> GetAll();

        Task<User> GetById(string id);

        Task<User> GetByEmail(string email);

        Task Create(User user);

        Task Update(User user);

        Task Delete(string id);

        Task<string> GetRole(string token);
    }
}
