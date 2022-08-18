using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class PushServerMessageHelper
    {
        public static PushServerMessage GetPuhServerMessage(
            string title,
            string text,
            string language,
            string group,
            string? image = null,
            string? video = null
        )
        {
            var message = new PushServerMessage();
            message.text = text;
            message.title = title;
            message.video = video ?? "";
            message.image = image ?? "";
            var destination = new PushServerDestination();
            destination.group = group;
            destination.language = language;

            return message;
        }
    }
}
