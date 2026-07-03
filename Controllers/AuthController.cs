using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VulnerableApp.Data;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext db, ILogger<AuthController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Login()
        {
            _logger.LogInformation(
                "Acceso a la vista Login. IP:{IP}",
                HttpContext.Connection.RemoteIpAddress);

            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var inicio = DateTime.UtcNow;

            _logger.LogInformation(
                "Intento de inicio de sesión. Usuario:{Usuario} IP:{IP}",
                username,
                HttpContext.Connection.RemoteIpAddress);

            try
            {
                if (username == "admin" && password == "admin")
                {
                    HttpContext.Session.SetString("User", username);
                    HttpContext.Session.SetInt32("UserId", 1);

                    _logger.LogInformation(
                        "Inicio de sesión exitoso para el administrador.");

                    return RedirectToAction("Dashboard");
                }

                string query =
                    "SELECT * FROM Users WHERE Username = '"
                    + username +
                    "' AND Password = '" +
                    password + "'";

                var user =
                    _db.Users
                    .FromSqlRaw(query)
                    .FirstOrDefault();

                if (user != null)
                {
                    HttpContext.Session.SetString("User", user.Username);
                    HttpContext.Session.SetInt32("UserId", user.Id);

                    _logger.LogInformation(
                        "Inicio de sesión exitoso. Usuario:{Usuario}",
                        user.Username);

                    return RedirectToAction("Dashboard");
                }

                _logger.LogWarning(
                    "Inicio de sesión fallido. Usuario:{Usuario}",
                    username);

                ViewBag.Error = "Usuario/contraseña inválido";

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error durante el proceso de autenticación.");

                throw;
            }
            finally
            {
                _logger.LogInformation(
                    "Fin del proceso de Login. Tiempo:{Tiempo} ms",
                    (DateTime.UtcNow - inicio).TotalMilliseconds);
            }
        }

        public IActionResult Dashboard()
        {
            _logger.LogInformation(
                "Acceso al Dashboard.");

            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                _logger.LogWarning(
                    "Intento de acceso al Dashboard sin autenticación.");

                return RedirectToAction("Login");
            }

            var user = _db.Users.Find(userId.Value);

            return View(user);
        }

        public IActionResult Logout()
        {
            _logger.LogInformation(
                "Cierre de sesión. Usuario:{Usuario}",
                HttpContext.Session.GetString("User"));

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}