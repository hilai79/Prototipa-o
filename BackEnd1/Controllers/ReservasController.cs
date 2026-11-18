using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaSenacApi.Data;
using ReservaSenacApi.Dtos;
using ReservaSenacApi.Models;

namespace ReservaSenacApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController(ReservaSenacContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservaFrontDto>>> Get(
        [FromQuery] DateOnly? date,
        [FromQuery] ulong? roomId,
        [FromQuery] string? status,
        [FromQuery] string? search)
    {
        var query = context.Reservas
            .AsNoTracking()
            .Include(x => x.Sala)
            .Include(x => x.Responsavel)
            .AsQueryable();

        if (date.HasValue)
            query = query.Where(x => x.DataReserva == date.Value);
        if (roomId.HasValue)
            query = query.Where(x => x.IdSala == roomId.Value);
        if (!string.IsNullOrWhiteSpace(status) && status != "todos")
            query = query.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.Objetivo.Contains(term) ||
                (x.Sala != null && x.Sala.Nome.Contains(term)) ||
                (x.Responsavel != null && x.Responsavel.Nome.Contains(term)));
        }

        var reservas = await query
            .OrderBy(x => x.DataReserva)
            .ThenBy(x => x.HorarioInicio)
            .Select(x => new ReservaFrontDto
            {
                Id = x.Id,
                Room = x.Sala!.Nome,
                Date = x.DataReserva,
                Start = x.HorarioInicio,
                End = x.HorarioFim,
                Title = x.Objetivo,
                Responsible = x.Responsavel!.Nome,
                Color = x.Cor,
                Status = x.Status
            })
            .ToListAsync();

        return reservas;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reserva>> GetById(ulong id)
    {
        var reserva = await context.Reservas
            .Include(x => x.Sala)
            .Include(x => x.Responsavel)
            .FirstOrDefaultAsync(x => x.Id == id);

        return reserva is null ? NotFound() : reserva;
    }

    [HttpPost]
    public async Task<ActionResult<Reserva>> Create(ReservaDto dto)
    {
        if (dto.HorarioFim <= dto.HorarioInicio)
            return BadRequest("O horario de termino deve ser posterior ao inicio.");

        var conflict = await context.Reservas.AnyAsync(x =>
            x.IdSala == dto.IdSala &&
            x.DataReserva == dto.DataReserva &&
            x.Status == "confirmada" &&
            dto.HorarioInicio < x.HorarioFim &&
            dto.HorarioFim > x.HorarioInicio);

        if (conflict)
            return Conflict("Conflito de agenda: a sala ja esta reservada nesse periodo.");

        var reserva = new Reserva
        {
            IdSala = dto.IdSala,
            IdResponsavel = dto.IdResponsavel,
            Objetivo = dto.Objetivo.Trim(),
            DataReserva = dto.DataReserva,
            HorarioInicio = dto.HorarioInicio,
            HorarioFim = dto.HorarioFim,
            Status = dto.Status,
            Cor = dto.Cor
        };

        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();

        context.ReservasHistorico.Add(new ReservaHistorico
        {
            IdReserva = reserva.Id,
            IdUsuario = reserva.IdResponsavel,
            Acao = "criada",
            Detalhes = "Reserva criada pelo backend."
        });
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = reserva.Id }, reserva);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Reserva>> Update(ulong id, ReservaDto dto)
    {
        var reserva = await context.Reservas.FindAsync(id);
        if (reserva is null) return NotFound();

        if (dto.HorarioFim <= dto.HorarioInicio)
            return BadRequest("O horario de termino deve ser posterior ao inicio.");

        var conflict = await context.Reservas.AnyAsync(x =>
            x.Id != id &&
            x.IdSala == dto.IdSala &&
            x.DataReserva == dto.DataReserva &&
            x.Status == "confirmada" &&
            dto.HorarioInicio < x.HorarioFim &&
            dto.HorarioFim > x.HorarioInicio);

        if (dto.Status == "confirmada" && conflict)
            return Conflict("Conflito de agenda: a sala ja esta reservada nesse periodo.");

        reserva.IdSala = dto.IdSala;
        reserva.IdResponsavel = dto.IdResponsavel;
        reserva.Objetivo = dto.Objetivo.Trim();
        reserva.DataReserva = dto.DataReserva;
        reserva.HorarioInicio = dto.HorarioInicio;
        reserva.HorarioFim = dto.HorarioFim;
        reserva.Status = dto.Status;
        reserva.Cor = dto.Cor;

        await context.SaveChangesAsync();

        context.ReservasHistorico.Add(new ReservaHistorico
        {
            IdReserva = reserva.Id,
            IdUsuario = reserva.IdResponsavel,
            Acao = dto.Status == "cancelada" ? "cancelada" : "alterada",
            Detalhes = dto.Status == "cancelada"
                ? "Reserva cancelada pelo backend."
                : "Reserva alterada pelo backend."
        });
        await context.SaveChangesAsync();

        return reserva;
    }

    [HttpPatch("{id}/cancelar")]
    public async Task<IActionResult> Cancel(ulong id)
    {
        var reserva = await context.Reservas.FindAsync(id);
        if (reserva is null) return NotFound();

        reserva.Status = "cancelada";
        await context.SaveChangesAsync();

        context.ReservasHistorico.Add(new ReservaHistorico
        {
            IdReserva = reserva.Id,
            IdUsuario = reserva.IdResponsavel,
            Acao = "cancelada",
            Detalhes = "Reserva cancelada pelo backend."
        });
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(ulong id)
    {
        var reserva = await context.Reservas.FindAsync(id);
        if (reserva is null) return NotFound();

        context.Reservas.Remove(reserva);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
