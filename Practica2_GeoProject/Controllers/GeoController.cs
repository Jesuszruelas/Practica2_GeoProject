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
        private readonly IConfiguration _configuration;

        public GeoController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            // User-Agent obligatorio para OSM
            var userAgent = _configuration["OSM_USER_AGENT"];
            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }

        public async Task<IActionResult> Index(string address = "", string buscarTipo = "", double latActual = 0, double lonActual = 0)
        {
            var modelo = new UbicationModel();
            modelo.TileLayerUrl = _configuration["OSM_TILE_LAYER"];

            if (!string.IsNullOrEmpty(address))
            {
                string busqueda = Uri.EscapeDataString(address);
                string urlSearch = string.Format(_configuration["NOMINATIM_URL"], busqueda);

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
                            // Convertir string to double
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

                    string urlOsm = _configuration["OVERPASS_URL"] + Uri.EscapeDataString(query);

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
                    catch {}
                }
            }
            return View(modelo);
        }
    }
}