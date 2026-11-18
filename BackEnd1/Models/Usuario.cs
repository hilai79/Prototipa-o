using System.ComponentModel.DataAnnotations;

namespace ReservaSenacApi.Models;

public class Usuario
{
    public ulong Id { get; set; }

    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(180), EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Cargo { get; set; } = string.Empty;

    [Required, RegularExpression("administrador|gestor|colaborador")]
    public string Perfil { get; set; } = "colaborador";

    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; }
    public ICollection<Reserva> Reservas { get; set; } = [];
}
