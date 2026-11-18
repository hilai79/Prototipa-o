namespace ReservaSenacApi.Models;

public class ReservaHistorico
{
    public ulong Id { get; set; }
    public ulong IdReserva { get; set; }
    public ulong? IdUsuario { get; set; }
    public string Acao { get; set; } = string.Empty;
    public string? Detalhes { get; set; }
    public DateTime RegistradoEm { get; set; }
}
