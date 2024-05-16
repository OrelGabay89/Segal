namespace IsraelTax.Interfaces
{
    public interface ITokenService
    {
        Task<string> FetchTokenUsingCode(string code);

    }
}