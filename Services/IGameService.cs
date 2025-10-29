using APICASyFAMAS.DataTransferObjects;

namespace APICASyFAMAS.Services
{
    public interface IGameService
    {
        Task<(bool Success, RegisterPlayerResponse Response, string ErrorMessage)> RegisterPlayerAsync(RegisterPlayerRequest request);
        Task<(bool Success, StartGameResponse Response, string ErrorMessage)> StartGameAsync(StartGameRequest request);
        Task<(bool Success, GuessNumberResponse Response, string ErrorMessage)> GuessNumberAsync(GuessNumberRequest request);
    }

}

