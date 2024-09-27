using Microsoft.AspNetCore.Mvc;
using VoteAppBackend.Data;
using VoteAppBackend.Models;
using VoteAppBackend.Services;
using System.Threading.Tasks;
using System.Linq;

namespace VoteAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PreferencesController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint para enviar las preferencias del usuario
        [HttpPost]
        public async Task<IActionResult> SubmitPreferences([FromBody] UserPreference preference)
        {
            if (preference == null || string.IsNullOrEmpty(preference.UserId) || preference.Preferences == null)
            {
                return BadRequest("Preferencias inválidas.");
            }

            // Verificar si el usuario ya tiene preferencias guardadas
            var existingPreference = _context.UserPreferences.FirstOrDefault(up => up.UserId == preference.UserId);
            if (existingPreference != null)
            {
                // Actualizar las preferencias existentes
                existingPreference.Preferences = preference.Preferences;
                _context.UserPreferences.Update(existingPreference);
            }
            else
            {
                // Guardar nuevas preferencias
                _context.UserPreferences.Add(preference);
            }

            await _context.SaveChangesAsync();

            return Ok("Preferencias guardadas exitosamente.");
        }

        // Endpoint para obtener recomendaciones para un usuario específico
        [HttpGet("{userId}")]
        public IActionResult GetRecommendations(string userId)
        {
            // Obtener todas las preferencias de los usuarios
            var preferences = _context.UserPreferences.ToList();

            // Verificar que el usuario exista
            var userPreference = preferences.FirstOrDefault(up => up.UserId == userId);
            if (userPreference == null)
            {
                return NotFound("No se encontraron preferencias para el usuario especificado.");
            }

            // Crear el servicio de recomendación
            var recommendationService = new RecommendationService(preferences);

            // Calcular vecinos más cercanos
            var neighbors = recommendationService.ComputeNearestNeighbor(userId);

            // Retornar los vecinos más cercanos
            return Ok(neighbors);
        }
    }
}
