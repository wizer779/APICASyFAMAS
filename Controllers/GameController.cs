using APICASyFAMAS.DataTransferObjects;
using APICASyFAMAS.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APICASyFAMAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private IGameService _gameService;
        private ILogger<GameController> _logger;

        // Constructor: recibe las dependencias que necesita el controlador
        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        // Endpoint para registrar un jugador nuevo
        [HttpPost("Registrarse")]
        public async Task<IActionResult> Register([FromBody] RegisterPlayerRequest request)
        {
            // Log simple para registrar que llegó una petición
            _logger.LogInformation("POST /api/game/v1/register");

            // Verificar que los datos del request sean válidos
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Message = "Datos inválidos" });
            }

            // Llamar al servicio que tiene la lógica de negocio
            var (success, response, error) = await _gameService.RegisterPlayerAsync(request);

            // Si algo salió mal, devolver el error
            if (!success)
            {
                return BadRequest(new ErrorResponse { Message = error });
            }

            // Si todo salió bien, devolver el ID del jugador
            return Ok(response);
        }

        // Endpoint para iniciar un nuevo juego
        [HttpPost("Iniciar juego")]
        public async Task<IActionResult> Start([FromBody] StartGameRequest request)
        {
            _logger.LogInformation("POST /api/game/v1/start - PlayerId: {PlayerId}", request.PlayerId);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Message = "Datos inválidos" });
            }

            var (success, response, error) = await _gameService.StartGameAsync(request);

            if (!success)
            {
                // Si el jugador no existe, devolver 404
                if (error.Contains("no está registrado"))
                {
                    return NotFound(new ErrorResponse { Message = error });
                }
                // Si ya tiene un juego activo, devolver 400
                return BadRequest(new ErrorResponse { Message = error });
            }

            return Ok(response);
        }

        // Endpoint para hacer un intento de adivinanza
        [HttpPost("Adivina el numero")]
        public async Task<IActionResult> Guess([FromBody] GuessNumberRequest request)
        {
            _logger.LogInformation("POST /api/game/v1/guess - GameId: {GameId}", request.GameId);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Message = "Datos inválidos" });
            }

            var (success, response, error) = await _gameService.GuessNumberAsync(request);

            if (!success)
            {
                // Si el juego no existe, devolver 404
                if (error.Contains("no existe"))
                {
                    return NotFound(new ErrorResponse { Message = error });
                }
                // Si el juego ya terminó o el número es inválido, devolver 400
                return BadRequest(new ErrorResponse { Message = error });
            }

            return Ok(response);
        }
    }
}
