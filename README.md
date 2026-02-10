# Practica2_GeoProject
Practica 2 Web
## Guía Práctica: Geolocalización con OSM y ASP.NET Core

**Objetivo:** Desarrollar una aplicación web MVC que consuma APIs geográficas gratuitas para ubicar al usuario en un mapa y encontrar puntos de interés cercanos sin costo.

#equipo 1 - Geolocalización
Jesus Daniel Osuna Luna
Gidel Zabdiel Jimenez Cervantes
Daniel Omar Almada Serrano
Jesús René Zamora Ruelas
Jesus Dario Taveres Hernandez


 **Configurar variables de entorno:**
- Revisa el archivo `.env.example` y crea un archivo `.env` con tus valores.
- Asegúrate de definir:
  - `OSM_USER_AGENT`
  - `NOMINATIM_URL`
  - `OVERPASS_URL`
  - `OSM_TILE_LAYER`

 **Restaurar paquetes NuGet:**
- Visual Studio lo hace automáticamente al abrir el proyecto.
- O manualmente:
  ```sh
  dotnet restore
  ```

## Ejecución

 **Compilar y ejecutar:**
- Desde Visual Studio: pulsa __F5__ o __Ctrl+F5__.
- Desde terminal:
  ```sh
  dotnet run
  ```

**Acceder a la aplicación:**
- Abre tu navegador y visita: [http://localhost:5000](http://localhost:5000) (o el puerto indicado).

## Uso

- Ingresa una dirección en el buscador.
- Visualiza la ubicación en el mapa.
- Busca lugares cercanos seleccionando el tipo (farmacia, supermercado, restaurante).
- Los resultados aparecen en la lista y en el mapa.

## Estructura del Proyecto

- `Controllers/GeoController.cs`: Lógica principal de búsqueda y consulta a APIs externas.
- `Models/UbicationModel.cs`: Modelo de datos para ubicaciones y lugares cercanos.
- `Views/Geo/Index.cshtml`: Vista principal con el mapa y buscador.
- `wwwroot/css/site.css`: Estilos personalizados.

## Recursos Externos

- [OpenStreetMap](https://www.openstreetmap.org/)
- [Overpass API](https://overpass-api.de/)
- [Leaflet.js](https://leafletjs.com/)

## Contacto

Para dudas o sugerencias, contacta al autor.
