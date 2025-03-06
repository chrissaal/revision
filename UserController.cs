/* Controlador que recibirá el token, extraerá el email, realizará la consulta a la base de datos y devolverá los resultados en JSON. */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize]
    /* Método que recibe el token OAuth2, extrae el email, consulta la base de datos 
    y devuelve la información del usuario en formato JSON */
    public async Task<IActionResult> GetUserInfo()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var token = authHeader.Replace("Bearer ", string.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var email = jwtToken.Claims.First(claim => claim.Type == "email").Value;

        var user = await _context.Users
            .Where(u => u.Email == email)
            .Select(u => new { u.Id, u.Name, u.Email })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        var userJson = JsonConvert.SerializeObject(user);
        return new ContentResult
        {
            Content = userJson,
            ContentType = "application/json",
            StatusCode = 200
        };
    }
}
