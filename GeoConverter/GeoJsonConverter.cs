namespace GeoConverter;

using System.Text.RegularExpressions;
using System.Xml.Linq;
using Geo;
using Geo.Abstractions.Interfaces;
using Geo.Geometries;
using Geo.IO.GeoJson;

public static class GeoJsonConverter
{
    private static string? ExtractCoordinates(string xml)
    {
        var doc = XDocument.Parse(xml);
        var multiGeo = doc
            ?.Element("kml")
            ?.Element("Document")
            ?.Element("Folder")
            ?.Element("Placemark")
            ?.Element("MultiGeometry");
        return multiGeo
            ?.Element("LineString")
            ?.Element("coordinates")
            ?.Value;
    }

    private static IGeometry ParseGeo(string geo)
    {
        var res = Regex.Matches(geo, @"(?<lat>\d+\.\d+),(?<long>\d+\.\d+)")
            .Select(m => (m.Groups["lat"].Value, m.Groups["long"].Value))
            .Select(x => (float.Parse(x.Item2), float.Parse(x.Item1)))
            .Select(x => new Coordinate(x.Item1, x.Item2));
        return new LineString(res);
    }

    private static string ConvertToGeoJson(IGeometry geometry)
    {
        var writer = new GeoJsonWriter();
        return writer.Write(geometry);
    }

    public static string ConvertFromKml(string kml)
    {
        var coordinates = ExtractCoordinates(kml);
        if (coordinates == null)
        {
            throw new Exception("Could not parse KML file.");
        }
        var geo = ParseGeo(coordinates);
        return ConvertToGeoJson(geo);
    }
}
