namespace tp6_torres_zucchini.Service
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.SignalR;
    using System.Linq;

    public class LogActionFilter : IAsyncActionFilter
    {
        private readonly IHubContext<MonitorHub> _hubContext;
        private readonly IMonitorService _monitorService;

        public LogActionFilter(IHubContext<MonitorHub> hubContext, IMonitorService monitorService)
        {
            _hubContext = hubContext;
            _monitorService = monitorService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Ejecutar la acción
            var result = await next();

            // Verificar si el controlador pertenece al grupo "PanelAdmin"
            var controllerType = context.Controller.GetType();
            var apiExplorerSettings = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.ApiExplorerSettingsAttribute), false)
                                                   .FirstOrDefault() as Microsoft.AspNetCore.Mvc.ApiExplorerSettingsAttribute;

            if (apiExplorerSettings?.GroupName == "PanelAdmin")
                return;

            // También verificar por nombre del controlador como respaldo
            var controllerName = controllerType.Name;
            if (controllerName.Contains("AdminController") || controllerName.Contains("PanelAdmin"))
                return;

            try
            {
                // Ejecutar logging automático
                await _monitorService.LogConexionesActivasAsync();
                await _monitorService.LogUltimosComandosAsync();

                // Emitir evento SignalR
                await _hubContext.Clients.All.SendAsync("NuevoEvento");
            }
            catch (Exception ex)
            {
                // Log del error si tienes acceso a un logger
                // _logger.LogError(ex, "Error en LogActionFilter");

                // O si no tienes logger disponible, al menos no dejes que falle silenciosamente
                System.Diagnostics.Debug.WriteLine($"Error en LogActionFilter: {ex.Message}");
            }
        }
    }
}