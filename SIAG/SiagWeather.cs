using System;
using System.Collections.Generic;
using System.Text;

namespace SIAG.WeatherModel
{
    //TODO ADD SIAG Weather json generated class

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
        public int tMinMin { get; set; }
        public int tMinMax { get; set; }
        public int tMaxMin { get; set; }
        public int tMaxMax { get; set; }
        public int reliability { get; set; }
    }

    public class Stationdata
    {
        public Symbol symbol { get; set; }
        public int max { get; set; }
        public int min { get; set; }
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
        public int tMinMin { get; set; }
        public int tMinMax { get; set; }
        public int tMaxMin { get; set; }
        public int tMaxMax { get; set; }
        public int reliability { get; set; }
    }

    public class Mountaintoday
    {
        public DateTime date { get; set; }
        public string title { get; set; }
        public string weather { get; set; }
        public string conditions { get; set; }
        public int temp1000 { get; set; }
        public int temp2000 { get; set; }
        public int temp3000 { get; set; }
        public int temp4000 { get; set; }
        public int[] snowLimit { get; set; }
        public int zeroLimit { get; set; }
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
        public int temp1000 { get; set; }
        public int temp2000 { get; set; }
        public int temp3000 { get; set; }
        public int temp4000 { get; set; }
        public int[] snowLimit { get; set; }
        public int zeroLimit { get; set; }
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
        public int min { get; set; }
        public int max { get; set; }
    }

    public class Tempmin
    {
        public int min { get; set; }
        public int max { get; set; }
    }

}
