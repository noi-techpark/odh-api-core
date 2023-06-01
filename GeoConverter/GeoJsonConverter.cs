// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace GeoConverter;

using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Geo;
using Geo.Abstractions.Interfaces;
using Geo.Geometries;
using Geo.Gps.Serialization;
using Geo.IO.GeoJson;

public static class GeoJsonConverter
{
    private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;

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
            .Select(x => (double.Parse(x.Item2, _culture), double.Parse(x.Item1, _culture)))
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

    public static string ConvertFromGpx(string gpx)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(gpx);
        var stream = new MemoryStream(bytes);
        var serializer = new Gpx11Serializer();
        var serialized = serializer.DeSerialize(new StreamWrapper(stream));
        var geometry =
            serialized.Waypoints.Any() ?
            serialized.Waypoints :
            serialized.Tracks.SelectMany(x => x.Segments).SelectMany(x => x.Waypoints);
        var coordinates = geometry.Select(x => x.Coordinate);
        var writer = new GeoJsonWriter();
        return ConvertToGeoJson(new LineString(coordinates));
    }
}
