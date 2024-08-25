using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Practica01.Controllers
{
    [Route("[controller]")]
    public class OperacionController : Controller
    {
        private readonly ILogger<OperacionController> _logger;

        public OperacionController(ILogger<OperacionController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(Operacion operacion)
        {
            if (operacion.Instrumentos == null || !operacion.Instrumentos.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un instrumento");
                return View("Index", operacion);
            }

            var precios = new Dictionary<string, double>
            {
                { "S&P 500", 500 },
                { "Dow Jones", 300 },
                { "Bonos US", 120 }
            };

            double montoTotalInstrumentos = operacion.Instrumentos.Sum(instrumento => precios.ContainsKey(instrumento) ? precios[instrumento] : 0);

            double montoAbonado = operacion.Monto.HasValue ? operacion.Monto.Value : 0;
            double comision = montoAbonado <= 300 ? 3 : 1;
            double igv = montoAbonado * 0.18;
            double total = montoTotalInstrumentos + (montoAbonado + igv + comision)*operacion.Instrumentos.Count ;

            ViewData["Instrumentos"] = string.Join(", ", operacion.Instrumentos);
            ViewData["FecOpe"] = operacion.FecOpe.ToString("dd/MM/yyyy");
            ViewData["IGV"] = igv.ToString("F2");
            ViewData["Comision"] = comision.ToString("F2");
            ViewData["Total"] = total.ToString("F2");
            ViewData["Monto"] = montoAbonado.ToString("F2");

            return View("Index");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }

        public class Operacion{
        public string? Nombre { get; set;}
        public string? Apellido { get;set;}
        public string? Correo { get; set;}
        public DateOnly FecOpe { get; set;}
        public List<string>? Instrumentos { get; set;}
        public double? Monto { get; set;}
    }
}