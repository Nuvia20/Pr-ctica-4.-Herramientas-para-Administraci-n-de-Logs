using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Constructor
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var inicio = DateTime.UtcNow;

            _logger.LogInformation(
                "Inicio Home.Index - Usuario:{Usuario} - IP:{IP}",
                User?.Identity?.Name ?? "Anónimo",
                HttpContext.Connection.RemoteIpAddress);

            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Home.Index");
                throw;
            }
            finally
            {
                var tiempo = DateTime.UtcNow - inicio;

                _logger.LogInformation(
                    "Fin Home.Index - Tiempo:{Tiempo} ms",
                    tiempo.TotalMilliseconds);
            }
        }

        public IActionResult Privacy()
        {
            var inicio = DateTime.UtcNow;

            _logger.LogInformation(
                "Inicio Home.Privacy - Usuario:{Usuario} - IP:{IP}",
                User?.Identity?.Name ?? "Anónimo",
                HttpContext.Connection.RemoteIpAddress);

            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Home.Privacy");
                throw;
            }
            finally
            {
                var tiempo = DateTime.UtcNow - inicio;

                _logger.LogInformation(
                    "Fin Home.Privacy - Tiempo:{Tiempo} ms",
                    tiempo.TotalMilliseconds);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError(
                "Se ejecutó la acción Error. RequestId: {RequestId}",
                Activity.Current?.Id ?? HttpContext.TraceIdentifier);

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}