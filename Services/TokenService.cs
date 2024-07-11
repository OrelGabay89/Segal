
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Shared.Models;

namespace Services
{
    public class TokenService : Interfaces.ITokenService
    {
        private readonly IMongoCollection<Token> _tokens;

        public TokenService(IConfiguration configuration)
        {
            var connectionString = configuration["DatabaseSettings:ConnectionString"];
            var databaseName = configuration["DatabaseSettings:DatabaseName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _tokens = database.GetCollection<Token>("tokens");
        }

        public async Task<List<Token>> GetTokensAsync()
        {
            var filter = Builders<Token>.Filter.Eq(token => token.Status, "Active");
            return await _tokens.Find(filter).ToListAsync();
        }

        public async Task<UpdateResult> UpdateTokensAsync(FilterDefinition<Token> filter, UpdateDefinition<Token> update)
        {
            return await _tokens.UpdateManyAsync(filter, update);
        }

        public async Task<UpdateResult> MarkTokenAsRevokedAsync(FilterDefinition<Token> filter)
        {
            var update = Builders<Token>.Update.Set(t => t.Status, "Revoked");
            return await _tokens.UpdateOneAsync(filter, update);
        }

        public async Task InsertTokenAsync(Token token)
        {
            await _tokens.InsertOneAsync(token);
        }

        public async Task<UpdateResult> RevokeAllTokensAsync()
        {
            var filter = Builders<Token>.Filter.Empty; // This filter matches all documents
            var update = Builders<Token>.Update.Set(t => t.Status, "Revoked"); // Assumes 'Status' is the field to be updated

            return await _tokens.UpdateManyAsync(filter, update);
        }


    }
}
