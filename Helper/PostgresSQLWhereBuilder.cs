using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class PostgresSQLWhereBuilder
    {
        public static void CheckPassedLanguage(ref string language, List<string> availablelanguages)
        {
            language = language.ToLower();

            if (!availablelanguages.Contains(language))
                throw new Exception("passed language not available or passed incorrect string");
        }

        //Return where and Parameters
        public static Tuple<string, List<PGParameters>> CreateIdListWhereExpression(List<string> idlist)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (idlist.Count > 0)
            {
                //Tuning force to use GIN Index
                if (idlist.Count == 1)
                {
                    //whereexpression = whereexpression + "data @> '{\"Id\" : \"" + idlist.FirstOrDefault() + "\" }'";
                    whereexpression = whereexpression + "id LIKE @id";
                    parameters.Add(new PGParameters() { Name = "id", Type = NpgsqlTypes.NpgsqlDbType.Text, Value = idlist.FirstOrDefault() });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var accoid in idlist)
                    {
                        idliststring = idliststring + "@id" + counter + ", ";
                        parameters.Add(new PGParameters() { Name = "id" + counter, Type = NpgsqlTypes.NpgsqlDbType.Text, Value = accoid });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
                }
            }


            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return where and Parameters
        public static Tuple<string, List<PGParameters>> CreateIdListWhereExpression(string? id)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (!String.IsNullOrEmpty(id))
            {
                whereexpression = whereexpression + "id LIKE @id";
                parameters.Add(new PGParameters() { Name = "id", Type = NpgsqlTypes.NpgsqlDbType.Text, Value = id });
            }

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return where and Parameters
        public static Tuple<string, List<PGParameters>> CreateIdListWhereExpressionCaseInsensitive(string id)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (!String.IsNullOrEmpty(id))
            {
                whereexpression = whereexpression + "id ILIKE @id";
                parameters.Add(new PGParameters() { Name = "id", Type = NpgsqlTypes.NpgsqlDbType.Text, Value = id });
            }

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return where and PArameters
        public static Tuple<string, List<PGParameters>> CreateIdListWhereExpression(List<string> idlist, bool insertdummyonemptyidlist)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (idlist.Count > 0)
            {
                //Tuning force to use GIN Index
                if (idlist.Count == 1)
                {
                    whereexpression = whereexpression + "Id = @id";
                    parameters.Add(new PGParameters() { Name = "id", Type = NpgsqlTypes.NpgsqlDbType.Text, Value = idlist.FirstOrDefault() });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;

                    foreach (var id in idlist)
                    {
                        idliststring = idliststring + "@id" + counter + ", ";
                        parameters.Add(new PGParameters() { Name = "id" + counter, Type = NpgsqlTypes.NpgsqlDbType.Text, Value = id });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression = whereexpression + "Id in (" + idliststring + ")";
                }
            }
            else if (idlist.Count == 0)
            {
                if (insertdummyonemptyidlist)
                {
                    whereexpression = "Id = @dummy";
                    parameters.Add(new PGParameters() { Name = "dummy", Type = NpgsqlTypes.NpgsqlDbType.Text, Value = "00000000" });
                }

            }


            return new Tuple<string, List<PGParameters>>(whereexpression, parameters); ;
        }

        //Return where and Parameters
        public static Tuple<string, List<PGParameters>> CreateLastChangedWhereExpression(string updatefrom)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (!String.IsNullOrEmpty(updatefrom))
            {
                whereexpression = whereexpression + "to_date(data ->> 'LastChange', 'YYYY-MM-DD') > @date";
                parameters.Add(new PGParameters() { Name = "date", Type = NpgsqlTypes.NpgsqlDbType.Date, Value = updatefrom });
            }

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return Where and Parameters for MetaRegion Query
        public static Tuple<string, List<PGParameters>> CreateMetaRegionWhereExpression(List<string> metaregionlist)
        {
            string whereexpression = "";
            string jsonexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (metaregionlist.Count > 0)
            {
                //Tuning force to use GIN Index
                if (metaregionlist.Count == 1)
                {
                    whereexpression = "data @> @jsonexpression";
                    jsonexpression = "{\"Id\" : \"" + metaregionlist.FirstOrDefault().ToUpper() + "\" }";

                    parameters.Add(new PGParameters() { Name = "jsonexpression", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = jsonexpression });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var mtaid in metaregionlist)
                    {
                        idliststring = idliststring + "@mtaid" + counter + ",";
                        parameters.Add(new PGParameters() { Name = "mtaid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "\"" + mtaid.ToUpper() + "\"" });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 1);

                    whereexpression = whereexpression + "data -> 'Id' IN (" + idliststring + ")";
                }
            }


            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return Where and Parameters for Activity
        public static Tuple<string, List<PGParameters>> CreateActivityWhereExpression(List<string> idlist, List<string> activitytypelist, List<string> subtypelist, List<string> difficultylist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, List<string> arealist, bool distance, int distancemin, int distancemax, bool duration, int durationmin, int durationmax, bool altitude, int altitudemin, int altitudemax, bool? highlight, bool? activefilter, bool? smgactivefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdUpperFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);
            AreaFilterWhere(ref whereexpression, parameters, arealist);
            ActivityTypeFilterWhere(ref whereexpression, parameters, activitytypelist);
            ActivitySubTypeFilterWhere(ref whereexpression, parameters, subtypelist);
            DifficultyFilterWhere(ref whereexpression, parameters, difficultylist);
            DistanceFilterWhere(ref whereexpression, parameters, distance, distancemin, distancemax);
            DurationFilterWhere(ref whereexpression, parameters, duration, durationmin, durationmax);
            AltitudeFilterWhere(ref whereexpression, parameters, altitude, altitudemin, altitudemax);
            HighlightFilterWhere(ref whereexpression, parameters, highlight);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);
            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Returns Where and Parameters for Poi
        public static Tuple<string, List<PGParameters>> CreatePoiWhereExpression(List<string> idlist, List<string> poitypelist, List<string> subtypelist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, List<string> arealist, bool? highlight, bool? activefilter, bool? smgactivefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdUpperFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);
            AreaFilterWhere(ref whereexpression, parameters, arealist);
            PoiTypeFilterWhere(ref whereexpression, parameters, poitypelist);
            PoiSubTypeFilterWhere(ref whereexpression, parameters, subtypelist);
            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            HighlightFilterWhere(ref whereexpression, parameters, highlight);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return Where and Parameters for Events
        public static Tuple<string, List<PGParameters>> CreateEventWhereExpression(List<string> idlist, List<string> orgidlist, List<string> rancidlist, List<string> typeidlist, List<string> topicrids, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, Nullable<DateTime> begin, Nullable<DateTime> end, bool? activefilter, bool? smgactivefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdUpperFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);
            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);

            EventTopicFilterWhere(ref whereexpression, parameters, topicrids);
            EventOrganizerFilterWhere(ref whereexpression, parameters, orgidlist);

            EventRancFilterWhere(ref whereexpression, parameters, rancidlist);
            EventTypeFilterWhere(ref whereexpression, parameters, typeidlist);
            //EventDateFilterWhere(ref whereexpression, parameters, begin, end);

            EventDateFilterWhereTest(ref whereexpression, parameters, begin, end);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return Where and Parameters for SmgPoi
        public static Tuple<string, List<PGParameters>> CreateSmgPoiWhereExpression(List<string> idlist, List<string> typelist, List<string> subtypelist, List<string> poitypelist, List<string> languagelist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, List<string> arealist, List<string> sourcelist, bool? highlight, bool? activefilter, bool? smgactivefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdLowerFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);
            AreaFilterWhere(ref whereexpression, parameters, arealist);
            SmgPoiTypeFilterWhere(ref whereexpression, parameters, typelist);
            SmgPoiSubTypeFilterWhere(ref whereexpression, parameters, subtypelist);
            SmgPoiPoiTypeFilterWhere(ref whereexpression, parameters, poitypelist);
            HasLanguageFilterWhere(ref whereexpression, parameters, languagelist);
            SourceFilterWhere(ref whereexpression, parameters, sourcelist);
            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            HighlightFilterWhere(ref whereexpression, parameters, highlight);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return Where and Parameters for Gastronomy
        public static Tuple<string, List<PGParameters>> CreateGastroWhereExpression(List<string> idlist, List<string> dishcodesids, List<string> ceremonycodesids, List<string> categorycodesids, List<string> facilitycodesids, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, bool? activefilter, bool? smgactivefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdUpperFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);

            CeremonyCodeFilterWhere(ref whereexpression, parameters, ceremonycodesids);
            CategoryCodeFilterWhere(ref whereexpression, parameters, categorycodesids);
            CuisineCodeFilterWhere(ref whereexpression, parameters, facilitycodesids);
            DishCodeFilterWhere(ref whereexpression, parameters, dishcodesids);

            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return Where and Parameters for Accommodation
        public static Tuple<string, List<PGParameters>> CreateAccoWhereExpression(List<string> idlist, List<string> accotypelist, bool apartmentfilter, List<string> categorylist, Dictionary<string, bool> featurelist, List<string> badgelist, Dictionary<string, bool> themelist, List<string> boardlist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, bool? activefilter, bool? smgactivefilter, bool? bookable, bool altitude, int altitudemin, int altitudemax)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdUpperFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);

            BoardFilter(ref whereexpression, parameters, boardlist);
            BadgeFilter(ref whereexpression, parameters, badgelist);
            CategoryFilter(ref whereexpression, parameters, categorylist);
            ThemeFilter(ref whereexpression, parameters, themelist);
            FeatureFilter(ref whereexpression, parameters, featurelist);
            AccoTypeFilter(ref whereexpression, parameters, accotypelist);
            ApartmentFilter(ref whereexpression, parameters, apartmentfilter);
            BookableFilter(ref whereexpression, parameters, bookable);
            AltitudeFilter(ref whereexpression, parameters, altitude, altitudemin, altitudemax);

            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Return Where and Parameters for Activity
        public static Tuple<string, List<PGParameters>> CreateArticleWhereExpression(List<string> idlist, List<string> typelist, List<string> subtypelist, List<string> smgtaglist, List<string> languagelist, bool? highlight, bool? activefilter, bool? smgactivefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdUpperFilterWhere(ref whereexpression, parameters, idlist);
            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);
            HasLanguageFilterWhere(ref whereexpression, parameters, languagelist);
            HighlightFilterWhere(ref whereexpression, parameters, highlight);

            ArticleTypeFilterWhere(ref whereexpression, parameters, typelist);
            ArticleSubTypeFilterWhere(ref whereexpression, parameters, subtypelist);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //Returns Where and Parameter for EventShort
        public static Tuple<string, List<PGParameters>> CreateEventShortWhereExpression(DateTime start, DateTime end, string source, string eventlocation, string activefilter, List<string> eventidlist, bool special)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdLowerFilterWhere(ref whereexpression, parameters, eventidlist);

            if (special)
                EventShortBeginEndSpecialFilterWhere(ref whereexpression, parameters, start, end);
            else
                EventShortBeginEndFilterWhere(ref whereexpression, parameters, start, end);

            EventShortActiveFilterWhere(ref whereexpression, parameters, activefilter);
            EventShortSourceFilterWhere(ref whereexpression, parameters, source);
            EventShortEventLocationFilterWhere(ref whereexpression, parameters, eventlocation.ToUpper());

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        //REturns Where and Parameter for Packages
        public static Tuple<string, List<PGParameters>> CreatePackageWhereExpression(List<string> idlist, List<string> accolist, List<string> boardlist, List<string> themelist, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, DateTime validfrom, DateTime validto, bool longstay, bool shortstay, bool? activefilter, bool? smgactivefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdUpperFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);

            PackagesAccoFilterWhere(ref whereexpression, parameters, accolist);
            PackagesBoardFilterWhere(ref whereexpression, parameters, boardlist);
            PackagesStayFilterWhere(ref whereexpression, parameters, shortstay, longstay);
            PackagesThemeFilterWhere(ref whereexpression, parameters, themelist);
            PackagesValidFromFilterWhere(ref whereexpression, parameters, validfrom, validto);

            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }


        #region Reusable Where Builders

        #region Reusable Where Builders Mobile

        public static Tuple<string, List<PGParameters>> CreateSmgPoiMobileWhereExpression(List<string> idlist, List<string> typelist, List<string> subtypelist, List<string> poitypelist, List<string> difficultylist, List<string> smgtaglist, List<string> languagelist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, bool distance, int distancemin, int distancemax, bool duration, double durationmin, double durationmax, bool altitude, int altitudemin, int altitudemax, bool? highlight, bool? activefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdLowerFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);

            DurationFilterWhere(ref whereexpression, parameters, duration, durationmin, durationmax);
            AltitudeFilterWhere(ref whereexpression, parameters, altitude, altitudemin, altitudemax);
            DistanceFilterWhere(ref whereexpression, parameters, distance, distancemin, distancemax);
            DifficultyFilterWhere(ref whereexpression, parameters, difficultylist);
            HasLanguageFilterWhere(ref whereexpression, parameters, languagelist);

            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            HighlightFilterWhere(ref whereexpression, parameters, highlight);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);

            //Typelist, subtypelist and Poitypelist only from smgtags
            PoiTypeFilterWhere(ref whereexpression, parameters, typelist);
            PoiSubTypeFilterWhere(ref whereexpression, parameters, subtypelist);
            SmgPoiPoiTypeFilterWhere(ref whereexpression, parameters, poitypelist);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        public static Tuple<string, List<PGParameters>> CreateEventMobileWhereExpression(List<string> idlist, List<string> orgidlist, List<string> rancidlist, List<string> typeidlist, List<string> topicrids, List<string> smgtaglist, List<string> districtlist, List<string> municipalitylist, List<string> tourismvereinlist, List<string> regionlist, Nullable<DateTime> begin, Nullable<DateTime> end, bool fromnow, bool? activefilter, bool? smgactivefilter, string languagefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdUpperFilterWhere(ref whereexpression, parameters, idlist);
            DistrictWhere(ref whereexpression, parameters, districtlist);
            LocFilterMunicipalityWhere(ref whereexpression, parameters, municipalitylist);
            LocFilterTvsWhere(ref whereexpression, parameters, tourismvereinlist);
            LocFilterRegionWhere(ref whereexpression, parameters, regionlist);
            SmgTagFilterWhere(ref whereexpression, parameters, smgtaglist);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);
            SmgActiveFilterWhere(ref whereexpression, parameters, smgactivefilter);

            EventTopicFilterWhere(ref whereexpression, parameters, topicrids);
            EventOrganizerFilterWhere(ref whereexpression, parameters, orgidlist);
            EventRancFilterWhere(ref whereexpression, parameters, rancidlist);

            EventTypeFilterWhere(ref whereexpression, parameters, typeidlist);
            EventFromNowFilter(ref whereexpression, parameters, fromnow);

            HasLanguageFilterWhere(ref whereexpression, parameters, new List<string>() { languagefilter });

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        public static Tuple<string, List<PGParameters>> CreateTipsMobileWhereExpression(List<string> typestoexclude, List<string> subtypestoexclude, List<string> poitypestoexclude, List<string> languagelist, bool? highlight, bool? activefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            HasLanguageFilterWhere(ref whereexpression, parameters, languagelist);
            HighlightFilterWhere(ref whereexpression, parameters, highlight);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);

            TypesToExcludeFilter(ref whereexpression, parameters, typestoexclude);
            SubTypesToExcludeFilter(ref whereexpression, parameters, subtypestoexclude);
            PoiTypesToExcludeFilter(ref whereexpression, parameters, poitypestoexclude);

            return new Tuple<string, List<PGParameters>>(whereexpression, parameters);
        }

        #endregion

        private static void IdUpperFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> idlist)
        {
            //IDLIST
            if (idlist.Count > 0)
            {
                if (idlist.Count == 1)
                {
                    whereexpression = whereexpression + "id LIKE @id";
                    parameters.Add(new PGParameters() { Name = "id", Type = NpgsqlTypes.NpgsqlDbType.Text, Value = idlist.FirstOrDefault().ToUpper() });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var activityid in idlist)
                    {
                        idliststring = idliststring + "@id" + counter + ", ";
                        parameters.Add(new PGParameters() { Name = "id" + counter, Type = NpgsqlTypes.NpgsqlDbType.Text, Value = activityid.ToUpper() });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression = whereexpression + "id in (" + idliststring + ")";
                }
            }
        }

        private static void IdLowerFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> idlist)
        {
            //IDLIST
            if (idlist.Count > 0)
            {
                if (idlist.Count == 1)
                {
                    whereexpression = whereexpression + "id LIKE @id";
                    parameters.Add(new PGParameters() { Name = "id", Type = NpgsqlTypes.NpgsqlDbType.Text, Value = idlist.FirstOrDefault().ToLower() });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var activityid in idlist)
                    {
                        idliststring = idliststring + "@id" + counter + ", ";
                        parameters.Add(new PGParameters() { Name = "id" + counter, Type = NpgsqlTypes.NpgsqlDbType.Text, Value = activityid.ToLower() });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression = whereexpression + "id in (" + idliststring + ")";
                }
            }
        }

        private static void DistrictWhere(ref string whereexpression, List<PGParameters> parameters, List<string> districtlist)
        {
            //DISTRICT
            if (districtlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                //Tuning force to use GIN Index
                if (districtlist.Count == 1)
                {
                    whereexpression = whereexpression + "data @> @districtid";
                    parameters.Add(new PGParameters() { Name = "districtid", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"DistrictId\" : \"" + districtlist.FirstOrDefault().ToUpper() + "\" }" });
                }
                else
                {
                    string districtliststring = "";
                    int counter = 1;
                    foreach (var distid in districtlist)
                    {
                        districtliststring = districtliststring + "@districtid" + counter + ",";
                        parameters.Add(new PGParameters() { Name = "districtid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "\"" + distid.ToUpper() + "\"" });
                        counter++;
                    }
                    districtliststring = districtliststring.Remove(districtliststring.Length - 1);

                    whereexpression = whereexpression + "data->'DistrictId' IN (" + districtliststring + ")";
                }
            }

        }

        private static void LocFilterDistrictWhere(ref string whereexpression, List<PGParameters> parameters, List<string> districtlist)
        {
            //MUNICIPALITY
            if (districtlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                //Tuning force to use GIN Index
                if (districtlist.Count == 1)
                {
                    whereexpression = whereexpression + "data @> @districtid";
                    parameters.Add(new PGParameters() { Name = "districtid", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"LocationInfo\" : { \"DistrictInfo\": { \"Id\": \"" + districtlist.FirstOrDefault().ToUpper() + "\" } } }" });
                }
                else
                {
                    string districtliststring = "";
                    int counter = 1;
                    foreach (var distid in districtlist)
                    {
                        districtliststring = districtliststring + "@districtid" + counter + ",";
                        parameters.Add(new PGParameters() { Name = "districtid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "\"" + distid.ToUpper() + "\"" });
                        counter++;
                    }
                    districtliststring = districtliststring.Remove(districtliststring.Length - 1);

                    whereexpression = whereexpression + "data->'LocationInfo'-> 'DistrictInfo' -> 'Id' in (" + districtliststring + ")";
                }
            }

        }

        private static void LocFilterMunicipalityWhere(ref string whereexpression, List<PGParameters> parameters, List<string> municipalitylist)
        {
            //MUNICIPALITY
            if (municipalitylist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                //Tuning force to use GIN Index
                if (municipalitylist.Count == 1)
                {
                    whereexpression = whereexpression + "data @> @municipalityid";
                    parameters.Add(new PGParameters() { Name = "municipalityid", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault().ToUpper() + "\" } } }" });
                }
                else
                {
                    string municipalityliststring = "";
                    int counter = 1;
                    foreach (var munid in municipalitylist)
                    {
                        municipalityliststring = municipalityliststring + "@municipalityid" + counter + ",";
                        parameters.Add(new PGParameters() { Name = "municipalityid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "\"" + munid.ToUpper() + "\"" });
                        counter++;
                    }
                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 1);

                    whereexpression = whereexpression + "data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in (" + municipalityliststring + ")";
                }
            }

        }

        private static void LocFilterTvsWhere(ref string whereexpression, List<PGParameters> parameters, List<string> tourismvereinlist)
        {
            //TOURISMVEREIN
            if (tourismvereinlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                //Tuning force to use GIN Index
                if (tourismvereinlist.Count == 1)
                {
                    whereexpression = whereexpression + "data @> @tourismvereinid";
                    parameters.Add(new PGParameters() { Name = "tourismvereinid", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"LocationInfo\" : { \"TvInfo\": { \"Id\": \"" + tourismvereinlist.FirstOrDefault().ToUpper() + "\" } } }" });
                }
                else
                {
                    string tvliststring = "";
                    int counter = 1;
                    foreach (var tvid in tourismvereinlist)
                    {
                        tvliststring = tvliststring + "@tourismvereinid" + counter + ",";
                        parameters.Add(new PGParameters() { Name = "tourismvereinid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "\"" + tvid.ToUpper() + "\"" });
                        counter++;
                    }
                    tvliststring = tvliststring.Remove(tvliststring.Length - 1);

                    whereexpression = whereexpression + "data->'LocationInfo'-> 'TvInfo' -> 'Id' in (" + tvliststring + ")";
                }
            }

        }

        private static void LocFilterRegionWhere(ref string whereexpression, List<PGParameters> parameters, List<string> regionlist)
        {
            //REGION TODO
            if (regionlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                //Tuning force to use GIN Index
                if (regionlist.Count == 1)
                {
                    whereexpression = whereexpression + "data @> @regionid";
                    parameters.Add(new PGParameters() { Name = "regionid", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"" + regionlist.FirstOrDefault() + "\" } } }" });
                }
                else
                {

                    string regionliststring = "";
                    int counter = 1;
                    foreach (var regid in regionlist)
                    {
                        regionliststring = regionliststring + "@regionid" + counter + ",";
                        parameters.Add(new PGParameters() { Name = "regionid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "\"" + regid.ToUpper() + "\"" });
                        counter++;
                    }
                    regionliststring = regionliststring.Remove(regionliststring.Length - 1);

                    whereexpression = whereexpression + "data->'LocationInfo'-> 'RegionInfo' -> 'Id' in (" + regionliststring + ")";
                }
            }

        }

        private static void AreaFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> arealist)
        {
            //AREA
            if (arealist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string arealiststring = "";
                int counter = 1;
                foreach (var area in arealist)
                {
                    arealiststring = arealiststring + "data @> @area" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "area" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"AreaId\": [\"" + area + "\"]}" });
                    counter++;
                }
                arealiststring = arealiststring.Remove(arealiststring.Length - 4);

                whereexpression = whereexpression + arealiststring + ")";
            }
        }

        private static void HighlightFilterWhere(ref string whereexpression, List<PGParameters> parameters, bool? highlight)
        {
            //Highlight
            if (highlight != null)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @highlight";
                parameters.Add(new PGParameters() { Name = "highlight", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Highlight\" : " + highlight.ToString().ToLower() + "}" });
            }
        }

        private static void ActiveFilterWhere(ref string whereexpression, List<PGParameters> parameters, bool? activefilter)
        {
            //Active
            if (activefilter != null)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @active";
                parameters.Add(new PGParameters() { Name = "active", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Active\" : " + activefilter.ToString().ToLower() + "}" });
            }
        }

        private static void SmgActiveFilterWhere(ref string whereexpression, List<PGParameters> parameters, bool? smgactivefilter)
        {
            //SmgActive
            if (smgactivefilter != null)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @smgactive";
                parameters.Add(new PGParameters() { Name = "smgactive", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SmgActive\" : " + smgactivefilter.ToString().ToLower() + "}" });
            }
        }

        private static void DistanceFilterWhere(ref string whereexpression, List<PGParameters> parameters, bool distance, int distancemin, int distancemax)
        {
            //DISTANCE
            if (distance)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "(data ->> 'DistanceLength')::numeric > @distancemin AND (data ->> 'DistanceLength')::numeric < @distancemax";
                parameters.Add(new PGParameters() { Name = "distancemin", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = distancemin.ToString() });
                parameters.Add(new PGParameters() { Name = "distancemax", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = distancemax.ToString() });
            }
        }

        private static void DurationFilterWhere(ref string whereexpression, List<PGParameters> parameters, bool duration, int durationmin, int durationmax)
        {
            //DURATION
            if (duration)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "(data ->> 'DistanceDuration')::numeric > @durationmin AND (data ->> 'DistanceDuration')::numeric < @durationmax";
                parameters.Add(new PGParameters() { Name = "durationmin", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = durationmin.ToString() });
                parameters.Add(new PGParameters() { Name = "durationmax", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = durationmax.ToString() });
            }
        }

        private static void DurationFilterWhere(ref string whereexpression, List<PGParameters> parameters, bool duration, double durationmin, double durationmax)
        {
            //DURATION
            if (duration)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "(data ->> 'DistanceDuration')::numeric > @durationmin AND (data ->> 'DistanceDuration')::numeric < @durationmax";
                parameters.Add(new PGParameters() { Name = "durationmin", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = durationmin.ToString() });
                parameters.Add(new PGParameters() { Name = "durationmax", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = durationmax.ToString() });
            }
        }

        private static void AltitudeFilterWhere(ref string whereexpression, List<PGParameters> parameters, bool altitude, int altitudemin, int altitudemax)
        {
            //ALTITUDE
            if (altitude)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "(data ->> 'AltitudeDifference')::numeric > @altitudemin AND (data ->> 'AltitudeDifference')::numeric < @altitudemax";
                parameters.Add(new PGParameters() { Name = "altitudemin", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = altitudemin.ToString() });
                parameters.Add(new PGParameters() { Name = "altitudemax", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = altitudemax.ToString() });
            }

        }

        private static void SmgTagFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> smgtaglist)
        {
            //SmgTags Info
            if (smgtaglist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string smgtagliststring = "";
                int counter = 1;

                foreach (var smgtag in smgtaglist)
                {
                    smgtagliststring = smgtagliststring + "data @> @smgtag" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "smgtag" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}" });
                    counter++;
                }
                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

                whereexpression = whereexpression + smgtagliststring + ")";
            }
        }

        private static void ActivityTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> activitytypelist)
        {
            //Activity Type
            if (activitytypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var activitytypeId in activitytypelist)
                {
                    activitytypestring = activitytypestring + "data @> @type" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "type" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Type\": \"" + activitytypeId + "\"}" });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        private static void ActivitySubTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> subtypelist)
        {
            //Activity Sub Type            
            if (subtypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string smgtagliststring = "";
                int counter = 1;

                foreach (var smgtag in subtypelist)
                {
                    smgtagliststring = smgtagliststring + "data @> @subtype" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "subtype" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SmgTags\": [\"" + smgtag.ToLower() + "\"]}" });
                    counter++;
                }
                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

                whereexpression = whereexpression + smgtagliststring + ")";
            }
        }

        private static void ArticleTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> articletypelist)
        {
            //Activity Type
            if (articletypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string articletypestring = "";
                int counter = 1;

                foreach (var articletypeId in articletypelist)
                {
                    articletypestring = articletypestring + "data @> @type" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "type" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Type\": \"" + articletypeId + "\"}" });
                    counter++;
                }
                articletypestring = articletypestring.Remove(articletypestring.Length - 4);

                whereexpression = whereexpression + articletypestring + ")";
            }
        }

        private static void ArticleSubTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> articlesubtypelist)
        {
            //Activity Type
            if (articlesubtypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string articlesubtypestring = "";
                int counter = 1;

                foreach (var articlesubtypeId in articlesubtypelist)
                {
                    articlesubtypestring = articlesubtypestring + "data @> @subtype" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "subtype" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SubType\": \"" + articlesubtypeId + "\"}" });
                    counter++;
                }
                articlesubtypestring = articlesubtypestring.Remove(articlesubtypestring.Length - 4);

                whereexpression = whereexpression + articlesubtypestring + ")";
            }
        }

        private static void DifficultyFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> difficultylist)
        {
            //Difficulty
            if (difficultylist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string difficultystring = "";
                int counter = 1;

                foreach (var difficultyId in difficultylist)
                {
                    difficultystring = difficultystring + "data @> @difficulty" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "difficulty" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Difficulty\": \"" + difficultyId + "\"}" });
                    counter++;
                }
                difficultystring = difficultystring.Remove(difficultystring.Length - 4);

                whereexpression = whereexpression + difficultystring + ")";
            }
        }

        private static void HasLanguageFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> haslanguage)
        {
            //SmgTags Info
            if (haslanguage.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string langliststring = "";
                int counter = 1;

                foreach (var language in haslanguage)
                {
                    langliststring = langliststring + "data @> @haslang" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "haslang" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"HasLanguage\": [\"" + language.ToLower() + "\"]}" });
                    counter++;
                }
                langliststring = langliststring.Remove(langliststring.Length - 4);

                whereexpression = whereexpression + langliststring + ")";
            }
        }

        private static void PoiTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> poitypelist)
        {
            //Activity Type
            if (poitypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var poitype in poitypelist)
                {
                    activitytypestring = activitytypestring + "data @> @type" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "type" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SmgTags\": [\"" + poitype.ToLower() + "\"]}" });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        private static void PoiSubTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> subtypelist)
        {
            //Activity Type
            if (subtypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var poitype in subtypelist)
                {
                    activitytypestring = activitytypestring + "data @> @subtype" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "subtype" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SmgTags\": [\"" + poitype.ToLower() + "\"]}" });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        private static void EventTopicFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> topicrids)
        {
            if (topicrids.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string topicliststring = "";
                int counter = 1;

                foreach (var topic in topicrids)
                {
                    topicliststring = topicliststring + "data @> @topic" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "topic" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"TopicRIDs\": [\"" + topic + "\"]}" });
                    counter++;
                }
                topicliststring = topicliststring.Remove(topicliststring.Length - 4);

                whereexpression = whereexpression + topicliststring + ")";
            }
        }

        private static void EventOrganizerFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> orgidlist)
        {
            //OrgIdList
            if (orgidlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string orgidliststring = "";
                int counter = 1;

                foreach (var orgid in orgidlist)
                {
                    orgidliststring = orgidliststring + "data @> @orgid" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "orgid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"OrgRID\": [\"" + orgid + "\"]}" });
                    counter++;
                }
                orgidliststring = orgidliststring.Remove(orgidliststring.Length - 4);

                whereexpression = whereexpression + orgidliststring + ")";
            }
        }

        private static void EventRancFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> rancidlist)
        {
            //RancIdList
            if (rancidlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string rancidliststring = "";
                int counter = 1;

                foreach (var rancid in rancidlist)
                {
                    rancidliststring = rancidliststring + "data @> @rancid" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "rancid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Ranc\": [\"" + rancid + "\"]}" });
                    counter++;
                }
                rancidliststring = rancidliststring.Remove(rancidliststring.Length - 4);

                whereexpression = whereexpression + rancidliststring + ")";
            }
        }

        private static void EventTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> typeidlist)
        {
            //TypeIdList
            if (typeidlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string typeidliststring = "";
                int counter = 1;

                foreach (var typeid in typeidlist)
                {
                    typeidliststring = typeidliststring + "data @> @typeid" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "typeid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Type\": [\"" + typeid + "\"]}" });
                    counter++;
                }
                typeidliststring = typeidliststring.Remove(typeidliststring.Length - 4);

                whereexpression = whereexpression + typeidliststring + ")";
            }
        }

        private static void EventDateFilterWhere(ref string whereexpression, List<PGParameters> parameters, DateTime? begin, DateTime? end)
        {
            //Begin & End
            if (begin != null && end != null)
            {
                //Beide nicht null
                if (begin != DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                }
                //Begin ist DateTime Min
                if (begin == DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') > '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') > '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                }
                //End ist DateTime Max
                if (begin != DateTime.MinValue && end == DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') < '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') < '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                }
                //APP USED THIS QUERY
                //else if (begin != DateTime.MinValue && end == DateTime.MaxValue)
                //{
                //    if (!String.IsNullOrEmpty(whereexpression))
                //        whereexpression = whereexpression + " AND ";

                //    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "')))";
                //}
            }
        }

        private static void EventDateFilterWhereTest(ref string whereexpression, List<PGParameters> parameters, DateTime? begin, DateTime? end)
        {
            //Begin & End
            if (begin != null && end != null)
            {
                //Beide nicht null
                if (begin != DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND (";
                    else
                        whereexpression = whereexpression + "(";

                    int counter = 1;
                    string datequerystring = "";

                    //GET every date in this period
                    for (DateTime loopdate = (DateTime)begin; loopdate <= end; loopdate = loopdate.AddDays(1))
                    {
                        datequerystring = datequerystring + "data @> @datebegin" + counter + " OR data @> @dateend" + counter + " OR ";
                        parameters.Add(new PGParameters() { Name = "datebegin" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"DateBegin\": \"" + String.Format("{0:yyyy-MM-dd}", loopdate) + "T00:00:00" + "\"}" });
                        parameters.Add(new PGParameters() { Name = "dateend" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"DateEnd\": \"" + String.Format("{0:yyyy-MM-dd}", loopdate) + "T00:00:00" + "\"}" });

                        counter++;
                    }
                    datequerystring = datequerystring.Remove(datequerystring.Length - 4);

                    whereexpression = whereexpression + datequerystring + ")";
                }
                ////Begin ist DateTime Min
                //if (begin == DateTime.MinValue && end != DateTime.MaxValue)
                //{
                //    if (!String.IsNullOrEmpty(whereexpression))
                //        whereexpression = whereexpression + " AND ";

                //    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') > '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') > '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                //}
                ////End ist DateTime Max
                //if (begin != DateTime.MinValue && end == DateTime.MaxValue)
                //{
                //    if (!String.IsNullOrEmpty(whereexpression))
                //        whereexpression = whereexpression + " AND ";

                //    whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') < '" + String.Format("{0:yyyy-MM-dd}", end) + "')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') < '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                //}

            }
        }

        private static void EventFromNowFilter(ref string whereexpression, List<PGParameters> parameter, bool fromnow)
        {

            //From Now
            if (fromnow)
            {

                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", DateTime.Now) + "')))";
            }
        }

        private static void SmgPoiTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> typelist)
        {
            //Activity Type
            if (typelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var activitytypeId in typelist)
                {
                    activitytypestring = activitytypestring + "data @> @type" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "type" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Type\": \"" + activitytypeId + "\"}" });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        private static void SmgPoiSubTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> subtypelist)
        {
            //Activity Sub Type            
            if (subtypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var activitytypeId in subtypelist)
                {
                    activitytypestring = activitytypestring + "data @> @subtype" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "subtype" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SubType\": \"" + activitytypeId + "\"}" });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        private static void SmgPoiPoiTypeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> poitypelist)
        {
            //Activity Sub Type            
            if (poitypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var activitytypeId in poitypelist)
                {
                    activitytypestring = activitytypestring + "data @> @poitype" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "poitype" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"PoiType\": \"" + activitytypeId + "\"}" });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        private static void SourceFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> sourcelist)
        {
            //Source
            if (sourcelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string sourcestring = "";
                int counter = 1;

                foreach (var sourceid in sourcelist)
                {
                    sourcestring = sourcestring + "data @> @source" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "source" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"SyncSourceInterface\": \"" + sourceid + "\" }" });
                    counter++;
                }
                sourcestring = sourcestring.Remove(sourcestring.Length - 4);

                whereexpression = whereexpression + sourcestring + ")";
            }
        }

        private static void CategoryCodeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> categorycodesids)
        {
            //Category Code
            if (categorycodesids.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                int counter = 1;
                string categorycodeliststring = "";

                foreach (var categorycode in categorycodesids)
                {
                    categorycodeliststring = categorycodeliststring + "data @> @categorycode" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "categorycode" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"CategoryCodes\": [{ \"Id\": \"" + categorycode.ToUpper() + "\" }] }" });
                    counter++;
                }
                categorycodeliststring = categorycodeliststring.Remove(categorycodeliststring.Length - 4);

                whereexpression = whereexpression + categorycodeliststring + ")";

            }
        }

        private static void CeremonyCodeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> ceremonycodesids)
        {
            //Capacity Ceremony
            if (ceremonycodesids.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                int counter = 1;
                string ceremonycodeliststring = "";

                foreach (var ceremonycode in ceremonycodesids)
                {
                    ceremonycodeliststring = ceremonycodeliststring + "data @> @ceremonycode" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "ceremonycode" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"CapacityCeremony\": [{ \"Id\": \"" + ceremonycode.ToUpper() + "\" }] }" });
                    counter++;
                }
                ceremonycodeliststring = ceremonycodeliststring.Remove(ceremonycodeliststring.Length - 4);

                whereexpression = whereexpression + ceremonycodeliststring + ")";

            }
        }

        private static void CuisineCodeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> facilitycodesids)
        {
            //OrgIdList
            if (facilitycodesids.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                int counter = 1;
                string facilitycodeliststring = "";

                foreach (var facilitycode in facilitycodesids)
                {
                    facilitycodeliststring = facilitycodeliststring + "data @> @facilitycode" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "facilitycode" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"Facilities\": [{ \"Id\": \"" + facilitycode.ToUpper() + "\" }] }" });
                    counter++;
                }
                facilitycodeliststring = facilitycodeliststring.Remove(facilitycodeliststring.Length - 4);

                whereexpression = whereexpression + facilitycodeliststring + ")";
            }
        }

        private static void DishCodeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> dishcodesids)
        {
            //Dishcode Rids
            if (dishcodesids.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                int counter = 1;
                string dishcodeliststring = "";

                foreach (var dishcode in dishcodesids)
                {
                    dishcodeliststring = dishcodeliststring + "data @> @dishcode" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "dishcode" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"DishRates\": [{ \"Id\": \"" + dishcode.ToUpper() + "\" }] }" });
                    counter++;
                }
                dishcodeliststring = dishcodeliststring.Remove(dishcodeliststring.Length - 4);

                whereexpression = whereexpression + dishcodeliststring + ")";
            }
        }

        private static void BoardFilter(ref string whereexpression, List<PGParameters> parameters, List<string> boardlist)
        {
            //Board Info schaugn ob des geat!!! umgekearter foll
            if (boardlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string boardliststring = "";
                int counter = 1;

                foreach (var boardid in boardlist)
                {
                    boardliststring = boardliststring + "data @> @boardid" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "boardid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"BoardIds\": [\"" + boardid + "\"]}" });
                    counter++;
                }
                boardliststring = boardliststring.Remove(boardliststring.Length - 4);

                whereexpression = whereexpression + boardliststring + ")";
            }
        }

        private static void BadgeFilter(ref string whereexpression, List<PGParameters> parameters, List<string> badgelist)
        {
            //Badges Info
            if (badgelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                int counter = 1;
                string badgeliststring = "";

                foreach (var badgeid in badgelist)
                {
                    badgeliststring = badgeliststring + "data @> @badgeid" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "badgeid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"BadgeIds\": [\"" + badgeid + "\"]}" });
                    counter++;
                }
                badgeliststring = badgeliststring.Remove(badgeliststring.Length - 4);

                whereexpression = whereexpression + badgeliststring + ")";
            }
        }

        private static void CategoryFilter(ref string whereexpression, List<PGParameters> parameters, List<string> categorylist)
        {
            //Category Info
            if (categorylist.Count > 0)
            {
                //Tuning force to use GIN Index
                if (categorylist.Count == 1)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "data @> @categoryid";
                    parameters.Add(new PGParameters() { Name = "categoryid", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"AccoCategoryId\": \"" + categorylist.FirstOrDefault() + "\" }" });
                }
                //Tuned to use GIN Index brings laut performance tests af der console ober net so im live betrieb schun!ALSO Bei kleinen Ergebnismengen ist dies schneller bei grösseren das untere
                else
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND (";
                    else
                        whereexpression = whereexpression + "(";

                    int counter = 1;
                    string categoryliststring = "";
                    foreach (var categoryid in categorylist)
                    {
                        categoryliststring = categoryliststring + "data @> @categoryid" + counter + " OR ";
                        parameters.Add(new PGParameters() { Name = "categoryid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"AccoCategoryId\": \"" + categoryid + "\"}" });
                        counter++;
                    }
                    categoryliststring = categoryliststring.Remove(categoryliststring.Length - 4);

                    whereexpression = whereexpression + categoryliststring + ")";
                }
                //Konstante laufzeit hot des.... net so stork noch ergebnismenge....
                //else
                //{
                //    if (!String.IsNullOrEmpty(whereexpression))
                //        whereexpression = whereexpression + " AND (";
                //    else
                //        whereexpression = whereexpression + "(";

                //    int counter = 1;

                //    string categoryliststring = "";
                //    foreach (var categoryid in categorylist)
                //    {
                //        categoryliststring = categoryliststring + "@categoryid" + counter + ", ";
                //        parameters.Add(new PGParameters() { Name = "categoryid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Text, Value = categoryid });
                //        counter++;
                //    }
                //    categoryliststring = categoryliststring.Remove(categoryliststring.Length - 2);

                //    whereexpression = whereexpression + "data->'AccoCategoryId' in (" + categoryliststring + "))";                   
                //}
            }
        }

        private static void ThemeFilter(ref string whereexpression, List<PGParameters> parameters, Dictionary<string, bool> themelist)
        {
            //Theme Info
            if (themelist.Where(x => x.Value == true).Count() > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string themeliststring = "";
                int counter = 1;

                foreach (var themeid in themelist.Where(x => x.Value == true))
                {
                    themeliststring = themeliststring + "data @> @themeid" + counter + " AND ";
                    parameters.Add(new PGParameters() { Name = "themeid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"ThemeIds\": [\"" + themeid.Key + "\"]}" });
                    counter++;
                }
                themeliststring = themeliststring.Remove(themeliststring.Length - 4);

                whereexpression = whereexpression + themeliststring + ")";
            }
        }

        private static void FeatureFilter(ref string whereexpression, List<PGParameters> parameters, Dictionary<string, bool> featurelist)
        {
            //Feature Info
            if (featurelist.Where(x => x.Value == true).Count() > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string featureliststring = "";
                int counter = 1;

                foreach (var featureid in featurelist.Where(x => x.Value == true))
                {
                    featureliststring = featureliststring + "data @> @featureid" + counter + " AND ";
                    parameters.Add(new PGParameters() { Name = "featureid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SpecialFeaturesIds\": [\"" + featureid.Key + "\"]}" });
                    counter++;
                }
                featureliststring = featureliststring.Remove(featureliststring.Length - 4);

                whereexpression = whereexpression + featureliststring + ")";
            }
        }

        private static void AccoTypeFilter(ref string whereexpression, List<PGParameters> parameters, List<string> accotypelist)
        {
            //AccoType Info schaugn ob des geat!!! umgekearter foll
            if (accotypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                //Tuning force to use GIN Index
                if (accotypelist.Count == 1)
                {
                    whereexpression = whereexpression + "data @> @accotypeid";
                    parameters.Add(new PGParameters() { Name = "accotypeid", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"AccoTypeId\": \"" + accotypelist.FirstOrDefault() + "\" }" });
                }
                else
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND (";
                    else
                        whereexpression = whereexpression + "(";

                    int counter = 1;
                    string categoryliststring = "";
                    foreach (var accotypeid in accotypelist)
                    {
                        categoryliststring = categoryliststring + "data @> @accotypeid" + counter + " OR ";
                        parameters.Add(new PGParameters() { Name = "accotypeid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"AccoTypeId\": \"" + accotypeid + "\"}" });
                        counter++;
                    }
                    categoryliststring = categoryliststring.Remove(categoryliststring.Length - 4);

                    whereexpression = whereexpression + categoryliststring + ")";
                }
                //else
                //{
                //    int counter = 1;
                //    string accotypeliststring = "";
                //    foreach (var accotypeid in accotypelist)
                //    {
                //        accotypeliststring = accotypeliststring + "@accotypeid" + counter + ", ";
                //        parameters.Add(new PGParameters() { Name = "accotypeid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = accotypeid });
                //        counter++;
                //    }
                //    accotypeliststring = accotypeliststring.Remove(accotypeliststring.Length - 2);

                //    whereexpression = whereexpression + "data->'AccoTypeId' in (" + accotypeliststring + ")";
                //}
            }
        }

        private static void ApartmentFilter(ref string whereexpression, List<PGParameters> parameters, bool apartmentfilter)
        {
            //Apartment
            if (apartmentfilter)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @hasapartment";
                parameters.Add(new PGParameters() { Name = "hasapartment", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"HasApartment\" : " + apartmentfilter.ToString().ToLower() + "}" });
            }
        }

        private static void BookableFilter(ref string whereexpression, List<PGParameters> parameters, bool? bookable)
        {
            //Bookable
            if (bookable != null)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @isbookable";
                parameters.Add(new PGParameters() { Name = "isbookable", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"IsBookable\" : " + bookable.ToString().ToLower() + " }" });
            }
        }

        private static void AltitudeFilter(ref string whereexpression, List<PGParameters> parameters, bool altitude, int altitudemin, int altitudemax)
        {
            //Altitude
            if (altitude)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";
                whereexpression = whereexpression + "(data ->>'Altitude')::numeric >= @altitudemin AND (data->> 'Altitude')::numeric <= @altitudemax";
                parameters.Add(new PGParameters() { Name = "altitudemin", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = altitudemin.ToString() });
                parameters.Add(new PGParameters() { Name = "altitudemax", Type = NpgsqlTypes.NpgsqlDbType.Numeric, Value = altitudemax.ToString() });
            }
        }

        private static void TypesToExcludeFilter(ref string whereexpression, List<PGParameters> parameters, List<string> typestoexclude)
        {
            //Activity Type
            if (typestoexclude.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string typestring = "";
                int counter = 1;

                foreach (var typeId in typestoexclude)
                {
                    typestring = typestring + "NOT data @> @typestoexclude" + counter + " AND ";
                    parameters.Add(new PGParameters() { Name = "typestoexclude" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Type\": \"" + typeId + "\"}" });
                    counter++;
                }
                typestring = typestring.Remove(typestring.Length - 4);

                whereexpression = whereexpression + typestring + ")";
            }
        }

        private static void SubTypesToExcludeFilter(ref string whereexpression, List<PGParameters> parameters, List<string> subtypestoexclude)
        {
            //Activity Sub Type            
            if (subtypestoexclude.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string subtypeliststring = "";
                int counter = 1;

                foreach (var subtype in subtypestoexclude)
                {
                    subtypeliststring = subtypeliststring + "NOT data @> @subtypestoexclude" + counter + " AND ";
                    parameters.Add(new PGParameters() { Name = "subtypestoexclude" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"SubType\": \"" + subtype + "\"}" });
                    counter++;
                }
                subtypeliststring = subtypeliststring.Remove(subtypeliststring.Length - 4);

                whereexpression = whereexpression + subtypeliststring + ")";
            }


        }

        private static void PoiTypesToExcludeFilter(ref string whereexpression, List<PGParameters> parameters, List<string> poitypestoexclude)
        {
            //Activity POI Type            
            if (poitypestoexclude.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string poitypeliststring = "";
                int counter = 1;

                foreach (var poitype in poitypestoexclude)
                {
                    poitypeliststring = poitypeliststring + "NOT data @> @poitypetoexclude" + counter + " AND ";
                    parameters.Add(new PGParameters() { Name = "poitypetoexclude" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"PoiType\": \"" + poitype + "\"}" });
                    counter++;
                }
                poitypeliststring = poitypeliststring.Remove(poitypeliststring.Length - 4);

                whereexpression = whereexpression + poitypeliststring + ")";
            }
        }

        private static void PackagesThemeFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> themelist)
        {
            if (themelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string smgtagliststring = "";
                int counter = 1;

                foreach (var theme in themelist)
                {
                    smgtagliststring = smgtagliststring + "data @> @theme" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "theme" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"PackageThemeList\": [\"" + theme + "\"]}" });
                    counter++;
                }
                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

                whereexpression = whereexpression + smgtagliststring + ")";
            }
        }

        private static void PackagesBoardFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> boardlist)
        {
            if (boardlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string boardliststring = "";
                int counter = 1;

                foreach (var boardid in boardlist)
                {
                    boardliststring = boardliststring + "data @> @board" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "board" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Services\": [\"" + boardid + "\"]}" });
                    counter++;
                }
                boardliststring = boardliststring.Remove(boardliststring.Length - 4);

                whereexpression = whereexpression + boardliststring + ")";
            }
        }

        private static void PackagesAccoFilterWhere(ref string whereexpression, List<PGParameters> parameters, List<string> accolist)
        {
            if (accolist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND (";
                else
                    whereexpression = whereexpression + "(";

                string accoliststring = "";
                int counter = 1;

                foreach (var accoid in accolist)
                {
                    accoliststring = accoliststring + "data @> @accoid" + counter + " OR ";
                    parameters.Add(new PGParameters() { Name = "accoid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"HotelId\": [\"" + accoid + "\"]}" });
                    counter++;
                }
                accoliststring = accoliststring.Remove(accoliststring.Length - 4);

                whereexpression = whereexpression + accoliststring + ")";
            }
        }

        private static void PackagesStayFilterWhere(ref string whereexpression, List<PGParameters> parameters, bool shortstay, bool longstay)
        {
            //Shortstay
            if (shortstay)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @shortstay";
                parameters.Add(new PGParameters() { Name = "shortstay", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"ShortStay\" : " + shortstay.ToString().ToLower() + "}" });
            }

            //Longstay
            if (longstay)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @longstay";
                parameters.Add(new PGParameters() { Name = "longstay", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"LongStay\" : " + longstay.ToString().ToLower() + "}" });
            }
        }

        private static void PackagesValidFromFilterWhere(ref string whereexpression, List<PGParameters> parameters, DateTime validfrom, DateTime validto)
        {
            //Datum von bis valid
            if (validfrom != DateTime.MinValue)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "to_date(data ->> 'ValidStart', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", validfrom) + "'";
            }
            if (validto != DateTime.MaxValue)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "to_date(data ->> 'ValidStop', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", validto) + "'";
            }
        }

        private static void EventShortActiveFilterWhere(ref string whereexpression, List<PGParameters> parameters, string activefilter)
        {

            //Active
            if (!String.IsNullOrEmpty(activefilter))
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @activefilter";
                parameters.Add(new PGParameters() { Name = "activefilter", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Display1\" :  \"" + activefilter.ToString() + "\" }" });
            }
        }

        private static void EventShortSourceFilterWhere(ref string whereexpression, List<PGParameters> parameters, string source)
        {
            //Source
            if (!String.IsNullOrEmpty(source))
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @sourcefilter";
                parameters.Add(new PGParameters() { Name = "sourcefilter", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"Source\" : \"" + source.ToString() + "\" }" });
            }
        }

        private static void EventShortEventLocationFilterWhere(ref string whereexpression, List<PGParameters> parameters, string eventlocation)
        {
            //EventLocation
            if (!String.IsNullOrEmpty(eventlocation))
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @eventlocation";
                parameters.Add(new PGParameters() { Name = "eventlocation", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"EventLocation\" : \"" + eventlocation.ToString() + "\" }" });
            }

        }

        private static void EventShortBeginEndFilterWhere(ref string whereexpression, List<PGParameters> parameters, DateTime start, DateTime end)
        {
            //Begin & End
            if (start != null && end != null)
            {
                if (start != DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "(((to_date(data ->> 'StartDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "') AND (to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                }
                else if (start != DateTime.MinValue && end == DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "((to_date(data ->> 'StartDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "'))";
                }
                else if (start == DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "((to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                }
            }
        }

        private static void EventShortBeginEndSpecialFilterWhere(ref string whereexpression, List<PGParameters> parameters, DateTime start, DateTime end)
        {
            //Begin & End
            if (start != null && end != null)
            {
                if (start != DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "(((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "') AND (to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                }
                else if (start != DateTime.MinValue && end == DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", start) + "'))";
                }
                else if (start == DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "((to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";
                }

            }
        }

        #endregion


    }
}
