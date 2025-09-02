namespace tp6_torres_zucchini.Service
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.SignalR;

    public class LogActionFilter : IActionFilter
    {
        private readonly IHubContext<MonitorHub> _hubContext;
        private readonly IMonitorService _monitorService;

        public LogActionFilter(IHubContext<MonitorHub> hubContext, IMonitorService monitorService)
        {
            _hubContext = hubContext;
            _monitorService = monitorService;
        }

        public void OnActionExecuting(ActionExecutingContext context) { }

        public async void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller.GetType().Name;
            if (controller.Contains("AdminController"))
                return;

            // Ejecutar logging automático
            await _monitorService.LogConexionesActivasAsync();
            await _monitorService.LogUltimosComandosAsync();

            // Emitir evento SignalR
            await _hubContext.Clients.All.SendAsync("NuevoEvento");
        }
    }

}
