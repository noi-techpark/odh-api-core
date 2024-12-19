// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class ArticleListCreator
    {
        public static List<string> CreateArticleTypeList(string typefilter)
        {
            List<string> typeids = new List<string>();

            if (typefilter != "null")
            {
                if (typefilter.Substring(typefilter.Length - 1, 1) == ",")
                    typefilter = typefilter.Substring(0, typefilter.Length - 1);

                var splittedfilter = typefilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    typeids.Add(filter);
                }
            }

            return typeids;
        }

        public static List<string> CreateArticleSubTypeList(
            string? articletype,
            string? subtypefilter
        )
        {
            List<string> typeids = new List<string>();

            if (subtypefilter != null && subtypefilter != "null")
            {
                if (subtypefilter.Substring(subtypefilter.Length - 1, 1) == ",")
                    subtypefilter = subtypefilter.Substring(0, subtypefilter.Length - 1);

                var splittedfilter = subtypefilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    switch (articletype)
                    {
                        case "basisartikel":

                            switch (filter)
                            {
                                case "1":
                                    typeids.Add("Suggestion");
                                    break;
                                case "2":
                                    typeids.Add("Hotel");
                                    break;
                                case "3":
                                    typeids.Add("Gastronomy");
                                    break;
                                case "4":
                                    typeids.Add("General");
                                    break;
                                case "5":
                                    typeids.Add("Tip");
                                    break;
                                case "6":
                                    typeids.Add("News");
                                    break;
                                case "7":
                                    typeids.Add("Detail");
                                    break;
                            }

                            break;

                        case "buchtippartikel":

                            switch (filter)
                            {
                                case "1":
                                    typeids.Add("Accommodation-Restaurants");
                                    break;
                                case "2":
                                    typeids.Add("Biking");
                                    break;
                                case "3":
                                    typeids.Add("Culture-History");
                                    break;
                                case "4":
                                    typeids.Add("Family");
                                    break;
                                case "5":
                                    typeids.Add("Gastronomy");
                                    break;
                                case "6":
                                    typeids.Add("Health");
                                    break;
                                case "7":
                                    typeids.Add("Novels-Poetry");
                                    break;
                                case "8":
                                    typeids.Add("Travel-guides");
                                    break;
                                case "9":
                                    typeids.Add("Trekking-guides");
                                    break;
                                case "10":
                                    typeids.Add("Winter");
                                    break;
                            }

                            break;
                        case "presseartikel":

                            switch (filter)
                            {
                                case "1":
                                    typeids.Add("allgemeinepresseartikel");
                                    break;
                                case "2":
                                    typeids.Add("pressemeetings");
                                    break;
                                case "3":
                                    typeids.Add("pressemitteilungen");
                                    break;
                                case "4":
                                    typeids.Add("pressethemenserviceartikel");
                                    break;
                            }

                            break;

                        case "b2bartikel":

                            switch (filter)
                            {
                                case "1":
                                    typeids.Add("B2BDetail");
                                    break;
                                case "2":
                                    typeids.Add("B2BProgrammTipp");
                                    break;
                                case "3":
                                    typeids.Add("B2BNeuigkeiten");
                                    break;
                                case "4":
                                    typeids.Add("B2BVeranstaltung");
                                    break;
                            }

                            break;

                        case "contentartikel":

                            switch (filter)
                            {
                                case "1":
                                    typeids.Add("Site Content");
                                    break;
                                case "2":
                                    typeids.Add("B2B Site Content");
                                    break;
                                case "3":
                                    typeids.Add("Press Site Content");
                                    break;
                            }

                            break;

                        case "veranstaltungsartikel":

                            switch (filter)
                            {
                                case "1":
                                    typeids.Add("Gastronomie");
                                    break;
                                case "2":
                                    typeids.Add("Treffen");
                                    break;
                                case "3":
                                    typeids.Add("Volksfest");
                                    break;
                                case "4":
                                    typeids.Add("Tradition");
                                    break;
                                case "5":
                                    typeids.Add("Kinder/Familie");
                                    break;
                                case "6":
                                    typeids.Add("Kultur");
                                    break;
                                case "7":
                                    typeids.Add("Kunsthandwerk");
                                    break;
                                case "8":
                                    typeids.Add("Theater");
                                    break;
                                case "9":
                                    typeids.Add("Wanderungen");
                                    break;
                                case "10":
                                    typeids.Add("Ausstellungen/Kunst");
                                    break;
                                case "11":
                                    typeids.Add("Sport");
                                    break;
                                case "12":
                                    typeids.Add("Messen");
                                    break;
                                case "13":
                                    typeids.Add("Musik");
                                    break;
                                case "14":
                                    typeids.Add("Geführte Touren");
                                    break;
                                case "15":
                                    typeids.Add("Unterhaltung");
                                    break;
                            }
                            break;
                    }
                }
            }

            return typeids;
        }

        //Activity Data

        public static List<string> CreateArticleTypefromFlag(string typefilter)
        {
            List<string> typelist = new List<string>();

            if (typefilter != "null")
            {
                int typefilterint = 0;
                if (int.TryParse(typefilter, out typefilterint))
                {
                    ArticleTypeFlag mypoitypeflag = (ArticleTypeFlag)typefilterint;

                    var myflags = mypoitypeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        typelist.Add(myflag);
                    }
                }
                else
                    return new List<string>();
            }

            return typelist;
        }

        public static List<string> CreateArticleSubTypefromFlag(
            string? typefiltertext,
            string? subtypefilter
        )
        {
            List<string> subtypelist = new List<string>();

            if (subtypefilter != "null")
            {
                long typefilterint = 0;
                if (long.TryParse(subtypefilter, out typefilterint))
                {
                    switch (typefiltertext)
                    {
                        case "basisartikel":
                            ArticleBasisArticleFlag mypoitypeflag1 =
                                (ArticleBasisArticleFlag)typefilterint;
                            subtypelist.AddRange(mypoitypeflag1.GetFlags().GetDescriptionList());

                            break;
                        case "buchtippartikel":
                            ArticleBuchTippFlag mypoitypeflag2 = (ArticleBuchTippFlag)typefilterint;
                            subtypelist.AddRange(mypoitypeflag2.GetFlags().GetDescriptionList());

                            break;
                        case "contentartikel":
                            ArticleContentArticleFlag mypoitypeflag3 =
                                (ArticleContentArticleFlag)typefilterint;
                            subtypelist.AddRange(mypoitypeflag3.GetFlags().GetDescriptionList());

                            break;
                        case "veranstaltungsartikel":
                            ArticleVeranstaltungsArticleFlag mypoitypeflag4 =
                                (ArticleVeranstaltungsArticleFlag)typefilterint;
                            subtypelist.AddRange(mypoitypeflag4.GetFlags().GetDescriptionList());

                            break;
                        case "presseartikel":
                            ArticlePresseArticleFlag mypoitypeflag5 =
                                (ArticlePresseArticleFlag)typefilterint;
                            subtypelist.AddRange(mypoitypeflag5.GetFlags().GetDescriptionList());

                            break;

                        //case "rezeptartikel":
                        //    ActivityTypeLaufenFitness mypoitypeflag6 = (ActivityTypeLaufenFitness)typefilterint;
                        //    subtypelist.AddRange(mypoitypeflag6.GetFlags().GetDescriptionList());

                        //    break;

                        //case "reiseveranstalter":
                        //    ActivityTypeLoipen mypoitypeflag7 = (ActivityTypeLoipen)typefilterint;
                        //    subtypelist.AddRange(mypoitypeflag7.GetFlags().GetDescriptionList());

                        //    break;

                        case "b2bartikel":
                            ArticleB2BArticleFlag mypoitypeflag8 =
                                (ArticleB2BArticleFlag)typefilterint;
                            subtypelist.AddRange(mypoitypeflag8.GetFlags().GetDescriptionList());

                            break;
                    }
                }
                else
                    return new List<string>();
            }

            return subtypelist;
        }
    }
}
