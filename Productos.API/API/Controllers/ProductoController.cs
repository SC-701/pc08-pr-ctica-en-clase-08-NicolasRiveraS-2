using Abstracciones.Interfaces.API;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductoController : ControllerBase, IProductoController {
        private readonly IProductoFlujo _productoFlujo;
        private readonly ILogger<ProductoController> _logger;

        public ProductoController(IProductoFlujo productoFlujo, ILogger<ProductoController> logger) {
            _productoFlujo = productoFlujo;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Agregar(ProductoRequest producto) {
            var resultado = await _productoFlujo.Agregar(producto);
            return CreatedAtAction(nameof(Obtener), new { Id = resultado }, null);
        }

        [HttpPut("{Id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Editar(Guid Id, ProductoRequest producto) {
            var resultado = await _productoFlujo.Editar(Id, producto);
            return Ok(resultado);
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> Eliminar(Guid Id) {
            await _productoFlujo.Eliminar(Id);
            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Obtener() {
            var resultado = await _productoFlujo.Obtener();
            if (!resultado.Any()) {
                return NoContent();
            }
            return Ok(resultado);
        }

        [HttpGet("{Id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Obtener(Guid Id) {
            var resultado = await _productoFlujo.Obtener(Id);
            if (resultado == null) {
                return NotFound();
            }
            return Ok(resultado);
        }

        [HttpGet("categorias")]
        public async Task<IActionResult> ObtenerCategorias() {
            var resultado = await _productoFlujo.ObtenerCategorias();
            if (!resultado.Any()) {
                return NoContent();
            }
            return Ok(resultado);
        }

        [HttpGet("subcategorias/{IdCategoria}")]
        public async Task<IActionResult> ObtenerSubCategorias(Guid IdCategoria) {
            var resultado = await _productoFlujo.ObtenerSubCategorias(IdCategoria);
            if (!resultado.Any()) {
                return NoContent();
            }
            return Ok(resultado);
        }
    }
}
