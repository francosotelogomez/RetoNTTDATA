using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using NttDataRetoTecnicoFS.Models;
[ApiController]
[Route("api/[controller]")]
public class PersonaController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public PersonaController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    [HttpGet]
public async Task<IActionResult> ObtenerPersonas()
{
    try
    {
        var client = _clientFactory.CreateClient();
        
        // Consumo asíncrono definiendo el parámetro 'results=10' según la especificación funcional
        var response = await client.GetAsync("https://randomuser.me/api/?results=10");
        
        if (!response.IsSuccessStatusCode) return BadRequest();

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        // Mapeo utilizando la clase de modelo 'Persona' para asegurar una arquitectura cliente/servidor tipada
        var users = json.RootElement.GetProperty("results").EnumerateArray().Select(u => new Persona {
            Nombre = $"{u.GetProperty("name").GetProperty("first")} {u.GetProperty("name").GetProperty("last")}",
            Genero = u.GetProperty("gender").GetString(),
            Ubicacion = $"{u.GetProperty("location").GetProperty("city")}, {u.GetProperty("location").GetProperty("country")}",
            Email = u.GetProperty("email").GetString(), // Asegúrate que en tu clase Persona se llame así
            Nacimiento = u.GetProperty("dob").GetProperty("date").GetDateTime().ToString("dd/MM/yyyy"),
            Foto = u.GetProperty("picture").GetProperty("large").GetString() // Asegúrate que en tu clase Persona se llame así
        }).ToList(); // .ToList() evita el error de ObjectDisposedException

        return Ok(users);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"error: {ex.Message}");
    }
}
}