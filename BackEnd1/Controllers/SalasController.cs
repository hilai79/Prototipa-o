using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaSenacApi.Data;
using ReservaSenacApi.Dtos;
using ReservaSenacApi.Models;

namespace ReservaSenacApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalasController(ReservaSenacContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sala>>> Get(
        [FromQuery] string? location,
        [FromQuery] uint? capacity)
    {
        var query = context.Salas.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(x => x.Localizacao == location);

        if (capacity.HasValue)
            query = query.Where(x => x.Capacidade <= capacity.Value);

        return await query.OrderBy(x => x.Nome).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sala>> GetById(ulong id)
    {
        var sala = await context.Salas.FindAsync(id);
        return sala is null ? NotFound() : sala;
    }

    [HttpPost]
    public async Task<ActionResult<Sala>> Create(SalaDto dto)
    {
        var sala = new Sala
        {
            Nome = dto.Nome.Trim(),
            Localizacao = dto.Localizacao.Trim(),
            Capacidade = dto.Capacidade,
            Status = dto.Status
        };

        context.Salas.Add(sala);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Ja existe uma sala com esse nome.");
        }

        return CreatedAtAction(nameof(GetById), new { id = sala.Id }, sala);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Sala>> Update(ulong id, SalaDto dto)
    {
        var sala = await context.Salas.FindAsync(id);
        if (sala is null) return NotFound();

        sala.Nome = dto.Nome.Trim();
        sala.Localizacao = dto.Localizacao.Trim();
        sala.Capacidade = dto.Capacidade;
        sala.Status = dto.Status;

        await context.SaveChangesAsync();
        return sala;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(ulong id)
    {
        var sala = await context.Salas.FindAsync(id);
        if (sala is null) return NotFound();

        context.Salas.Remove(sala);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("A sala nao pode ser excluida porque possui reservas.");
        }

        return NoContent();
    }
}
