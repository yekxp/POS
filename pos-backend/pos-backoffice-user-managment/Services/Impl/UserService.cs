using AutoMapper;
using JwtManagerHandler.Models;
using pos_backoffice_user_managment.Models;
using pos_backoffice_user_managment.Models.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace pos_backoffice_user_managment.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<string> GetRole(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null!;

            return await _userRepository.GetRole(token);
        }

        public async Task<AuthenticationResponse> Login(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrWhiteSpace(authenticationRequest.Username) || string.IsNullOrWhiteSpace(authenticationRequest.Password))
                return null!;

            authenticationRequest.Password = GetHashString(authenticationRequest.Password);

            return await _userRepository.Login(authenticationRequest);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User> GetById(string id)
        {
            User user = await _userRepository.GetById(id);

            return user ?? throw new KeyNotFoundException("User not found");
        }

        public async Task Create(UserRequest model)
        {
            if (await _userRepository.GetByEmail(model.Email!) != null)
                throw new Exception("User with the email '" + model.Email + "' already exists");

            User user = _mapper.Map<User>(model);
            user.PasswordHash = GetHashString(model.Password);

            await _userRepository.Create(user);
        }

        public static byte[] GetHash(string inputString)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public async Task Update(string id, UserUpdate model)
        {
            User user = await _userRepository.GetById(id) ?? throw new KeyNotFoundException("User not found");

            bool emailChanged = !string.IsNullOrEmpty(model.Email) && user.Email != model.Email;
            if (emailChanged && await _userRepository.GetByEmail(model.Email!) != null)
                throw new Exception("User with the email '" + model.Email + "' already exists");


            if (!string.IsNullOrEmpty(model.Password))
                user.PasswordHash = GetHashString(model.Password);

            _mapper.Map(model, user);

            await _userRepository.Update(user);
        }

        public async Task Delete(string id)
        {
            await _userRepository.Delete(id);
        }
    }
}
