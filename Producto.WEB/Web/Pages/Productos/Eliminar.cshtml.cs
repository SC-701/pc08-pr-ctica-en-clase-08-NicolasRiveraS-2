using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;

namespace Web.Pages.Productos {
    [Authorize(Roles = "2")]
    public class EliminarModel : PageModel {
        private readonly IConfiguracion _configuracion;

        [BindProperty]
        public ProductoDetalle producto { get; set; } = new();

        public EliminarModel(IConfiguracion configuracion) {
            _configuracion = configuracion;
        }

        public async Task<ActionResult> OnGet(Guid? id) {
            if (id is null) {
                return NotFound();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerProducto");
            var cliente = ObtenerClienteConToken();
            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, id));
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.StatusCode == HttpStatusCode.NotFound) {
                return NotFound();
            }

            respuesta.EnsureSuccessStatusCode();
            if (respuesta.StatusCode == HttpStatusCode.OK) {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                producto = JsonSerializer.Deserialize<ProductoDetalle>(resultado, opciones) ?? new ProductoDetalle();
            }

            return Page();
        }

        public async Task<ActionResult> OnPost(Guid? id) {
            var idProducto = id ?? producto.Id;
            if (idProducto == Guid.Empty) {
                return NotFound();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "EliminarProducto");
            var cliente = ObtenerClienteConToken();
            var solicitud = new HttpRequestMessage(HttpMethod.Delete, string.Format(endpoint, idProducto));
            var respuesta = await cliente.SendAsync(solicitud);

            respuesta.EnsureSuccessStatusCode();
            return RedirectToPage("../Index");
        }

        // Helper — extrae el JWT de los claims y configura el HttpClient
        private HttpClient ObtenerClienteConToken () {
            var tokenClaim = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == "Token");
            var cliente = new HttpClient();
            if (tokenClaim != null)
                cliente.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue(
                        "Bearer", tokenClaim.Value);
            return cliente;
        }
    }
}
