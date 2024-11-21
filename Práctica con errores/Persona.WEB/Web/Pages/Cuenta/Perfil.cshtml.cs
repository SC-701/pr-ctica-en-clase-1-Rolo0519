using Abstracciones.Interfaces.Reglas;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace Web.Pages.Cuenta
{
    public class PerfilModel : PageModel
    {
        private readonly IConfiguracion _configuracion;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public PerfilRequest perfil { get; set; } = default!;

        [BindProperty]
        public IFormFile? foto { get; set; }

        [BindProperty]
        public IFormFile? curriculum { get; set; }

        [BindProperty]
        public HttpStatusCode envioEstado { get; set; }

        public PerfilModel(IConfiguracion configuracion, IWebHostEnvironment environment)
        {
            _configuracion = configuracion;
            _environment = environment;
        }

        public async Task OnGetAsync()
        {
            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "ObtenerPerfil");
            var cliente = new HttpClient();
            var IdUsuario = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (IdUsuario == null)
                return;

            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value);

            var solicitud = new HttpRequestMessage(HttpMethod.Get, string.Format(endpoint, IdUsuario));
            var respuesta = await cliente.SendAsync(solicitud);

            if (respuesta.StatusCode == HttpStatusCode.OK)
            {
                var resultado = await respuesta.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                perfil = JsonSerializer.Deserialize<PerfilRequest>(resultado, opciones);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string endpoint = _configuracion.ObtenerMetodo("ApiEndPoints", "AgregarPerfil");
            var cliente = new HttpClient();

            // Extraemos el token de autorización
            cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value);

            if (foto != null)
            {
                perfil.Foto = await obtenerDocumentoAsync(foto);
            }

            if (curriculum != null)
            {
                perfil.Curriculum = await obtenerDocumentoAsync(curriculum);
            }

            var contenido = new StringContent(JsonSerializer.Serialize(perfil), System.Text.Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync(endpoint, contenido);
            envioEstado = respuesta.StatusCode;

            if (respuesta.StatusCode == HttpStatusCode.Created)
            {
                return RedirectToPage("Perfil");
            }

            ModelState.AddModelError(string.Empty, "No se pudo agregar el perfil. Intente de nuevo más tarde.");
            return Page();
        }

        public async Task<DocumentoContenido> obtenerDocumentoAsync(IFormFile archivo)
        {
            var file = Path.Combine(_environment.WebRootPath, "Documentos", archivo.FileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                await archivo.CopyToAsync(fileStream);
            }

            byte[] contenido = System.IO.File.ReadAllBytes(file);
            System.IO.File.Delete(file);

            return new DocumentoContenido { Nombre = archivo.FileName, Contenido = contenido };
        }
    }
}