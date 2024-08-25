using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Practica01.Controllers
{
    [Route("[controller]")]
    public class OperacionController : Controller
    {
        private readonly ILogger<OperacionController> _logger;
        private readonly Dictionary<string, double> _preciosInstrumentos = new()
        {
            { "S&P 500", 500 },
            { "Dow Jones", 300 },
            { "Bonos US", 120 }
        };

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

            double montoTotalInstrumentos = CalcularMontoTotalInstrumentos(operacion.Instrumentos);
            double montoAbonado = operacion.Monto ?? 0;
            double comision = CalcularComision(montoAbonado);
            double igv = CalcularIGV(montoAbonado);
            double total = CalcularTotal(montoTotalInstrumentos, montoAbonado, comision, igv, operacion.Instrumentos.Count);

            PrepararDatosVista(operacion, igv, comision, total, montoAbonado);

            return View("Index");
        }

        private double CalcularMontoTotalInstrumentos(List<string> instrumentos)
        {
            return instrumentos.Sum(instrumento => _preciosInstrumentos.ContainsKey(instrumento) ? _preciosInstrumentos[instrumento] : 0);
        }

        private double CalcularComision(double monto)
        {
            return monto <= 300 ? 3 : 1;
        }

        private double CalcularIGV(double monto)
        {
            return monto * 0.18;
        }

        private double CalcularTotal(double montoTotalInstrumentos, double montoAbonado, double comision, double igv, int cantidadInstrumentos)
        {
            return montoTotalInstrumentos + (montoAbonado + igv + comision) * cantidadInstrumentos;
        }

        private void PrepararDatosVista(Operacion operacion, double igv, double comision, double total, double montoAbonado)
        {
            ViewData["Instrumentos"] = string.Join(", ", operacion.Instrumentos);
            ViewData["FecOpe"] = operacion.FecOpe.ToString("dd/MM/yyyy");
            ViewData["IGV"] = igv.ToString("F2");
            ViewData["Comision"] = comision.ToString("F2");
            ViewData["Total"] = total.ToString("F2");
            ViewData["Monto"] = montoAbonado.ToString("F2");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }

    public class Operacion
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public DateOnly FecOpe { get; set; }
        public List<string>? Instrumentos { get; set; }
        public double? Monto { get; set; }
    }
}
