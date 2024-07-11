using MongoDB.Driver;
using Shared.Models;

namespace Services.Interfaces
{
    public interface ITokenService
    {
        Task<List<Token>> GetTokensAsync();
        Task<UpdateResult> UpdateTokensAsync(FilterDefinition<Token> filter, UpdateDefinition<Token> update);
    }
}