using System;
using System.Collections.Generic;
using System.Text;

namespace SIAG.WeatherModel
{
    public class SiagWeather
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string hour { get; set; }
        public Today today { get; set; }
        public Tomorrow tomorrow { get; set; }
        public Mountaintoday mountainToday { get; set; }
        public Mountaintomorrow mountainTomorrow { get; set; }
        public int bulletinStatus { get; set; }
        public string type { get; set; }
        public string evolution { get; set; }
        public string evolutionTitle { get; set; }
        public Dayforecast[] dayForecasts { get; set; }
    }

    public class Today
    {
        public DateTime date { get; set; }
        public string hour { get; set; }
        public string title { get; set; }
        public string conditions { get; set; }
        public string temperatures { get; set; }
        public Stationdata[] stationData { get; set; }
        public string imageUrl { get; set; }
        public string weather { get; set; }
        public int bulletinStatus { get; set; }
        public string type { get; set; }
        public double tMinMin { get; set; }
        public double tMinMax { get; set; }
        public double tMaxMin { get; set; }
        public double tMaxMax { get; set; }
        public int reliability { get; set; }
    }

    public class Stationdata
    {
        public Symbol symbol { get; set; }
        public double max { get; set; }
        public double min { get; set; }
    }

    public class Symbol
    {
        public string code { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
    }

    public class Tomorrow
    {
        public DateTime date { get; set; }
        public string hour { get; set; }
        public string title { get; set; }
        public string conditions { get; set; }
        public string temperatures { get; set; }
        public Stationdata[] stationData { get; set; }
        public string imageUrl { get; set; }
        public string weather { get; set; }
        public int bulletinStatus { get; set; }
        public string type { get; set; }
        public double tMinMin { get; set; }
        public double tMinMax { get; set; }
        public double tMaxMin { get; set; }
        public double tMaxMax { get; set; }
        public int reliability { get; set; }
    }

    public class Mountaintoday
    {
        public DateTime date { get; set; }
        public string title { get; set; }
        public string weather { get; set; }
        public string conditions { get; set; }
        public double temp1000 { get; set; }
        public double temp2000 { get; set; }
        public double temp3000 { get; set; }
        public double temp4000 { get; set; }
        public double[] snowLimit { get; set; }
        public double zeroLimit { get; set; }
        public North north { get; set; }
        public South south { get; set; }
        public Wind wind { get; set; }
        public int reliability { get; set; }
        public string moonRise { get; set; }
        public string moonSet { get; set; }
        public string sunRise { get; set; }
        public string sunSet { get; set; }
        public string imageUrl { get; set; }
    }

    public class North
    {
        public string code { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
    }

    public class South
    {
        public string code { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
    }

    public class Wind
    {
        public string code { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
    }

    public class Mountaintomorrow
    {
        public DateTime date { get; set; }
        public string title { get; set; }
        public string weather { get; set; }
        public string conditions { get; set; }
        public double temp1000 { get; set; }
        public double temp2000 { get; set; }
        public double temp3000 { get; set; }
        public double temp4000 { get; set; }
        public double[] snowLimit { get; set; }
        public double zeroLimit { get; set; }
        public North north { get; set; }
        public South south { get; set; }
        public Wind wind { get; set; }
        public int reliability { get; set; }
        public string moonRise { get; set; }
        public string moonSet { get; set; }
        public string sunRise { get; set; }
        public string sunSet { get; set; }
        public string imageUrl { get; set; }
    }

    public class Dayforecast
    {
        public DateTime date { get; set; }
        public int reliability { get; set; }
        public Symbol symbol { get; set; }
        public Tempmax tempMax { get; set; }
        public Tempmin tempMin { get; set; }
    }

    public class Tempmax
    {
        public double min { get; set; }
        public double max { get; set; }
    }

    public class Tempmin
    {
        public double min { get; set; }
        public double max { get; set; }
    }
}
