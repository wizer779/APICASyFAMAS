using APICASyFAMAS.Data;
using APICASyFAMAS.DataTransferObjects;
using APICASyFAMAS.Models;
using GameCore;
using Microsoft.EntityFrameworkCore;

namespace APICASyFAMAS.Services
{
    public class GameService : IGameService
    {
        private GameDbContext _context;
        private ILogger<GameService> _logger;

        public GameService(GameDbContext context, ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool Success, RegisterPlayerResponse Response, string ErrorMessage)> RegisterPlayerAsync(RegisterPlayerRequest request)
        {
            try
            {
                _logger.LogInformation("Registrando jugador: {FirstName} {LastName}", request.FirstName, request.LastName);

                // Verificar si ya existe
                var existe = await _context.Players
                    .AnyAsync(p => p.FirstName == request.FirstName && p.LastName == request.LastName);

                if (existe)
                {
                    _logger.LogWarning("Jugador ya existe");
                    return (false, null, "El usuario ya se encuentra registrado");
                }

                // Crear jugador
                var player = new Player
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Age = request.Age,
                    RegisterDate = DateTime.Now
                };

                _context.Players.Add(player);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Jugador registrado con ID: {PlayerId}", player.PlayerId);

                return (true, new RegisterPlayerResponse { PlayerId = player.PlayerId }, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar jugador");
                return (false, null, "Error interno del servidor");
            }
        }

        public async Task<(bool Success, StartGameResponse Response, string ErrorMessage)> StartGameAsync(StartGameRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando juego para jugador {PlayerId}", request.PlayerId);

                // Verificar que existe el jugador
                var player = await _context.Players.FindAsync(request.PlayerId);
                if (player == null)
                {
                    return (false, null, $"El jugador con ID {request.PlayerId} no está registrado");
                }

                // Verificar si tiene juego activo
                var juegoActivo = await _context.Games
                    .AnyAsync(g => g.PlayerId == request.PlayerId && !g.IsFinished);

                if (juegoActivo)
                {
                    return (false, null, "Ya tienes un juego activo. Debes finalizarlo antes de comenzar uno nuevo");
                }

                // Generar número secreto
                var secretNumber = GenerateSecretNumber();

                var game = new Game
                {
                    PlayerId = request.PlayerId,
                    SecretNumber = secretNumber,
                    CreatedAt = DateTime.Now,
                    IsFinished = false
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Juego {GameId} creado con número secreto", game.GameId);

                var response = new StartGameResponse
                {
                    GameId = game.GameId,
                    PlayerId = game.PlayerId,
                    CreatedAt = game.CreatedAt
                };

                return (true, response, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar juego");
                return (false, null, "Error interno del servidor");
            }
        }

        public async Task<(bool Success, GuessNumberResponse Response, string ErrorMessage)> GuessNumberAsync(GuessNumberRequest request)
        {
            try
            {
                _logger.LogInformation("Intento de adivinanza para juego {GameId}", request.GameId);

                // Validar número
                if (!IsValidNumber(request.AttemptedNumber))
                {
                    return (false, null, "El número debe tener 4 dígitos sin repetirse");
                }

                // Buscar juego
                var game = await _context.Games.FindAsync(request.GameId);
                if (game == null)
                {
                    return (false, null, $"El juego con ID {request.GameId} no existe");
                }

                // Verificar si ya finalizó
                if (game.IsFinished)
                {
                    return (false, null, $"El juego {request.GameId} ya ha finalizado");
                }

                var resultado = Evaluator.ValidateAttempt(game.SecretNumber, request.AttemptedNumber);

                // Guardar intento
                var attempt = new Attempt
                {
                    GameId = game.GameId,
                    AttemptedNumber = request.AttemptedNumber,
                    Famas = resultado.Fama,
                    Picas = resultado.Pica,
                    Message = resultado.Message,
                    AttemptDate = DateTime.Now
                };

                _context.Attempts.Add(attempt);

                // Si adivinó (4 famas), finalizar juego
                if (resultado.Fama == 4)
                {
                    game.IsFinished = true;
                    game.FinishedAt = DateTime.Now;
                    _logger.LogInformation("Juego {GameId} finalizado", game.GameId);
                }

                await _context.SaveChangesAsync();

                var response = new GuessNumberResponse
                {
                    GameId = game.GameId,
                    AttemptedNumber = request.AttemptedNumber,
                    Message = resultado.Message
                };

                return (true, response, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar intento");
                return (false, null, "Error interno del servidor");
            }
        }

        private string GenerateSecretNumber()
        {
            var random = new Random();
            var digits = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var result = "";

            for (int i = 0; i < 4; i++)
            {
                int index = random.Next(digits.Count);
                result += digits[index];
                digits.RemoveAt(index);
            }

            return result;
        }

        private bool IsValidNumber(string number)
        {
            if (string.IsNullOrEmpty(number) || number.Length != 4)
                return false;

            if (!number.All(char.IsDigit))
                return false;

            if (number.Distinct().Count() != 4)
                return false;

            return true;
        }
    }
}

