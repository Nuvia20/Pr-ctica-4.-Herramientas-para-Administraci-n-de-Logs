using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SearchController> _logger;

        public SearchController(AppDbContext db, ILogger<SearchController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(string search)
        {
            var inicio = DateTime.UtcNow;

            _logger.LogInformation(
                "Inicio Search.Index - Usuario:{Usuario} IP:{IP} Parámetro:{Busqueda}",
                User?.Identity?.Name ?? "Anónimo",
                HttpContext.Connection.RemoteIpAddress,
                search);

            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    _logger.LogWarning("La búsqueda fue realizada sin parámetros.");
                    return View(new List<User>());
                }

                string query = "SELECT * FROM Users WHERE Username LIKE '%" + search + "%'";

                var users = _db.Users.FromSqlRaw(query).ToList();

                _logger.LogInformation(
                    "Search.Index encontró {Cantidad} resultados.",
                    users.Count);

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Search.Index");
                throw;
            }
            finally
            {
                var tiempo = DateTime.UtcNow - inicio;

                _logger.LogInformation(
                    "Fin Search.Index - Tiempo:{Tiempo} ms",
                    tiempo.TotalMilliseconds);
            }
        }
    }
}