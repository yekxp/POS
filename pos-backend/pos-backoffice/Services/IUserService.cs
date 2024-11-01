using JwtManagerHandler.Models;
using pos_backoffice.Models;

namespace pos_backoffice.Services
{
    public interface IUserService
    {
        Task<AuthenticationResponse> Login(AuthenticationRequest authenticationRequest);

        Task<IEnumerable<User>> GetAll();

        Task<User> GetById(string id);

        Task Create(UserRequest model);

        Task Update(string id, UserUpdate model);

        Task Delete(string id);

        Task<string> GetRole(string token);
    }
}
