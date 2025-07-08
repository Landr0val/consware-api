using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace consware_api.test;

/// <summary>
/// Controlador de prueba para verificar logging y swagger
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TestController : ControllerBase
{
    private readonly Serilog.ILogger _logger = Log.ForContext<TestController>();

    /// <summary>
    /// Endpoint de prueba para verificar funcionamiento básico
    /// </summary>
    /// <returns>Mensaje de prueba</returns>
    /// <response code="200">Prueba exitosa</response>
    [HttpGet]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult Get()
    {
        _logger.Information("Test endpoint called");
        return Ok(new { message = "Test successful", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Endpoint de prueba para generar un error controlado
    /// </summary>
    /// <returns>Error de prueba</returns>
    /// <response code="400">Error de prueba</response>
    [HttpGet("error")]
    [ProducesResponseType(typeof(object), 400)]
    public IActionResult GetError()
    {
        _logger.Warning("Test error endpoint called");
        throw new ArgumentException("This is a test error");
    }

    /// <summary>
    /// Endpoint de prueba para logging con diferentes niveles
    /// </summary>
    /// <param name="level">Nivel de log (info, warning, error)</param>
    /// <returns>Confirmación de log</returns>
    /// <response code="200">Log registrado</response>
    /// <response code="400">Nivel inválido</response>
    [HttpPost("log/{level}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    public IActionResult LogTest(string level)
    {
        switch (level.ToLower())
        {
            case "info":
                _logger.Information("Test info log message");
                break;
            case "warning":
                _logger.Warning("Test warning log message");
                break;
            case "error":
                _logger.Error("Test error log message");
                break;
            default:
                return BadRequest(new { message = "Invalid log level. Use: info, warning, error" });
        }

        return Ok(new { message = $"Log level '{level}' registered successfully" });
    }
}
