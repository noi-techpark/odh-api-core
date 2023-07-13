// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DSS;
using System;
using System.Collections.Generic;

namespace OdhApiImporter.Helpers.DSS
{
    public class DSSImportUtil
    {
        public static string GetSourceId(dynamic input, string entitytype)
        {       
            switch (entitytype.ToLower())
            {
                case "lift":
                    return (string)input.pid;
                case "slope":
                    return (string)input.pid;
                case "snowpark":
                    //pid
                    return (string)input.pid;
                case "alpinehut":
                    return (string)input.pid;
                case "skicircuit":
                    //no id there name + clockwise?
                    return (string)input.pid;
                case "sellingpoint":
                    return (string)input.pid;
                case "taxi":
                    return (string)input.pid;
                case "healthcare":
                    return (string)input.pid;
                case "skiresort":
                    return (string)input.pid;
                case "webcam":
                    //pid
                    return (string)input.pid;
                case "overview":
                    return (string)input.pid;
                case "weather":
                    //rrid
                    return (string)input.rrid;
                default:
                    return (string)input.pid;
            }
        }

        public static Tuple<DSSRequestType, bool> GetRequestTypeList(string entitytype)
        {
            switch (entitytype.ToLower())
            {
                case "lift":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.liftbase, false);                                                            
                case "slope":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.slopebase, false);                    
                case "snowpark":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.snowparks, true);                    
                case "alpinehut":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.alpinehuts, true);
                case "skicircuit":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.skicircuits, true);
                case "sellingpoint":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.sellingpoints, true);
                case "taxi":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.taxi, true);
                case "healthcare":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.healthcare, true);
                case "skiresort":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.skiresorts, true);
                case "webcam":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.webcams, true);
                case "overview":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.overview, true);
                case "weather":
                    return new Tuple<DSSRequestType, bool>(DSSRequestType.weather, true);
                default:
                    return null;
            }            
        }
    }
}
