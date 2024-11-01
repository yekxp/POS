using JwtManagerHandler;
using JwtManagerHandler.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using pos_backoffice.Database;
using pos_backoffice.Models;

namespace pos_backoffice.Services.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(IOptions<MongoDBSettings> settings, IMongoClient mongoClient, JwtTokenHandler jwtTokenHandler)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _usersCollection = database.GetCollection<User>(settings.Value.UsersCollectionName);
            _jwtTokenHandler = jwtTokenHandler;
        }

        public async Task<AuthenticationResponse> Login(AuthenticationRequest authenticationRequest)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Username, authenticationRequest.Username),
                Builders<User>.Filter.Eq(u => u.PasswordHash, authenticationRequest.Password)
            );

            User user = await _usersCollection.Find(filter).FirstOrDefaultAsync();
            if (user is null)
                return null!;

            authenticationRequest.Role = (JwtManagerHandler.Models.Role)user.Role;
            return _jwtTokenHandler.GenerateJwtToken(authenticationRequest)!;
        }

        public Task<string> GetRole(string token)
        {
            if (string.IsNullOrEmpty(token))
                return Task.FromResult(token);

            return Task.FromResult(_jwtTokenHandler.GetRoleFromJwtToken(token)!);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _usersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<User> GetById(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _usersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await _usersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Create(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task Update(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<User>.Update
                .Set(u => u.Username, user.Username)
                .Set(u => u.Name, user.Name)
                .Set(u => u.Surname, user.Surname)
                .Set(u => u.Email, user.Email)
                .Set(u => u.Role, user.Role)
                .Set(u => u.PasswordHash, user.PasswordHash);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task Delete(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await _usersCollection.DeleteOneAsync(filter);
        }
    }
}
