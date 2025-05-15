using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("usuarios-activos")]
    public async Task<IActionResult> ObtenerUsuariosActivos()
    {
        var usuariosActivos = await _context.Users
            .Where(u => u.Activo)
            .ToListAsync();

        return Ok(usuariosActivos);
    }
}
