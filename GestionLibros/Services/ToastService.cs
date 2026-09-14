using Microsoft.JSInterop;

namespace GestionLibros.Services
{
    public class ToastService(IJSRuntime JS)
    {
        public async Task Mostrar(string tipo, string summary, string detail, int duration = 5000)
        {
            await JS.InvokeVoidAsync("mostrarToast", tipo, summary, detail, duration);
        }

        public Task Success(string summary, string detail, int duration = 5000)
            => Mostrar("success", summary, detail, duration);

        public Task Error(string summary, string detail, int duration = 5000)
            => Mostrar("error", summary, detail, duration);

        public Task Warning(string summary, string detail, int duration = 5000)
            => Mostrar("warning", summary, detail, duration);
    }
}