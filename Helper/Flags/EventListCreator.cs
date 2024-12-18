// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class EventListCreator
    {
        public static List<string> CreateEventTopicRidList(string topicfilter)
        {
            List<string> topicids = new List<string>();

            if (!String.IsNullOrEmpty(topicfilter))
            {
                if (topicfilter != "null")
                {
                    if (topicfilter.Substring(topicfilter.Length - 1, 1) == ",")
                        topicfilter = topicfilter.Substring(0, topicfilter.Length - 1);

                    var splittedfilter = topicfilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {
                        switch (filter)
                        {
                            case "1":
                                topicids.Add("0D25868CC23242D6AC97AEB2973CB3D6");
                                break;
                            case "2":
                                topicids.Add("162C0067811B477DA725D2F5F2D98398");
                                break;
                            case "3":
                                topicids.Add("252200A028C8449D9A6205369A6D0D36");
                                break;
                            case "4":
                                topicids.Add("33BDC54BD39946F4852B3394B00610AE");
                                break;
                            case "5":
                                topicids.Add("4C4961D9FC5B48EEB73067BEB9D4402A");
                                break;
                            case "6":
                                topicids.Add("6884FE362C88434B9F49725E3328112B");
                                break;
                            case "7":
                                topicids.Add("767F6F43FC394CE9A3C8A9725C6FF134");
                                break;
                            case "8":
                                topicids.Add("7E048074BA004EC58E29E330A9AA476B");
                                break;
                            case "9":
                                topicids.Add("9C3449EE278C4D94AA5A7C286729DEA0");
                                break;
                            case "10":
                                topicids.Add("ACE8B613F2074A7BB59C0B1DD40A43CD");
                                break;
                            case "11":
                                topicids.Add("B5467FEFE5C74FA5AD32B83793A76165");
                                break;
                            case "12":
                                topicids.Add("C72CE969B98947FABC99CBC7B033F28E");
                                break;
                            case "13":
                                topicids.Add("D98B49DF24C342D09A8161836435CF86");
                                break;
                        }
                    }
                }
            }
            return topicids;
        }

        public static List<string> CreateEventTopicRidListfromFlag(string? topicfilter)
        {
            List<string> topicids = new List<string>();

            if (!String.IsNullOrEmpty(topicfilter))
            {
                if (topicfilter != "null")
                {
                    var topicfilterint = Convert.ToInt64(topicfilter);

                    EventTopicFlag mytopicflag = (EventTopicFlag)topicfilterint;

                    var myflags = mytopicflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        topicids.Add(myflag);
                    }
                }
            }

            return topicids;
        }
    }
}
