using MongoDB.Bson;
using MongoDB.Driver;
using SocialAI.Service.Models;

namespace SocialAI.Service.Services
{
    public class AdminService
    {
        private readonly IMongoCollection<User> _users;

        public AdminService(IMongoClient client)
        {
            var database = client.GetDatabase("YoutubeMernSocial");
            _users = database.GetCollection<User>("users");
        }

        // ======================================================
        // GET USERS (SEARCH + PAGINATION)
        // ======================================================
        public async Task<(List<AdminUserDto> items, long totalCount)>
            GetUsersAsync(string? search, int page, int pageSize)
        {
            var filter = Builders<User>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var regex = new BsonRegularExpression(search, "i");

                // ✅ REQUIRED FIX:
                // Use strongly-typed expressions (prevents BSON mismatch)
                filter = Builders<User>.Filter.Or(
                    Builders<User>.Filter.Regex(u => u.Name, regex),
                    Builders<User>.Filter.Regex(u => u.Email, regex)
                );
            }

            var totalCount = await _users.CountDocumentsAsync(filter);

            var users = await _users
                .Find(filter)
                .SortByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .Project(u => new AdminUserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    IsBlocked = u.IsBlocked,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return (users, totalCount);
        }

        // ======================================================
        // UPDATE USER ROLE
        // ======================================================
        public async Task UpdateUserRoleAsync(string id, string role)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update.Set(u => u.Role, role);

            await _users.UpdateOneAsync(filter, update);
        }

        // ======================================================
        // BLOCK USER
        // ======================================================
        public async Task BlockUserAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var update = Builders<User>.Update.Set(u => u.IsBlocked, true);

            await _users.UpdateOneAsync(filter, update);
        }

        // ======================================================
        // DELETE USER
        // ======================================================
        public async Task DeleteUserAsync(string id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }
    }
}
