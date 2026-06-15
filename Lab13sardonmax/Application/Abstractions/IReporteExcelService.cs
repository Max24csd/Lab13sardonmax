using Lab13sardonmax.Application.Reportes;

namespace Lab13sardonmax.Application.Abstractions;

public interface IReporteExcelService
{
    byte[] CrearReporteTickets(IReadOnlyList<TicketReporteDto> tickets);
    byte[] CrearReporteActividadUsuarios(IReadOnlyList<ActividadUsuarioReporteDto> usuarios);
    byte[] ModificarCelda(byte[] archivoExcel, string celda, int nuevoValor);
}
