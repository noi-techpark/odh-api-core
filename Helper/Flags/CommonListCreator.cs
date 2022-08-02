using System;
using System.Collections.Generic;
using System.Globalization;

namespace Helper
{
    public class CommonListCreator
    {
        public static List<string> CreateIdList(string? activityidstring)
        {
            List<string> activityIds = new List<string>();

            if (!String.IsNullOrEmpty(activityidstring))
            {
                if (activityidstring.Substring(activityidstring.Length - 1, 1) == ",")
                    activityidstring = activityidstring.Substring(0, activityidstring.Length - 1);

                var splittedfilter = activityidstring.Split(',');

                foreach (var filter in splittedfilter)
                {
                    activityIds.Add(filter);
                }
            }

            return activityIds;
        }

        public static List<string> CreateSmgPoiSourceList(string? sourcestring)
        {
            List<string> activityIds = new List<string>();

            if (!String.IsNullOrEmpty(sourcestring))
            {
                if (sourcestring.ToLower() != "null")
                {
                    if (sourcestring.Substring(sourcestring.Length - 1, 1) == ",")
                        sourcestring = sourcestring.Substring(0, sourcestring.Length - 1);

                    var splittedfilter = sourcestring.Split(',');

                    foreach (var filter in splittedfilter)
                    {
                        activityIds.Add(filter.ToLower());
                    }
                }
            }

            return activityIds;
        }

        public static List<string> CreateLowerCaseSmgTagList(string? activityidstring)
        {
            List<string> activityIds = new List<string>();

            if (activityidstring != null)
            {
                if (activityidstring.Substring(activityidstring.Length - 1, 1) == ",")
                    activityidstring = activityidstring.Substring(0, activityidstring.Length - 1);

                var splittedfilter = activityidstring.Split(',');

                foreach (var filter in splittedfilter)
                {
                    activityIds.Add(filter);
                }
            }

            return activityIds;
        }

        public static List<string> CreateDistrictIdList(string? locfilter, string typ)
        {
            List<string> locIds = new List<string>();

            if (locfilter != null)
            {
                if (locfilter.Substring(locfilter.Length - 1, 1) == ",")
                    locfilter = locfilter.Substring(0, locfilter.Length - 1);

                var splittedfilter = locfilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    if (filter.StartsWith(typ))
                        locIds.Add(filter.Replace(typ, ""));
                }
            }

            return locIds;
        }

        public static (int min, int max) CreateRangeString(string? rangetoSplit)
        {
            if (rangetoSplit != null)
            {
                var splittedfilter = rangetoSplit.Split(',');

                return (Convert.ToInt32(splittedfilter[0]), Convert.ToInt32(splittedfilter[1]));
            }
            else
                return (0, 0);
        }

        public static (double, double) CreateRangeStringDouble(string? rangetoSplit)
        {
            NumberFormatInfo provider = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };

            if (rangetoSplit != null)
            {
                var splittedfilter = rangetoSplit.Split(',');

                return (Convert.ToDouble(splittedfilter[0], provider), Convert.ToDouble(splittedfilter[1], provider));
            }
            else
                return (0.0, 0.0);
        }

        public static List<string> CreateDifficultyList(string? difficultyfilter, string? activitypoitype)
        {
            List<string> difficultyids = new List<string>();

            if (difficultyfilter != null)
            {
                if (difficultyfilter.Substring(difficultyfilter.Length - 1, 1) == ",")
                    difficultyfilter = difficultyfilter.Substring(0, difficultyfilter.Length - 1);

                var splittedfilter = difficultyfilter.Split(',');

                switch (activitypoitype)
                {
                    case "Berg":
                    case "Radfahren":
                    case "Wandern":
                    case "Laufen und Fitness":

                        foreach (var filter in splittedfilter)
                        {
                            switch (filter)
                            {
                                case "1":
                                    difficultyids.Add("1");
                                    difficultyids.Add("2");
                                    break;

                                case "2":
                                    difficultyids.Add("3");
                                    difficultyids.Add("4");
                                    break;

                                case "3":
                                    difficultyids.Add("5");
                                    difficultyids.Add("6");
                                    break;
                            }
                        }

                        break;

                    case "Rodelbahnen":
                        //hot koan Schwierigkeit
                        break;

                    case "Aufstiegsanlagen":
                        //hot koan Schwierigkeit
                        break;

                    case "Piste":

                        foreach (var filter in splittedfilter)
                        {
                            switch (filter)
                            {
                                case "1":
                                    difficultyids.Add("2");
                                    break;

                                case "2":
                                    difficultyids.Add("4");
                                    break;

                                case "3":
                                    difficultyids.Add("6");
                                    break;
                            }
                        }

                        break;

                    case "Loipen":

                        foreach (var filter in splittedfilter)
                        {
                            switch (filter)
                            {
                                case "1":
                                    difficultyids.Add("2");
                                    break;

                                case "2":
                                    difficultyids.Add("4");
                                    break;

                                case "3":
                                    difficultyids.Add("6");
                                    break;
                            }
                        }

                        break;
                }


            }

            return difficultyids;
        }

        public static List<string> CreateDifficultyListfromFlag(string? difficultyfilter, string activitypoitype)
        {
            List<string> difficultyids = new List<string>();

            if (difficultyfilter != null)
            {
                if (difficultyfilter.Substring(difficultyfilter.Length - 1, 1) == ",")
                    difficultyfilter = difficultyfilter.Substring(0, difficultyfilter.Length - 1);

                var splittedfilter = difficultyfilter.Split(',');

                switch (activitypoitype)
                {
                    case "Berg":
                    case "Radfahren":
                    case "Wandern":
                    case "Laufen und Fitness":

                        foreach (var filter in splittedfilter)
                        {
                            switch (filter)
                            {
                                case "one":
                                    difficultyids.Add("leicht");
                                    break;

                                case "two":
                                    difficultyids.Add("wenig schwierig");
                                    break;

                                case "three":
                                    difficultyids.Add("mäßig schwierig");
                                    break;

                                case "four":
                                    difficultyids.Add("schwierig");
                                    break;

                                case "five":
                                    difficultyids.Add("sehr schwierig");
                                    break;
                            }
                        }

                        break;

                    case "Rodelbahnen":
                        //hot koan Schwierigkeit
                        break;

                    case "Aufstiegsanlagen":
                        //hot koan Schwierigkeit
                        break;

                    case "Piste":

                        foreach (var filter in splittedfilter)
                        {
                            switch (filter)
                            {
                                case "one":
                                    difficultyids.Add("blau");
                                    break;

                                case "two":
                                    difficultyids.Add("rot");
                                    break;

                                case "three":
                                    difficultyids.Add("schwarz");
                                    break;
                            }
                        }

                        break;

                    case "Loipen":

                        foreach (var filter in splittedfilter)
                        {
                            switch (filter)
                            {
                                case "one":
                                    difficultyids.Add("blau");
                                    break;

                                case "two":
                                    difficultyids.Add("gelb");
                                    break;

                                case "three":
                                    difficultyids.Add("rot");
                                    break;

                                case "four":
                                    difficultyids.Add("schwarz");
                                    break;
                            }
                        }

                        break;
                }


            }

            return difficultyids;
        }
    }
}
