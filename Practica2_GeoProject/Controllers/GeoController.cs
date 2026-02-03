using Microsoft.AspNetCore.Mvc;
using Practica2_GeoProject.Models;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Practica2_GeoProject.Controllers
{
    public class GeoController : Controller
    {
        private readonly HttpClient _httpClient;

        public GeoController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            // User-Agent obligatorio para OSM
            var userAgent = "PracticaEstudiante_Mexico_ITSON/1.0 (tu_correo@ejemplo.com)";
            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        public async Task<IActionResult> Index(string address = "", string buscarTipo = "", double latActual = 0, double lonActual = 0)
        {
            var modelo = new UbicationModel();

            if (!string.IsNullOrEmpty(address))
            {
                string busqueda = Uri.EscapeDataString(address);
                string urlSearch = $"https://nominatim.openstreetmap.org/search?q={busqueda}&format=json&limit=1";

                try
                {
                    var response = await _httpClient.GetAsync(urlSearch);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var resultados = JsonSerializer.Deserialize<List<NominatimResult>>(jsonString);

                        if (resultados != null && resultados.Count > 0)
                        {
                            var sitio = resultados[0];
                            modelo.Address = sitio.DisplayName;
                            modelo.Found = true;
                            // Convertir string a double con seguridad
                            if (double.TryParse(sitio.Lat, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                                double.TryParse(sitio.Lon, NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                            {
                                modelo.Lat = lat;
                                modelo.Lon = lon;
                            }
                        }
                    }
                }
                catch {}
            }
           
            else if (latActual != 0 && lonActual != 0)
            {
                modelo.Lat = latActual;
                modelo.Lon = lonActual;
                modelo.Found = true;
                modelo.Address = "Ubicación Seleccionada";

                if (!string.IsNullOrEmpty(buscarTipo))
                {
                    string latStr = modelo.Lat.ToString(CultureInfo.InvariantCulture);
                    string lonStr = modelo.Lon.ToString(CultureInfo.InvariantCulture);

                    string query = $@"
                        [out:json][timeout:25];
                        (
                          node[""amenity""=""{buscarTipo}""](around:1500,{latStr},{lonStr});
                          way[""amenity""=""{buscarTipo}""](around:1500,{latStr},{lonStr});
                        );
                        out center;
                    ";

                    string urlOsm = "https://overpass-api.de/api/interpreter?data=" + Uri.EscapeDataString(query);

                    try
                    {
                        var jsonString = await _httpClient.GetStringAsync(urlOsm);

                        using (JsonDocument doc = JsonDocument.Parse(jsonString))
                        {
                            foreach (var element in doc.RootElement.GetProperty("elements").EnumerateArray())
                            {
                                // Solo procesamos si tiene nombre
                                if (element.TryGetProperty("tags", out var tags) && tags.TryGetProperty("name", out var name))
                                {
                                    double itemLat = 0;
                                    double itemLon = 0;

                                    // Lógica inteligente:
                                    // Si es un PUNTO, tiene 'lat' y 'lon' directos.
                                    // Si es un EDIFICIO, tiene un objeto 'center' con 'lat' y 'lon'.
                                    if (element.TryGetProperty("lat", out var l))
                                    {
                                        itemLat = l.GetDouble();
                                        itemLon = element.GetProperty("lon").GetDouble();
                                    }
                                    else if (element.TryGetProperty("center", out var c))
                                    {
                                        itemLat = c.GetProperty("lat").GetDouble();
                                        itemLon = c.GetProperty("lon").GetDouble();
                                    }

                                    // Solo agregamos si logramos obtener coordenadas válidas
                                    if (itemLat != 0 && itemLon != 0)
                                    {
                                        modelo.FindedPlaces.Add(new NearPlace
                                        {
                                            Nombre = name.GetString(),
                                            Tipo = buscarTipo,
                                            Lat = itemLat,
                                            Lon = itemLon
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch { /* Ignorar errores de OSM */ }
                }
            }

            return View(modelo);
        }
    }
}