using System.Text.Json.Serialization;

namespace Practica2_GeoProject.Models
{
    public class UbicationModel
    {
        // Propiedades de dirección
        public string Address { get; set; } // Nombre del lugar (Ej: UTS Sonora)
        public bool Found { get; set; } = false;
        public double Lat { get; set; }
        public double Lon { get; set; }

        public List<NearPlace> FindedPlaces { get; set; } = new List<NearPlace>();
    }

    public class NearPlace
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

   
    public class NominatimResult
    {
        [JsonPropertyName("lat")]
        public string Lat { get; set; }

        [JsonPropertyName("lon")]
        public string Lon { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }

    // 4. Clases para Overpass (Búsqueda de tiendas)
    public class OsmResponse
    {
        [JsonPropertyName("elements")]
        public List<OsmElement> Elements { get; set; }
    }

    public class OsmElement
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("tags")]
        public Dictionary<string, string> Tags { get; set; }
    }
}