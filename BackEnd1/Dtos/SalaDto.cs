using System.ComponentModel.DataAnnotations;

namespace ReservaSenacApi.Dtos;

public class SalaDto
{
    [Required, MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Localizacao { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public uint Capacidade { get; set; }

    [RegularExpression("disponivel|indisponivel|manutencao")]
    public string Status { get; set; } = "disponivel";
}
