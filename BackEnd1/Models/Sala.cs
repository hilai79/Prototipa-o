using System.ComponentModel.DataAnnotations;

namespace ReservaSenacApi.Models;

public class Sala
{
    public ulong Id { get; set; }

    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Localizacao { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public uint Capacidade { get; set; }

    [Required, RegularExpression("disponivel|indisponivel|manutencao")]
    public string Status { get; set; } = "disponivel";

    public DateTime CriadoEm { get; set; }
    public ICollection<Reserva> Reservas { get; set; } = [];
}
