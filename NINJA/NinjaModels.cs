using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using DataModel;

namespace NINJA
{

    #region Ninja classes

    public class NinjaObject<T>
    {
        public int offset { get; set; }
        public ICollection<NinjaData<T>> data { get; set; }
        public int limit { get; set; }
    }

    public class NinjaData<T>
    {
        public Dictionary<string, T> tmetadata { get; set; }

        public T smetadata { get; set; }
        public NinjaCoordinates scoordinate { get; set; }
        public string sname { get; set; }
        public string sorigin { get; set; }
        public string stype { get; set; }
        public string scode { get; set; }
    }

    public class NinjaCoordinates
    {
        public double x { get; set; }
        public double y { get; set; }
        public int srid { get; set; }
    }

    public class NinjaEvent
    {
        public string place { get; set; }
        public string room { get; set; }
        public string ticket { get; set; }
        public string link_to_ticket_info { get; set; }
        public string link { get; set; }
        public IDictionary<string, string> title { get; set; }
        public IDictionary<string, string> decription { get; set; }
        public IDictionary<string, string> event_type { get; set; }
        public string event_type_key { get; set; }
        [JsonProperty("begin_date")]
        public string begin_date { get; set; }
        [JsonProperty("end_date")]
        public string end_date { get; set; }
        [JsonProperty("begin_time")]
        public string begin_time { get; set; }
        [JsonProperty("end_time")]
        public string end_time { get; set; }
        [JsonProperty("price")]
        public Nullable<double> price { get; set; }
        [JsonProperty("number_of_seats")]
        public Nullable<int> number_of_seats { get; set; }
    }

    public class NinjaPlaceRoom
    {
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> decription { get; set; }
        public Nullable<int> floor { get; set; }
        public string phone { get; set; }
        public string place { get; set; }
        public IDictionary<string, string> address { get; set; }
        public IDictionary<string, string> city { get; set; }
        public string placeid { get; set; }
        public string email { get; set; }
        public Nullable<int> max_seats { get; set; }
        public string open_time { get; set; }
        public string closing_time { get; set; }
        public string sheetName { get; set; }
        public string closing_days { get; set; }
        public string zipcode { get; set; }
        public string province { get; set; }
    }

    #endregion
}
