namespace Lab13sardonmax.Application.Reportes;

public sealed record TicketReporteDto(
    string Codigo,
    string Titulo,
    string Usuario,
    string Estado,
    DateTime FechaCreacion,
    DateTime? FechaCierre,
    int TotalRespuestas,
    double HorasAtencion);

public sealed record ActividadUsuarioReporteDto(
    string Usuario,
    string Correo,
    string Roles,
    DateTime FechaRegistro,
    int TicketsCreados,
    int RespuestasRealizadas,
    int TicketsCerrados);

public sealed record ResumenReportesDto(
    int TotalTickets,
    int TicketsAbiertos,
    int TicketsEnProceso,
    int TicketsCerrados,
    int TotalUsuarios,
    int TotalRespuestas);
