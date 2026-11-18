using System.ComponentModel.DataAnnotations;

namespace ReservaSenacApi.Dtos;

public class ReservaDto
{
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

    [RegularExpression("confirmada|cancelada|concluida")]
    public string Status { get; set; } = "confirmada";

    [MaxLength(20)]
    public string? Cor { get; set; }
}

public class ReservaFrontDto
{
    public ulong Id { get; set; }
    public string Room { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string Status { get; set; } = string.Empty;
}
