using Microsoft.EntityFrameworkCore;
using ReservaSenacApi.Models;

namespace ReservaSenacApi.Data;

public class ReservaSenacContext : DbContext
{
    public ReservaSenacContext(DbContextOptions<ReservaSenacContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Sala> Salas => Set<Sala>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<ReservaHistorico> ReservasHistorico => Set<ReservaHistorico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id_usuario");
            entity.Property(x => x.Nome).HasColumnName("nome");
            entity.Property(x => x.Email).HasColumnName("email");
            entity.Property(x => x.Cargo).HasColumnName("cargo");
            entity.Property(x => x.Perfil).HasColumnName("perfil");
            entity.Property(x => x.Ativo).HasColumnName("ativo");
            entity.Property(x => x.CriadoEm).HasColumnName("criado_em");
        });

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.ToTable("sala");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id_sala");
            entity.Property(x => x.Nome).HasColumnName("nome");
            entity.Property(x => x.Localizacao).HasColumnName("localizacao");
            entity.Property(x => x.Capacidade).HasColumnName("capacidade");
            entity.Property(x => x.Status).HasColumnName("status");
            entity.Property(x => x.CriadoEm).HasColumnName("criado_em");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("reserva");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id_reserva");
            entity.Property(x => x.IdSala).HasColumnName("id_sala");
            entity.Property(x => x.IdResponsavel).HasColumnName("id_responsavel");
            entity.Property(x => x.Objetivo).HasColumnName("objetivo");
            entity.Property(x => x.DataReserva).HasColumnName("data_reserva");
            entity.Property(x => x.HorarioInicio).HasColumnName("horario_inicio");
            entity.Property(x => x.HorarioFim).HasColumnName("horario_fim");
            entity.Property(x => x.Status).HasColumnName("status");
            entity.Property(x => x.Cor).HasColumnName("cor");
            entity.Property(x => x.CriadoEm).HasColumnName("criado_em");
            entity.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");

            entity.HasOne(x => x.Sala)
                .WithMany(x => x.Reservas)
                .HasForeignKey(x => x.IdSala)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Responsavel)
                .WithMany(x => x.Reservas)
                .HasForeignKey(x => x.IdResponsavel)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReservaHistorico>(entity =>
        {
            entity.ToTable("reserva_historico");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id_historico");
            entity.Property(x => x.IdReserva).HasColumnName("id_reserva");
            entity.Property(x => x.IdUsuario).HasColumnName("id_usuario");
            entity.Property(x => x.Acao).HasColumnName("acao");
            entity.Property(x => x.Detalhes).HasColumnName("detalhes");
            entity.Property(x => x.RegistradoEm).HasColumnName("registrado_em");
        });
    }
}
