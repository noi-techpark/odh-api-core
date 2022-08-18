using System.Collections.Generic;

namespace DataModel
{
    public class PushServerMessage
    {
        public PushServerMessage() { }

        public string? title { get; set; }
        public string? text { get; set; }
        public string? image { get; set; }
        public string video { get; set; }
        public PushServerDestination destination { get; set; }
    }

    public class PushServerDestination
    {
        public string group { get; set; }
        public string language { get; set; }
    }

    public class PushServerCustomMessage
    {
        public PushServerCustomMessage()
        {
            destination = new Dictionary<string, List<string>>();
        }

        public string title { get; set; }
        public string text { get; set; }

        public PushServerCustom custom { get; set; }

        //public Dictionary<string, string> image { get; set; }
        //public Dictionary<string, string> video { get; set; }
        public Dictionary<string, List<string>> destination { get; set; }
    }

    public class PushServerCustom
    {
        public string group { get; set; }
        public string listId { get; set; }
        public string listName { get; set; }
        public string listOwner { get; set; }
    }
}
