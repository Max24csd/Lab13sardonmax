using Lab13sardonmax.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab13sardonmax.Infrastructure.Data;

public sealed class LaboratorioDbContext(DbContextOptions<LaboratorioDbContext> options)
    : DbContext(options)
{
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<UsuarioRol> UsuariosRoles => Set<UsuarioRol>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Respuesta> Respuestas => Set<Respuesta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurarModelo(modelBuilder);
        SembrarDatos(modelBuilder);
    }

    private static void ConfigurarModelo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(rol => rol.Id);
            entity.Property(rol => rol.Id).HasColumnName("role_id");
            entity.Property(rol => rol.Nombre).HasColumnName("role_name").HasMaxLength(50);
            entity.HasIndex(rol => rol.Nombre).IsUnique();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(usuario => usuario.Id);
            entity.Property(usuario => usuario.Id).HasColumnName("user_id");
            entity.Property(usuario => usuario.NombreUsuario).HasColumnName("username").HasMaxLength(100);
            entity.Property(usuario => usuario.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
            entity.Property(usuario => usuario.Correo).HasColumnName("email").HasMaxLength(150);
            entity.Property(usuario => usuario.FechaCreacion).HasColumnName("created_at");
            entity.HasIndex(usuario => usuario.NombreUsuario).IsUnique();
            entity.HasIndex(usuario => usuario.Correo).IsUnique();
        });

        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(usuarioRol => new { usuarioRol.UsuarioId, usuarioRol.RolId });
            entity.Property(usuarioRol => usuarioRol.UsuarioId).HasColumnName("user_id");
            entity.Property(usuarioRol => usuarioRol.RolId).HasColumnName("role_id");
            entity.Property(usuarioRol => usuarioRol.FechaAsignacion).HasColumnName("assigned_at");
            entity.HasOne(usuarioRol => usuarioRol.Usuario)
                .WithMany(usuario => usuario.Roles)
                .HasForeignKey(usuarioRol => usuarioRol.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(usuarioRol => usuarioRol.Rol)
                .WithMany(rol => rol.Usuarios)
                .HasForeignKey(usuarioRol => usuarioRol.RolId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("tickets");
            entity.HasKey(ticket => ticket.Id);
            entity.Property(ticket => ticket.Id).HasColumnName("ticket_id");
            entity.Property(ticket => ticket.UsuarioId).HasColumnName("user_id");
            entity.Property(ticket => ticket.Titulo).HasColumnName("title").HasMaxLength(255);
            entity.Property(ticket => ticket.Descripcion).HasColumnName("description");
            entity.Property(ticket => ticket.Estado).HasColumnName("status").HasMaxLength(20);
            entity.Property(ticket => ticket.FechaCreacion).HasColumnName("created_at");
            entity.Property(ticket => ticket.FechaCierre).HasColumnName("closed_at");
            entity.HasOne(ticket => ticket.Usuario)
                .WithMany(usuario => usuario.Tickets)
                .HasForeignKey(ticket => ticket.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Respuesta>(entity =>
        {
            entity.ToTable("responses");
            entity.HasKey(respuesta => respuesta.Id);
            entity.Property(respuesta => respuesta.Id).HasColumnName("response_id");
            entity.Property(respuesta => respuesta.TicketId).HasColumnName("ticket_id");
            entity.Property(respuesta => respuesta.RespondedorId).HasColumnName("responder_id");
            entity.Property(respuesta => respuesta.Mensaje).HasColumnName("message");
            entity.Property(respuesta => respuesta.FechaCreacion).HasColumnName("created_at");
            entity.HasOne(respuesta => respuesta.Ticket)
                .WithMany(ticket => ticket.Respuestas)
                .HasForeignKey(respuesta => respuesta.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(respuesta => respuesta.Respondedor)
                .WithMany(usuario => usuario.Respuestas)
                .HasForeignKey(respuesta => respuesta.RespondedorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void SembrarDatos(ModelBuilder modelBuilder)
    {
        var administrador = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var soporte = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var cliente = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var ana = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var luis = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var maria = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var carlos = Guid.Parse("20000000-0000-0000-0000-000000000004");
        var ticket1 = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var ticket2 = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var ticket3 = Guid.Parse("30000000-0000-0000-0000-000000000003");
        var ticket4 = Guid.Parse("30000000-0000-0000-0000-000000000004");
        var ticket5 = Guid.Parse("30000000-0000-0000-0000-000000000005");

        modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = administrador, Nombre = "administrador" },
            new Rol { Id = soporte, Nombre = "soporte" },
            new Rol { Id = cliente, Nombre = "cliente" });

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = ana, NombreUsuario = "ana.admin", PasswordHash = "hash-demo", Correo = "ana@ticketera.pe", FechaCreacion = new DateTime(2026, 4, 1, 8, 0, 0) },
            new Usuario { Id = luis, NombreUsuario = "luis.soporte", PasswordHash = "hash-demo", Correo = "luis@ticketera.pe", FechaCreacion = new DateTime(2026, 4, 2, 9, 0, 0) },
            new Usuario { Id = maria, NombreUsuario = "maria.cliente", PasswordHash = "hash-demo", Correo = "maria@email.com", FechaCreacion = new DateTime(2026, 4, 5, 10, 0, 0) },
            new Usuario { Id = carlos, NombreUsuario = "carlos.cliente", PasswordHash = "hash-demo", Correo = "carlos@email.com", FechaCreacion = new DateTime(2026, 4, 8, 11, 0, 0) });

        modelBuilder.Entity<UsuarioRol>().HasData(
            new UsuarioRol { UsuarioId = ana, RolId = administrador, FechaAsignacion = new DateTime(2026, 4, 1, 8, 5, 0) },
            new UsuarioRol { UsuarioId = luis, RolId = soporte, FechaAsignacion = new DateTime(2026, 4, 2, 9, 5, 0) },
            new UsuarioRol { UsuarioId = maria, RolId = cliente, FechaAsignacion = new DateTime(2026, 4, 5, 10, 5, 0) },
            new UsuarioRol { UsuarioId = carlos, RolId = cliente, FechaAsignacion = new DateTime(2026, 4, 8, 11, 5, 0) });

        modelBuilder.Entity<Ticket>().HasData(
            new Ticket { Id = ticket1, UsuarioId = maria, Titulo = "No puedo iniciar sesion", Descripcion = "La plataforma rechaza mis credenciales.", Estado = "cerrado", FechaCreacion = new DateTime(2026, 5, 20, 9, 10, 0), FechaCierre = new DateTime(2026, 5, 20, 12, 30, 0) },
            new Ticket { Id = ticket2, UsuarioId = carlos, Titulo = "Error al descargar comprobante", Descripcion = "El boton de descarga no responde.", Estado = "en_proceso", FechaCreacion = new DateTime(2026, 6, 3, 15, 20, 0) },
            new Ticket { Id = ticket3, UsuarioId = maria, Titulo = "Actualizar correo de contacto", Descripcion = "Necesito cambiar el correo de mi cuenta.", Estado = "abierto", FechaCreacion = new DateTime(2026, 6, 7, 10, 45, 0) },
            new Ticket { Id = ticket4, UsuarioId = carlos, Titulo = "Pago duplicado", Descripcion = "Se registro dos veces el mismo pago.", Estado = "cerrado", FechaCreacion = new DateTime(2026, 5, 28, 8, 15, 0), FechaCierre = new DateTime(2026, 5, 29, 16, 0, 0) },
            new Ticket { Id = ticket5, UsuarioId = maria, Titulo = "Consulta sobre plan empresarial", Descripcion = "Solicito informacion sobre beneficios.", Estado = "cerrado", FechaCreacion = new DateTime(2026, 6, 1, 11, 0, 0), FechaCierre = new DateTime(2026, 6, 1, 13, 20, 0) });

        modelBuilder.Entity<Respuesta>().HasData(
            new Respuesta { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), TicketId = ticket1, RespondedorId = luis, Mensaje = "Se restablecio la contrasena.", FechaCreacion = new DateTime(2026, 5, 20, 11, 45, 0) },
            new Respuesta { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), TicketId = ticket2, RespondedorId = luis, Mensaje = "Estamos revisando el modulo de comprobantes.", FechaCreacion = new DateTime(2026, 6, 3, 16, 0, 0) },
            new Respuesta { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), TicketId = ticket4, RespondedorId = ana, Mensaje = "El pago duplicado fue anulado.", FechaCreacion = new DateTime(2026, 5, 29, 15, 40, 0) },
            new Respuesta { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), TicketId = ticket5, RespondedorId = luis, Mensaje = "Se envio la informacion solicitada.", FechaCreacion = new DateTime(2026, 6, 1, 13, 0, 0) });
    }
}
