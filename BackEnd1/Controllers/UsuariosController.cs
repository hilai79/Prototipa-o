using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaSenacApi.Data;
using ReservaSenacApi.Models;

namespace ReservaSenacApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(ReservaSenacContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get()
        => await context.Usuarios.AsNoTracking().Where(x => x.Ativo).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetById(ulong id)
    {
        var usuario = await context.Usuarios.FindAsync(id);
        return usuario is null ? NotFound() : usuario;
    }
}
