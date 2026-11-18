using System.ComponentModel.DataAnnotations;

namespace ReservaSenacApi.Models;

public class Reserva
{
    public ulong Id { get; set; }

    [Required]
    public ulong IdSala { get; set; }

    [Required]
    public ulong IdResponsavel { get; set; }

    [Required, MaxLength(200)]
    public string Objetivo { get; set; } = string.Empty;

    [Required]
    public DateOnly DataReserva { get; set; }

    [Required]
    public TimeSpan HorarioInicio { get; set; }

    [Required]
    public TimeSpan HorarioFim { get; set; }

    [Required, RegularExpression("confirmada|cancelada|concluida")]
    public string Status { get; set; } = "confirmada";

    [MaxLength(20)]
    public string? Cor { get; set; }

    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
    public Sala? Sala { get; set; }
    public Usuario? Responsavel { get; set; }
}
