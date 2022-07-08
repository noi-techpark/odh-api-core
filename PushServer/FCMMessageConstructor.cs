using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace PushServer
{
    public class FCMMessageConstructor
    {
        public static FCMModels ConstructMyMessage(string identifier, string language, IIdentifiable myobject)
        {
            var message = default(FCMModels);

            if (identifier == "noicommunityapp" && myobject is ArticlesLinked)
            {
                message = new FCMModels();

                message.to = "/topics/newsfeednoi_" + language.ToLower();

                string deeplink = "noi-community://it.bz.noi.community/newsDetails/" + myobject.Id;

                message.data = new { deep_link = deeplink };
                FCMNotification notification = new FCMNotification();

                notification.icon = "ic_notification";
                notification.link = deeplink;
                notification.title = ((ArticlesLinked)myobject).Detail.ContainsKey(language) && !String.IsNullOrEmpty(((ArticlesLinked)myobject).Detail[language].Title) ? ((ArticlesLinked)myobject).Detail[language].Title : "Noi Community App News";
                notification.body = ((ArticlesLinked)myobject).Detail.ContainsKey(language) && !String.IsNullOrEmpty(((ArticlesLinked)myobject).Detail[language].AdditionalText) ? ((ArticlesLinked)myobject).Detail[language].AdditionalText : "Check out the latest News on the NOI Community App";
                notification.sound = "default";

                message.notification = notification;
            }

            return message;
        }

    }
}
