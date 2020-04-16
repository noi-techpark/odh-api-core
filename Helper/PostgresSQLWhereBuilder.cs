using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helper
{
    public static class PostgresSQLWhereBuilder
    {
        private static string[] _languagesToSearchFor =
            new[] { "de", "it", "en" };

        /// <summary>
        /// Provide title fields as JsonPath
        /// </summary>
        /// <param name="language">
        /// If provided only the fields with the
        /// specified language get returned
        /// </param>
        private static string[] TitleFieldsToSearchFor(string? language) =>
            _languagesToSearchFor.Where(lang =>
                language != null ? lang == language : true
            ).Select(lang =>
                $"Detail.{lang}.Title"
            ).ToArray();

        public static void CheckPassedLanguage(ref string language, IEnumerable<string> availablelanguages)
        {
            language = language.ToLower();

            if (!availablelanguages.Contains(language))
                throw new Exception("passed language not available or passed incorrect string");
        }

        //Return where and Parameters
        public static (string, IEnumerable<PGParameters>) CreateIdListWhereExpression(IReadOnlyCollection<string> idlist)
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
                    whereexpression += "id LIKE @id";
                    parameters.Add(new PGParameters()
                    {
                        Name = "id",
                        Type = NpgsqlTypes.NpgsqlDbType.Text,
                        Value = idlist.FirstOrDefault()
                    });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var accoid in idlist)
                    {
                        idliststring += $"@id{counter}, ";
                        parameters.Add(new PGParameters()
                        {
                            Name = "id" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Text,
                            Value = accoid
                        });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression += $"Id in ({idliststring})";
                }
            }


            return (whereexpression, parameters);
        }

        //Return where and Parameters
        public static (string, IEnumerable<PGParameters>) CreateIdListWhereExpression(string? id)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (!String.IsNullOrEmpty(id))
            {
                whereexpression += "id LIKE @id";
                parameters.Add(new PGParameters()
                {
                    Name = "id",
                    Type = NpgsqlTypes.NpgsqlDbType.Text,
                    Value = id
                });
            }

            return (whereexpression, parameters);
        }

        //Return where and Parameters
        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateIdListWhereExpressionCaseInsensitive(string id)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (!String.IsNullOrEmpty(id))
            {
                whereexpression += "id ILIKE @id";
                parameters.Add(new PGParameters()
                {
                    Name = "id",
                    Type = NpgsqlTypes.NpgsqlDbType.Text,
                    Value = id
                });
            }

            return (whereexpression, parameters);
        }

        //Return where and PArameters
        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateIdListWhereExpression(
            IReadOnlyCollection<string> idlist, bool insertdummyonemptyidlist)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (idlist.Count > 0)
            {
                //Tuning force to use GIN Index
                if (idlist.Count == 1)
                {
                    whereexpression += "Id = @id";
                    parameters.Add(new PGParameters()
                    {
                        Name = "id",
                        Type = NpgsqlTypes.NpgsqlDbType.Text,
                        Value = idlist.FirstOrDefault()
                    });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;

                    foreach (var id in idlist)
                    {
                        idliststring += $"@id{counter}, ";
                        parameters.Add(new PGParameters()
                        {
                            Name = "id" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Text,
                            Value = id
                        });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression += $"Id in ({idliststring})";
                }
            }
            else if (idlist.Count == 0)
            {
                if (insertdummyonemptyidlist)
                {
                    whereexpression = "Id = @dummy";
                    parameters.Add(new PGParameters()
                    {
                        Name = "dummy",
                        Type = NpgsqlTypes.NpgsqlDbType.Text,
                        Value = "00000000"
                    });
                }

            }


            return (whereexpression, parameters); ;
        }

        //Return Where and Parameters for MetaRegion Query4
        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateMetaRegionWhereExpression(
            IReadOnlyCollection<string> metaregionlist)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            //IDLIST
            if (metaregionlist.Count > 0)
            {
                //Tuning force to use GIN Index
                if (metaregionlist.Count == 1)
                {
                    whereexpression = "data @> @jsonexpression";
                    string jsonexpression = $"{{\"Id\" : \"{metaregionlist.FirstOrDefault().ToUpper()}\" }}";

                    parameters.Add(new PGParameters() { Name = "jsonexpression", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = jsonexpression });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var mtaid in metaregionlist)
                    {
                        idliststring += $"@mtaid{counter},";
                        parameters.Add(new PGParameters()
                        {
                            Name = "mtaid" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = "\"" + mtaid.ToUpper() + "\""
                        });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 1);

                    whereexpression += $"data -> 'Id' IN ({idliststring})";
                }
            }

            return (whereexpression, parameters);
        }

        [System.Diagnostics.Conditional("TRACE")]
        private static void LogMethodInfo(System.Reflection.MethodBase m, params object?[] parameters)
        {
            var parameterInfo =
                m.GetParameters()
                    .Zip(parameters)
                    .Select((x, _) => (x.First.Name, x.Second));
            Serilog.Log.Debug("{method}({@parameters})", m.Name, parameterInfo);
        }

        //Return Where and Parameters for Activity
        public static Query ActivityWhereExpression(
            this Query query,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> activitytypelist,
            IReadOnlyCollection<string> subtypelist, IReadOnlyCollection<string> difficultylist,
            IReadOnlyCollection<string> smgtaglist, IReadOnlyCollection<string> districtlist,
            IReadOnlyCollection<string> municipalitylist, IReadOnlyCollection<string> tourismvereinlist,
            IReadOnlyCollection<string> regionlist, IReadOnlyCollection<string> arealist, bool distance, int distancemin,
            int distancemax, bool duration, int durationmin, int durationmax, bool altitude, int altitudemin,
            int altitudemax, bool? highlight, bool? activefilter, bool? smgactivefilter, string? searchfilter,
            string? language, string? lastchange)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, activitytypelist,
                subtypelist, difficultylist,
                smgtaglist, districtlist,
                municipalitylist, tourismvereinlist,
                regionlist, arealist, distance, distancemin,
                distancemax, duration, durationmin,
                durationmax, altitude, altitudemin,
                altitudemax, highlight, activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .DistrictFilter(districtlist)
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .AreaFilter(arealist)
                .ActivityTypeFilter(activitytypelist)
                .ActivitySubTypeFilter(subtypelist)
                .DifficultyFilter(difficultylist)
                .DistanceFilter(distance, distancemin, distancemax)
                .DurationFilter(duration, durationmin, durationmax)
                .AltitudeFilter(altitude, altitudemin, altitudemax)
                .HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SmgTagFilter(smgtaglist)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange);
        }

        //Return Where and Parameters for Poi
        public static Query PoiWhereExpression(
            this Query query,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> poitypelist,
            IReadOnlyCollection<string> subtypelist, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist,
            IReadOnlyCollection<string> arealist, bool? highlight, bool? activefilter,
            bool? smgactivefilter, string? searchfilter, string? language, string? lastchange)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", 
                idlist, poitypelist,
                subtypelist, smgtaglist,
                districtlist, municipalitylist,
                tourismvereinlist, regionlist,
                arealist, highlight, activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .DistrictFilter(districtlist)
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .AreaFilter(arealist)
                .PoiTypeFilter(poitypelist)
                .PoiSubTypeFilter(subtypelist)
                .SmgTagFilter(smgtaglist)
                .HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange);
        }

        //Return Where and Parameters for Gastronomy
        public static Query GastronomyWhereExpression(
            this Query query,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> dishcodeslist,
            IReadOnlyCollection<string> ceremonycodeslist, IReadOnlyCollection<string> categorycodeslist,
            IReadOnlyCollection<string> facilitycodeslist, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist, bool? activefilter,
            bool? smgactivefilter, string? searchfilter, string? language, string? lastchange)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                idlist, dishcodeslist,
                ceremonycodeslist, categorycodeslist,
                facilitycodeslist, smgtaglist,
                districtlist, municipalitylist,
                tourismvereinlist, regionlist,
                activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .DistrictFilter(districtlist)                
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .CeremonyCodeFilter(ceremonycodeslist)
                .CategoryCodeFilter(categorycodeslist)
                .CuisineCodeFilter(facilitycodeslist)
                .DishCodeFilter(dishcodeslist)
                .SmgTagFilter(smgtaglist)                
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange);
        }

        //Return Where and Parameters for Activity
        public static Query ODHActivityPoiWhereExpression(
            this Query query,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typelist, IReadOnlyCollection<string> subtypelist,
            IReadOnlyCollection<string> poitypelist, IReadOnlyCollection<string> sourcelist, IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> smgtaglist, IReadOnlyCollection<string> districtlist,
            IReadOnlyCollection<string> municipalitylist, IReadOnlyCollection<string> tourismvereinlist,
            IReadOnlyCollection<string> regionlist, IReadOnlyCollection<string> arealist, bool? highlight, bool? activefilter, bool? smgactivefilter, 
            string? searchfilter, string? language, string? lastchange)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, typelist,
                subtypelist, poitypelist, languagelist, sourcelist,
                smgtaglist, districtlist,
                municipalitylist, tourismvereinlist,
                regionlist, arealist, highlight, activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdLowerFilter(idlist)
                .DistrictFilter(districtlist)
                .LocFilterMunicipalityFilter(municipalitylist)
                .LocFilterTvsFilter(tourismvereinlist)
                .LocFilterRegionFilter(regionlist)
                .AreaFilter(arealist)
                .ODHActivityPoiTypeFilter(typelist)
                .ODHActivityPoiSubTypeFilter(subtypelist)
                .ODHActivityPoiPoiTypeFilter(subtypelist)
                .SourceFilter(sourcelist)
                .HasLanguageFilter(languagelist)
                .HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SmgTagFilter(smgtaglist)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange);
        }

        //Return Where and Parameters for Article
        public static Query ArticleWhereExpression(
            this Query query,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typelist, IReadOnlyCollection<string> subtypelist,
            IReadOnlyCollection<string> languagelist,
            IReadOnlyCollection<string> smgtaglist, bool? highlight, bool? activefilter, bool? smgactivefilter,
            string? searchfilter, string? language, string? lastchange)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, typelist,
                subtypelist, languagelist, smgtaglist, 
                highlight, activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdLowerFilter(idlist)
                .ODHActivityPoiTypeFilter(typelist)
                .ODHActivityPoiSubTypeFilter(subtypelist)
                .ODHActivityPoiPoiTypeFilter(subtypelist)
                .HasLanguageFilter(languagelist)
                .HighlightFilter(highlight)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SmgTagFilter(smgtaglist)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter)
                .LastChangedFilter(lastchange);
        }


        //Return Where and Parameters for WebCamInfo
        public static Query WebCamInfoWhereExpression(
            this Query query,
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> sourcelist,
            bool? activefilter, bool? smgactivefilter, string? searchfilter,
            string? language, string? lastchange)
        {
            LogMethodInfo(
                System.Reflection.MethodBase.GetCurrentMethod()!,
                 "<query>", // not interested in query
                idlist, sourcelist,
                activefilter,
                smgactivefilter, searchfilter,
                language, lastchange
            );

            return query
                .IdUpperFilter(idlist)
                .SourceFilter(sourcelist)
                .ActiveFilter(activefilter)
                .SmgActiveFilter(smgactivefilter)
                .SearchFilter(TitleFieldsToSearchFor(language), searchfilter) //TODO here the title is in another field
                .LastChangedFilter(lastchange);
        }



        //Return Where and Parameters for Events
        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateEventWhereExpression(
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> orgidlist,
            IReadOnlyCollection<string> rancidlist, IReadOnlyCollection<string> typeidlist,
            IReadOnlyCollection<string> topicrids, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist, DateTime? begin,
            DateTime? end, bool? activefilter, bool? smgactivefilter)
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

            return (whereexpression, parameters);
        }

        //Return Where and Parameters for SmgPoi
        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateSmgPoiWhereExpression(
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typelist,
            IReadOnlyCollection<string> subtypelist, IReadOnlyCollection<string> poitypelist,
            IReadOnlyCollection<string> languagelist, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist,
            IReadOnlyCollection<string> arealist, IReadOnlyCollection<string> sourcelist, bool? highlight,
            bool? activefilter, bool? smgactivefilter)
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

            return (whereexpression, parameters);
        }

        //Return Where and Parameters for Gastronomy
        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateGastroWhereExpression(
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> dishcodesids,
            IReadOnlyCollection<string> ceremonycodesids, IReadOnlyCollection<string> categorycodesids,
            IReadOnlyCollection<string> facilitycodesids, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist, bool? activefilter,
            bool? smgactivefilter)
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

            return (whereexpression, parameters);
        }

        //Return Where and Parameters for Accommodation
        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateAccoWhereExpression(
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> accotypelist, bool apartmentfilter,
            IReadOnlyCollection<string> categorylist, IReadOnlyDictionary<string, bool> featurelist,
            IReadOnlyCollection<string> badgelist, IReadOnlyDictionary<string, bool> themelist,
            IReadOnlyCollection<string> boardlist, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist, bool? activefilter,
            bool? smgactivefilter, bool? bookable, bool altitude, int altitudemin, int altitudemax)
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

            return (whereexpression, parameters);
        }

        //Return Where and Parameters for Article
        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateArticleWhereExpression(
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typelist,
            IReadOnlyCollection<string> subtypelist, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> languagelist, bool? highlight, bool? activefilter, bool? smgactivefilter)
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

            return (whereexpression, parameters);
        }

        //Returns Where and Parameter for EventShort
        [Obsolete]
        public static (string wherexpression, IEnumerable<PGParameters> parameters) CreateEventShortWhereExpression(
            DateTime start, DateTime end, string source, string eventlocation,
            string activefilter, IReadOnlyCollection<string> eventidlist, bool special)
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

            return (whereexpression, parameters);
        }

        //Returns Where and Parameter for Packages
        [Obsolete]
        public static (string wherexpression, IEnumerable<PGParameters> parameters) CreatePackageWhereExpression(
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> accolist,
            IReadOnlyCollection<string> boardlist, IReadOnlyCollection<string> themelist,
            IReadOnlyCollection<string> smgtaglist, IReadOnlyCollection<string> districtlist,
            IReadOnlyCollection<string> municipalitylist, IReadOnlyCollection<string> tourismvereinlist,
            IReadOnlyCollection<string> regionlist, DateTime validfrom, DateTime validto, bool longstay, bool shortstay,
            bool? activefilter, bool? smgactivefilter)
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

            return (whereexpression, parameters);
        }

        //Return Where and Parameters for Alpinebits
        [Obsolete]
        public static (string wherexpression, IEnumerable<PGParameters> parameters) CreateAlpineBitsWhereExpression(
            IReadOnlyCollection<string> idlist, string source,
            string messagetype, string requestdate,
            IReadOnlyCollection<string> accommodationIds)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            IdFilterWhere(ref whereexpression, parameters, idlist);
            AlpineBitsMessageFilterWhere(ref whereexpression, parameters, messagetype);
            AlpineBitsSourceFilterWhere(ref whereexpression, parameters, source);
            AlpineBitsAccommodationIdFilterWhere(ref whereexpression, parameters, accommodationIds);
            //RequestDate

            return (whereexpression, parameters);
        }

        #region Reusable Where Builders

        #region Reusable Where Builders Mobile

        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateSmgPoiMobileWhereExpression(
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> typelist,
            IReadOnlyCollection<string> subtypelist, IReadOnlyCollection<string> poitypelist,
            IReadOnlyCollection<string> difficultylist, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> languagelist, IReadOnlyCollection<string> districtlist,
            IReadOnlyCollection<string> municipalitylist, IReadOnlyCollection<string> tourismvereinlist,
            IReadOnlyCollection<string> regionlist, bool distance, int distancemin, int distancemax, bool duration,
            double durationmin, double durationmax, bool altitude, int altitudemin, int altitudemax, bool? highlight,
            bool? activefilter)
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

            return (whereexpression, parameters);
        }

        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateEventMobileWhereExpression(
            IReadOnlyCollection<string> idlist, IReadOnlyCollection<string> orgidlist,
            IReadOnlyCollection<string> rancidlist, IReadOnlyCollection<string> typeidlist,
            IReadOnlyCollection<string> topicrids, IReadOnlyCollection<string> smgtaglist,
            IReadOnlyCollection<string> districtlist, IReadOnlyCollection<string> municipalitylist,
            IReadOnlyCollection<string> tourismvereinlist, IReadOnlyCollection<string> regionlist, DateTime? begin,
            DateTime? end, bool fromnow, bool? activefilter, bool? smgactivefilter, string languagefilter)
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

            return (whereexpression, parameters);
        }

        [Obsolete]
        public static (string, IEnumerable<PGParameters>) CreateTipsMobileWhereExpression(
            IReadOnlyCollection<string> typestoexclude, IReadOnlyCollection<string> subtypestoexclude,
            IReadOnlyCollection<string> poitypestoexclude, IReadOnlyCollection<string> languagelist,
            bool? highlight, bool? activefilter)
        {
            string whereexpression = "";
            List<PGParameters> parameters = new List<PGParameters>();

            HasLanguageFilterWhere(ref whereexpression, parameters, languagelist);
            HighlightFilterWhere(ref whereexpression, parameters, highlight);
            ActiveFilterWhere(ref whereexpression, parameters, activefilter);

            TypesToExcludeFilter(ref whereexpression, parameters, typestoexclude);
            SubTypesToExcludeFilter(ref whereexpression, parameters, subtypestoexclude);
            PoiTypesToExcludeFilter(ref whereexpression, parameters, poitypestoexclude);

            return (whereexpression, parameters);
        }

        #endregion

        [Obsolete]
        private static void IdUpperFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> idlist)
        {
            //IDLIST
            if (idlist.Count > 0)
            {
                if (idlist.Count == 1)
                {
                    whereexpression = $"{whereexpression}id LIKE @id";
                    parameters.Add(new PGParameters()
                    {
                        Name = "id",
                        Type = NpgsqlTypes.NpgsqlDbType.Text,
                        Value = idlist.FirstOrDefault().ToUpper()
                    });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var activityid in idlist)
                    {
                        idliststring += $"@id{counter}, ";
                        parameters.Add(new PGParameters()
                        {
                            Name = "id" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Text,
                            Value = activityid.ToUpper()
                        });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression = $"{whereexpression}id in ({idliststring})";
                }
            }
        }

        [Obsolete]
        private static void IdLowerFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> idlist)
        {
            //IDLIST
            if (idlist.Count > 0)
            {
                if (idlist.Count == 1)
                {
                    whereexpression += "id LIKE @id";
                    parameters.Add(new PGParameters()
                    {
                        Name = "id",
                        Type = NpgsqlTypes.NpgsqlDbType.Text,
                        Value = idlist.FirstOrDefault().ToLower()
                    });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var activityid in idlist)
                    {
                        idliststring += $"@id{counter}, ";
                        parameters.Add(new PGParameters()
                        {
                            Name = "id" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Text,
                            Value = activityid.ToLower()
                        });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression = whereexpression + "id in (" + idliststring + ")";
                }
            }
        }

        [Obsolete]
        private static void IdFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> idlist)
        {
            //IDLIST
            if (idlist.Count > 0)
            {
                if (idlist.Count == 1)
                {
                    whereexpression += "id LIKE @id";
                    parameters.Add(new PGParameters() { Name = "id", Type = NpgsqlTypes.NpgsqlDbType.Text, Value = idlist.FirstOrDefault() });
                }
                else
                {
                    string idliststring = "";
                    int counter = 1;
                    foreach (var activityid in idlist)
                    {
                        idliststring = idliststring + "@id" + counter + ", ";
                        parameters.Add(new PGParameters() { Name = "id" + counter, Type = NpgsqlTypes.NpgsqlDbType.Text, Value = activityid });
                        counter++;
                    }
                    idliststring = idliststring.Remove(idliststring.Length - 2);

                    whereexpression = whereexpression + "id in (" + idliststring + ")";
                }
            }
        }

        [Obsolete]
        private static void DistrictWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> districtlist)
        {
            //DISTRICT
            if (districtlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                //Tuning force to use GIN Index
                if (districtlist.Count == 1)
                {
                    whereexpression += "data @> @districtid";
                    parameters.Add(new PGParameters()
                    {
                        Name = "districtid",
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{\"DistrictId\" : \"" + districtlist.FirstOrDefault().ToUpper() + "\" }"
                    });
                }
                else
                {
                    string districtliststring = "";
                    int counter = 1;
                    foreach (var distid in districtlist)
                    {
                        districtliststring = districtliststring + "@districtid" + counter + ",";
                        parameters.Add(new PGParameters()
                        {
                            Name = "districtid" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = "\"" + distid.ToUpper() + "\""
                        });
                        counter++;
                    }
                    districtliststring = districtliststring.Remove(districtliststring.Length - 1);

                    whereexpression = $"{whereexpression}data->'DistrictId' IN ({districtliststring})";
                }
            }

        }

        [Obsolete]
        private static void LocFilterDistrictWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> districtlist)
        {
            //MUNICIPALITY
            if (districtlist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                //Tuning force to use GIN Index
                if (districtlist.Count == 1)
                {
                    whereexpression += "data @> @districtid";
                    parameters.Add(new PGParameters()
                    {
                        Name = "districtid",
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{\"LocationInfo\" : {{ \"DistrictInfo\": {{ \"Id\": \"{districtlist.FirstOrDefault().ToUpper()}\" }} }} }}"
                    });
                }
                else
                {
                    string districtliststring = "";
                    int counter = 1;
                    foreach (var distid in districtlist)
                    {
                        districtliststring += $"@districtid{counter},";
                        parameters.Add(new PGParameters()
                        {
                            Name = "districtid" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = "\"" + distid.ToUpper() + "\""
                        });
                        counter++;
                    }
                    districtliststring = districtliststring.Remove(districtliststring.Length - 1);

                    whereexpression += $" data->'LocationInfo'-> 'DistrictInfo' -> 'Id' in ({districtliststring})";
                }
            }
        }

        [Obsolete]
        private static void LocFilterMunicipalityWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> municipalitylist)
        {
            //MUNICIPALITY
            if (municipalitylist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                //Tuning force to use GIN Index
                if (municipalitylist.Count == 1)
                {
                    whereexpression += "data @> @municipalityid";
                    parameters.Add(new PGParameters()
                    {
                        Name = "municipalityid",
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{\"LocationInfo\" : { \"MunicipalityInfo\": { \"Id\": \"" + municipalitylist.FirstOrDefault().ToUpper() + "\" } } }"
                    });
                }
                else
                {
                    string municipalityliststring = "";
                    int counter = 1;
                    foreach (var munid in municipalitylist)
                    {
                        municipalityliststring += $"@municipalityid{counter},";
                        parameters.Add(new PGParameters()
                        {
                            Name = "municipalityid" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = "\"" + munid.ToUpper() + "\""
                        });
                        counter++;
                    }
                    municipalityliststring = municipalityliststring.Remove(municipalityliststring.Length - 1);

                    whereexpression += $" data->'LocationInfo'-> 'MunicipalityInfo' -> 'Id' in ({municipalityliststring})";
                }
            }

        }

        [Obsolete]
        private static void LocFilterTvsWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> tourismvereinlist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "tourismvereinid",
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{\"LocationInfo\" : {{ \"TvInfo\": {{ \"Id\": \"{tourismvereinlist.FirstOrDefault().ToUpper()}\" }} }} }}"
                    });
                }
                else
                {
                    string tvliststring = "";
                    int counter = 1;
                    foreach (var tvid in tourismvereinlist)
                    {
                        tvliststring = tvliststring + "@tourismvereinid" + counter + ",";
                        parameters.Add(new PGParameters()
                        {
                            Name = "tourismvereinid" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = "\"" + tvid.ToUpper() + "\""
                        });
                        counter++;
                    }
                    tvliststring = tvliststring.Remove(tvliststring.Length - 1);

                    whereexpression += $" data->'LocationInfo'-> 'TvInfo' -> 'Id' in ({tvliststring})";
                }
            }

        }

        [Obsolete]
        private static void LocFilterRegionWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> regionlist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "regionid",
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{\"LocationInfo\" : {{ \"RegionInfo\": {{ \"Id\": \"{regionlist.FirstOrDefault()}\" }} }} }}"
                    });
                }
                else
                {

                    string regionliststring = "";
                    int counter = 1;
                    foreach (var regid in regionlist)
                    {
                        regionliststring = regionliststring + "@regionid" + counter + ",";
                        parameters.Add(new PGParameters()
                        {
                            Name = "regionid" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = "\"" + regid.ToUpper() + "\""
                        });
                        counter++;
                    }
                    regionliststring = regionliststring.Remove(regionliststring.Length - 1);

                    whereexpression += $" data->'LocationInfo'-> 'RegionInfo' -> 'Id' in ({regionliststring})";
                }
            }

        }

        [Obsolete]
        private static void AreaFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> arealist)
        {
            //AREA
            if (arealist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string arealiststring = "";
                int counter = 1;
                foreach (var area in arealist)
                {
                    arealiststring = $"{arealiststring}data @> @area{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "area" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"AreaId\": [\"" + area + "\"]}"
                    });
                    counter++;
                }
                arealiststring = arealiststring.Remove(arealiststring.Length - 4);

                whereexpression = whereexpression + arealiststring + ")";
            }
        }

        [Obsolete]
        private static void HighlightFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, bool? highlight)
        {
            //Highlight
            if (highlight != null)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "data @> @highlight";
                parameters.Add(new PGParameters()
                {
                    Name = "highlight",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = $"{{ \"Highlight\" : {(highlight ?? false).ToString().ToLower()}}}"
                });
            }
        }

        [Obsolete]
        private static void ActiveFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, bool? activefilter)
        {
            //Active
            if (activefilter != null)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "data @> @active";
                parameters.Add(new PGParameters()
                {
                    Name = "active",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = $"{{ \"Active\" : {(activefilter ?? false).ToString().ToLower()}}}"
                });
            }
        }

        [Obsolete]
        private static void SmgActiveFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, bool? smgactivefilter)
        {
            //SmgActive
            if (smgactivefilter != null)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "data @> @smgactive";
                parameters.Add(new PGParameters()
                {
                    Name = "smgactive",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = $"{{ \"SmgActive\" : {(smgactivefilter ?? false).ToString().ToLower()}}}"
                });
            }
        }

        [Obsolete]
        private static void DistanceFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, bool distance,
            int distancemin, int distancemax)
        {
            //DISTANCE
            if (distance)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "(data ->> 'DistanceLength')::numeric > @distancemin AND (data ->> 'DistanceLength')::numeric < @distancemax";
                parameters.Add(new PGParameters()
                {
                    Name = "distancemin",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = distancemin.ToString()
                });
                parameters.Add(new PGParameters()
                {
                    Name = "distancemax",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = distancemax.ToString()
                });
            }
        }

        [Obsolete]
        private static void DurationFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters,
            bool duration, int durationmin, int durationmax)
        {
            //DURATION
            if (duration)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "(data ->> 'DistanceDuration')::numeric > @durationmin AND (data ->> 'DistanceDuration')::numeric < @durationmax";
                parameters.Add(new PGParameters()
                {
                    Name = "durationmin",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = durationmin.ToString()
                });
                parameters.Add(new PGParameters()
                {
                    Name = "durationmax",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = durationmax.ToString()
                });
            }
        }

        [Obsolete]
        private static void DurationFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters,
            bool duration, double durationmin, double durationmax)
        {
            //DURATION
            if (duration)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "(data ->> 'DistanceDuration')::numeric > @durationmin AND (data ->> 'DistanceDuration')::numeric < @durationmax";
                parameters.Add(new PGParameters()
                {
                    Name = "durationmin",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = durationmin.ToString()
                });
                parameters.Add(new PGParameters()
                {
                    Name = "durationmax",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = durationmax.ToString()
                });
            }
        }

        [Obsolete]
        private static void AltitudeFilter(
            ref string whereexpression, IList<PGParameters> parameters,
            bool altitude, int altitudemin, int altitudemax)
        {
            //ALTITUDE
            if (altitude)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "(data ->> 'AltitudeDifference')::numeric > @altitudemin AND (data ->> 'AltitudeDifference')::numeric < @altitudemax";
                parameters.Add(new PGParameters()
                {
                    Name = "altitudemin",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = altitudemin.ToString()
                });
                parameters.Add(new PGParameters()
                {
                    Name = "altitudemax",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = altitudemax.ToString()
                });
            }

        }

        [Obsolete]
        private static void AltitudeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters,
            bool altitude, int altitudemin, int altitudemax)
        {
            //Altitude
            if (altitude)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";
                whereexpression += "(data ->>'Altitude')::numeric >= @altitudemin AND (data->> 'Altitude')::numeric <= @altitudemax";
                parameters.Add(new PGParameters()
                {
                    Name = "altitudemin",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = altitudemin.ToString()
                });
                parameters.Add(new PGParameters()
                {
                    Name = "altitudemax",
                    Type = NpgsqlTypes.NpgsqlDbType.Numeric,
                    Value = altitudemax.ToString()
                });
            }
        }

        [Obsolete]
        private static void SmgTagFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> smgtaglist)
        {
            //SmgTags Info
            if (smgtaglist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string smgtagliststring = "";
                int counter = 1;

                foreach (var smgtag in smgtaglist)
                {
                    smgtagliststring += $"data @> @smgtag{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "smgtag" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"SmgTags\": [\"{smgtag.ToLower()}\"]}}"
                    });
                    counter++;
                }
                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

                whereexpression += smgtagliststring + ")";
            }
        }

        [Obsolete]
        private static void SearchFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, string[] fields, string? searchfilter)
        {
            /// <summary>
            /// Convert a (simple) JsonPath path to a Postgres array,
            /// which can be used in the #>> operator.<br />
            /// E.g. Detail.de.Title => Detail,de,Title
            /// </summary>
            static string JsonPathToPostgresArray(string field) =>
                field.Replace('.', ',');

            if (searchfilter != null && fields.Length > 0)
            {
                string searchwhereexpression = "";
                if (!string.IsNullOrEmpty(whereexpression))
                    searchwhereexpression += " AND (";
                else
                    searchwhereexpression += "(";
                searchwhereexpression +=
                    string.Join(
                        " OR ",
                        fields.Select(field =>
                            $"quote_ident(data#>>'{{{JsonPathToPostgresArray(field)}}}') ILIKE @searchfilter"
                        )
                    );
                parameters.Add(new PGParameters()
                {
                    Name = "searchfilter",
                    Type = NpgsqlTypes.NpgsqlDbType.Text,
                    Value = $"%{searchfilter}%"
                });
                searchwhereexpression += ")";

                whereexpression += searchwhereexpression;
            }
        }

        [Obsolete]
        private static void ActivityTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> activitytypelist)
        {
            //Activity Type
            if (activitytypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var activitytypeId in activitytypelist)
                {
                    activitytypestring = $"{activitytypestring}data @> @type{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "type" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"Type\": \"{activitytypeId}\"}}"
                    });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression += activitytypestring + ")";
            }
        }

        [Obsolete]
        private static void ActivitySubTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> subtypelist)
        {
            //Activity Sub Type
            if (subtypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string smgtagliststring = "";
                int counter = 1;

                foreach (var smgtag in subtypelist)
                {
                    smgtagliststring = $"{smgtagliststring}data @> @subtype{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "subtype" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"SmgTags\": [\"{smgtag.ToLower()}\"]}}"
                    });
                    counter++;
                }
                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

                whereexpression += smgtagliststring + ")";
            }
        }

        [Obsolete]
        private static void ArticleTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> articletypelist)
        {
            //Activity Type
            if (articletypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string articletypestring = "";
                int counter = 1;

                foreach (var articletypeId in articletypelist)
                {
                    articletypestring += $"data @> @type{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "type" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"Type\": \"{articletypeId}\"}}"
                    });
                    counter++;
                }
                articletypestring = articletypestring.Remove(articletypestring.Length - 4);

                whereexpression += articletypestring + ")";
            }
        }

        [Obsolete]
        private static void ArticleSubTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> articlesubtypelist)
        {
            //Activity Type
            if (articlesubtypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string articlesubtypestring = "";
                int counter = 1;

                foreach (var articlesubtypeId in articlesubtypelist)
                {
                    articlesubtypestring += $"data @> @subtype{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "subtype" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"SubType\": \"{articlesubtypeId}\"}}"
                    });
                    counter++;
                }
                articlesubtypestring = articlesubtypestring.Remove(articlesubtypestring.Length - 4);

                whereexpression += articlesubtypestring + ")";
            }
        }

        [Obsolete]
        private static void DifficultyFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> difficultylist)
        {
            //Difficulty
            if (difficultylist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string difficultystring = "";
                int counter = 1;

                foreach (var difficultyId in difficultylist)
                {
                    difficultystring += $"data @> @difficulty{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "difficulty" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"Difficulty\": \"{difficultyId}\"}}"
                    });
                    counter++;
                }
                difficultystring = difficultystring.Remove(difficultystring.Length - 4);

                whereexpression += difficultystring + ")";
            }
        }

        [Obsolete]
        private static void HasLanguageFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> haslanguage)
        {
            //SmgTags Info
            if (haslanguage.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string langliststring = "";
                int counter = 1;

                foreach (var language in haslanguage)
                {
                    langliststring += $"data @> @haslang{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "haslang" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"HasLanguage\": [\"{language.ToLower()}\"]}}"
                    });
                    counter++;
                }
                langliststring = langliststring.Remove(langliststring.Length - 4);

                whereexpression = whereexpression + langliststring + ")";
            }
        }

        [Obsolete]
        private static void PoiTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> poitypelist)
        {
            //Activity Type
            if (poitypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var poitype in poitypelist)
                {
                    activitytypestring += $"data @> @type{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "type" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"SmgTags\": [\"{poitype.ToLower()}\"]}}"
                    });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        [Obsolete]
        private static void PoiSubTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> subtypelist)
        {
            //Activity Type
            if (subtypelist.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string activitytypestring = "";
                int counter = 1;

                foreach (var poitype in subtypelist)
                {
                    activitytypestring += $"data @> @subtype{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "subtype" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"SmgTags\": [\"{poitype.ToLower()}\"]}}"
                    });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        [Obsolete]
        private static void EventTopicFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> topicrids)
        {
            if (topicrids.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string topicliststring = "";
                int counter = 1;

                foreach (var topic in topicrids)
                {
                    topicliststring += $"data @> @topic{counter} OR ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "topic" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = $"{{ \"TopicRIDs\": [\"{topic}\"]}}"
                    });
                    counter++;
                }
                topicliststring = topicliststring.Remove(topicliststring.Length - 4);

                whereexpression = whereexpression + topicliststring + ")";
            }
        }

        [Obsolete]
        private static void EventOrganizerFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> orgidlist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "orgid" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"OrgRID\": [\"" + orgid + "\"]}"
                    });
                    counter++;
                }
                orgidliststring = orgidliststring.Remove(orgidliststring.Length - 4);

                whereexpression = whereexpression + orgidliststring + ")";
            }
        }

        [Obsolete]
        private static void EventRancFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> rancidlist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "rancid" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"Ranc\": [\"" + rancid + "\"]}"
                    });
                    counter++;
                }
                rancidliststring = rancidliststring.Remove(rancidliststring.Length - 4);

                whereexpression = whereexpression + rancidliststring + ")";
            }
        }

        [Obsolete]
        private static void EventTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> typeidlist)
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
                    parameters.Add(new PGParameters()
                    {

                        Name = "typeid" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"Type\": [\"" + typeid + "\"]}"
                    });
                    counter++;
                }
                typeidliststring = typeidliststring.Remove(typeidliststring.Length - 4);

                whereexpression = whereexpression + typeidliststring + ")";
            }
        }

        [Obsolete]
        private static void EventDateFilterWhereColumns(ref string whereexpression, IList<PGParameters> parameters, DateTime? begin, DateTime? end)
        {
            //Begin & End
            if (begin != null && end != null)
            {
                //Beide nicht null
                if (begin != DateTime.MinValue && end != DateTime.MaxValue)
                {
                    end = end.Value.AddHours(23).AddMinutes(59).AddSeconds(59);

                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "(begindate >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND begindate <= '" + String.Format("{0:yyyy-MM-dd}", end) + "') OR (enddate >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND enddate <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')";
                }
                //Begin ist DateTime Min
                if (begin == DateTime.MinValue && end != DateTime.MaxValue)
                {
                    end = end.Value.AddHours(23).AddMinutes(59).AddSeconds(59);

                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "(begindate > '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND begindate <= '" + String.Format("{0:yyyy-MM-dd}", end) + "') OR (enddate > '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND enddate <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')";
                }
                //End ist DateTime Max
                if (begin != DateTime.MinValue && end == DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = whereexpression + "(begindate >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND begindate < '" + String.Format("{0:yyyy-MM-dd}", end) + "') OR (enddate >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "' AND enddate < '" + String.Format("{0:yyyy-MM-dd}", end) + "')";
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

        [Obsolete]
        private static void EventDateFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, DateTime? begin, DateTime? end)
        {
            //Begin & End
            if (begin != null && end != null)
            {
                //Beide nicht null
                if (begin != DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = $"{whereexpression}(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '{begin:yyyy-MM-dd}') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') <= '{$"{end:yyyy-MM-dd}"}')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') >= '{$"{begin:yyyy-MM-dd}"}') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') <= '{$"{end:yyyy-MM-dd}"}')))";
                }
                //Begin ist DateTime Min
                if (begin == DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = $"{whereexpression}(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') > '{begin:yyyy-MM-dd}') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') <= '{$"{end:yyyy-MM-dd}"}')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') > '{$"{begin:yyyy-MM-dd}"}') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') <= '{$"{end:yyyy-MM-dd}"}')))";
                }
                //End ist DateTime Max
                if (begin != DateTime.MinValue && end == DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression = $"{whereexpression}(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '{begin:yyyy-MM-dd}') AND (to_date(data ->> 'DateBegin', 'YYYY-MM-DD') < '{$"{end:yyyy-MM-dd}"}')) OR ((to_date(data->> 'DateEnd', 'YYYY-MM-DD') >= '{$"{begin:yyyy-MM-dd}"}') AND(to_date(data->> 'DateEnd', 'YYYY-MM-DD') < '{$"{end:yyyy-MM-dd}"}')))";
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

        [Obsolete]
        private static void EventDateFilterWhereTest(
            ref string whereexpression, IList<PGParameters> parameters, DateTime? begin, DateTime? end)
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
                        parameters.Add(new PGParameters()
                        {
                            Name = "datebegin" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = $"{{ \"DateBegin\": \"{loopdate:yyyy-MM-dd}T00:00:00\"}}"
                        });
                        parameters.Add(new PGParameters()
                        {
                            Name = "dateend" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = $"{{ \"DateEnd\": \"{loopdate:yyyy-MM-dd}T00:00:00\"}}"
                        });

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

        [Obsolete]
        private static void EventFromNowFilter(
            ref string whereexpression, IList<PGParameters> parameter, bool fromnow)
        {

            //From Now
            if (fromnow)
            {

                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "(((to_date(data ->> 'DateBegin', 'YYYY-MM-DD') >= '" + $"{DateTime.Now:yyyy-MM-dd}" + "')))";
            }
        }

        [Obsolete]
        private static void EventDateFilterWhereWithNextBegin(ref string whereexpression, IList<PGParameters> parameters, DateTime? begin, DateTime? end)
        {
            //Begin & End
            if (begin != null && end != null)
            {

                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "(((to_date(data ->> 'NextBeginDate', 'YYYY-MM-DD') >= '" + String.Format("{0:yyyy-MM-dd}", begin) + "') AND (to_date(data ->> 'NextBeginDate', 'YYYY-MM-DD') <= '" + String.Format("{0:yyyy-MM-dd}", end) + "')))";

            }
        }

        [Obsolete]
        private static void SmgPoiTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> typelist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "type" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"Type\": \"" + activitytypeId + "\"}"
                    });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        [Obsolete]
        private static void SmgPoiSubTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> subtypelist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "subtype" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"SubType\": \"" + activitytypeId + "\"}"
                    });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        [Obsolete]
        private static void SmgPoiPoiTypeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> poitypelist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "poitype" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"PoiType\": \"" + activitytypeId + "\"}"
                    });
                    counter++;
                }
                activitytypestring = activitytypestring.Remove(activitytypestring.Length - 4);

                whereexpression = whereexpression + activitytypestring + ")";
            }
        }

        [Obsolete]
        private static void SourceFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> sourcelist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "source" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{\"SyncSourceInterface\": \"" + sourceid + "\" }"
                    });
                    counter++;
                }
                sourcestring = sourcestring.Remove(sourcestring.Length - 4);

                whereexpression = whereexpression + sourcestring + ")";
            }
        }

        [Obsolete]
        private static void CategoryCodeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> categorycodesids)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "categorycode" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{\"CategoryCodes\": [{ \"Id\": \"" + categorycode.ToUpper() + "\" }] }"
                    });
                    counter++;
                }
                categorycodeliststring = categorycodeliststring.Remove(categorycodeliststring.Length - 4);

                whereexpression = whereexpression + categorycodeliststring + ")";

            }
        }

        [Obsolete]
        private static void CeremonyCodeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> ceremonycodesids)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "ceremonycode" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{\"CapacityCeremony\": [{ \"Id\": \"" + ceremonycode.ToUpper() + "\" }] }"
                    });
                    counter++;
                }
                ceremonycodeliststring = ceremonycodeliststring.Remove(ceremonycodeliststring.Length - 4);

                whereexpression = whereexpression + ceremonycodeliststring + ")";

            }
        }

        [Obsolete]
        private static void CuisineCodeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> facilitycodesids)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "facilitycode" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{\"Facilities\": [{ \"Id\": \"" + facilitycode.ToUpper() + "\" }] }"
                    });
                    counter++;
                }
                facilitycodeliststring = facilitycodeliststring.Remove(facilitycodeliststring.Length - 4);

                whereexpression = whereexpression + facilitycodeliststring + ")";
            }
        }

        [Obsolete]
        private static void DishCodeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> dishcodesids)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "dishcode" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{\"DishRates\": [{ \"Id\": \"" + dishcode.ToUpper() + "\" }] }"
                    });
                    counter++;
                }
                dishcodeliststring = dishcodeliststring.Remove(dishcodeliststring.Length - 4);

                whereexpression = whereexpression + dishcodeliststring + ")";
            }
        }

        [Obsolete]
        private static void BoardFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> boardlist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "boardid" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"BoardIds\": [\"" + boardid + "\"]}"
                    });
                    counter++;
                }
                boardliststring = boardliststring.Remove(boardliststring.Length - 4);

                whereexpression = whereexpression + boardliststring + ")";
            }
        }

        [Obsolete]
        private static void BadgeFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> badgelist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "badgeid" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"BadgeIds\": [\"" + badgeid + "\"]}"
                    });
                    counter++;
                }
                badgeliststring = badgeliststring.Remove(badgeliststring.Length - 4);

                whereexpression = whereexpression + badgeliststring + ")";
            }
        }

        [Obsolete]
        private static void CategoryFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> categorylist)
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
                        parameters.Add(new PGParameters()
                        {
                            Name = "categoryid" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = "{ \"AccoCategoryId\": \"" + categoryid + "\"}"
                        });
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

        [Obsolete]
        private static void ThemeFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyDictionary<string, bool> themelist)
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
                    themeliststring += $"data @> @themeid{counter} AND ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "themeid" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"ThemeIds\": [\"" + themeid.Key + "\"]}"
                    });
                    counter++;
                }
                themeliststring = themeliststring.Remove(themeliststring.Length - 4);

                whereexpression = whereexpression + themeliststring + ")";
            }
        }

        [Obsolete]
        private static void FeatureFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyDictionary<string, bool> featurelist)
        {
            //Feature Info
            if (featurelist.Where(x => x.Value == true).Count() > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string featureliststring = "";
                int counter = 1;

                foreach (var featureid in featurelist.Where(x => x.Value == true))
                {
                    featureliststring += $"data @> @featureid{counter} AND ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "featureid" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"SpecialFeaturesIds\": [\"" + featureid.Key + "\"]}"
                    });
                    counter++;
                }
                featureliststring = featureliststring.Remove(featureliststring.Length - 4);

                whereexpression = whereexpression + featureliststring + ")";
            }
        }

        [Obsolete]
        private static void AccoTypeFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> accotypelist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "accotypeid",
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{\"AccoTypeId\": \"" + accotypelist.FirstOrDefault() + "\" }"
                    });
                }
                else
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression += " AND (";
                    else
                        whereexpression += "(";

                    int counter = 1;
                    string categoryliststring = "";
                    foreach (var accotypeid in accotypelist)
                    {
                        categoryliststring += $"data @> @accotypeid{counter} OR ";
                        parameters.Add(new PGParameters()
                        {
                            Name = "accotypeid" + counter,
                            Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                            Value = "{ \"AccoTypeId\": \"" + accotypeid + "\"}"
                        });
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

        [Obsolete]
        private static void ApartmentFilter(
            ref string whereexpression, IList<PGParameters> parameters, bool apartmentfilter)
        {
            //Apartment
            if (apartmentfilter)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @hasapartment";
                parameters.Add(new PGParameters()
                {
                    Name = "hasapartment",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = "{ \"HasApartment\" : " + apartmentfilter.ToString().ToLower() + "}"
                });
            }
        }

        [Obsolete]
        private static void BookableFilter(
            ref string whereexpression, IList<PGParameters> parameters, bool? bookable)
        {
            //Bookable
            if (bookable != null)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "data @> @isbookable";
                parameters.Add(new PGParameters()
                {
                    Name = "isbookable",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = "{ \"IsBookable\" : " + (bookable ?? false).ToString().ToLower() + " }"
                });
            }
        }

        [Obsolete]
        private static void TypesToExcludeFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> typestoexclude)
        {
            //Activity Type
            if (typestoexclude.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND (";
                else
                    whereexpression += "(";

                string typestring = "";
                int counter = 1;

                foreach (var typeId in typestoexclude)
                {
                    typestring += $"NOT data @> @typestoexclude{counter} AND ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "typestoexclude" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"Type\": \"" + typeId + "\"}"
                    });
                    counter++;
                }
                typestring = typestring.Remove(typestring.Length - 4);

                whereexpression = whereexpression + typestring + ")";
            }
        }

        [Obsolete]
        private static void SubTypesToExcludeFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> subtypestoexclude)
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
                    subtypeliststring += $"NOT data @> @subtypestoexclude{counter} AND ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "subtypestoexclude" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"SubType\": \"" + subtype + "\"}"
                    });
                    counter++;
                }
                subtypeliststring = subtypeliststring.Remove(subtypeliststring.Length - 4);

                whereexpression = whereexpression + subtypeliststring + ")";
            }
        }

        [Obsolete]
        private static void PoiTypesToExcludeFilter(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> poitypestoexclude)
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
                    poitypeliststring += $"NOT data @> @poitypetoexclude{counter} AND ";
                    parameters.Add(new PGParameters()
                    {
                        Name = "poitypetoexclude" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"PoiType\": \"" + poitype + "\"}"
                    });
                    counter++;
                }
                poitypeliststring = poitypeliststring.Remove(poitypeliststring.Length - 4);

                whereexpression = whereexpression + poitypeliststring + ")";
            }
        }

        [Obsolete]
        private static void PackagesThemeFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> themelist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "theme" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"PackageThemeList\": [\"" + theme + "\"]}"
                    });
                    counter++;
                }
                smgtagliststring = smgtagliststring.Remove(smgtagliststring.Length - 4);

                whereexpression = whereexpression + smgtagliststring + ")";
            }
        }

        [Obsolete]
        private static void PackagesBoardFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> boardlist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "board" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"Services\": [\"" + boardid + "\"]}"
                    });
                    counter++;
                }
                boardliststring = boardliststring.Remove(boardliststring.Length - 4);

                whereexpression = whereexpression + boardliststring + ")";
            }
        }

        [Obsolete]
        private static void PackagesAccoFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> accolist)
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
                    parameters.Add(new PGParameters()
                    {
                        Name = "accoid" + counter,
                        Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                        Value = "{ \"HotelId\": [\"" + accoid + "\"]}"
                    });
                    counter++;
                }
                accoliststring = accoliststring.Remove(accoliststring.Length - 4);

                whereexpression = whereexpression + accoliststring + ")";
            }
        }

        [Obsolete]
        private static void PackagesStayFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, bool shortstay, bool longstay)
        {
            //Shortstay
            if (shortstay)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @shortstay";
                parameters.Add(new PGParameters()
                {
                    Name = "shortstay",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = "{ \"ShortStay\" : " + shortstay.ToString().ToLower() + "}"
                });
            }

            //Longstay
            if (longstay)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression = whereexpression + "data @> @longstay";
                parameters.Add(new PGParameters()
                {
                    Name = "longstay",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = "{ \"LongStay\" : " + longstay.ToString().ToLower() + "}"
                });
            }
        }

        [Obsolete]
        private static void PackagesValidFromFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, DateTime validfrom, DateTime validto)
        {
            //Datum von bis valid
            if (validfrom != DateTime.MinValue)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression += $"to_date(data ->> 'ValidStart', 'YYYY-MM-DD') >= '{validfrom:yyyy-MM-dd}'";
            }
            if (validto != DateTime.MaxValue)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression += $"to_date(data ->> 'ValidStop', 'YYYY-MM-DD') <= '{validto:yyyy-MM-dd}'";
            }
        }

        [Obsolete]
        private static void EventShortActiveFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, string activefilter)
        {

            //Active
            if (!String.IsNullOrEmpty(activefilter))
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression = whereexpression + " AND ";

                whereexpression += "data @> @activefilter";
                parameters.Add(new PGParameters()
                {
                    Name = "activefilter",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = "{ \"Display1\" :  \"" + activefilter.ToString() + "\" }"
                });
            }
        }

        [Obsolete]
        private static void EventShortSourceFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, string source)
        {
            //Source
            if (!String.IsNullOrEmpty(source))
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "data @> @sourcefilter";
                parameters.Add(new PGParameters()
                {
                    Name = "sourcefilter",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = "{ \"Source\" : \"" + source.ToString() + "\" }"
                });
            }
        }

        [Obsolete]
        private static void EventShortEventLocationFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, string eventlocation)
        {
            //EventLocation
            if (!String.IsNullOrEmpty(eventlocation))
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "data @> @eventlocation";
                parameters.Add(new PGParameters()
                {
                    Name = "eventlocation",
                    Type = NpgsqlTypes.NpgsqlDbType.Jsonb,
                    Value = "{ \"EventLocation\" : \"" + eventlocation.ToString() + "\" }"
                });
            }

        }

        [Obsolete]
        private static void EventShortBeginEndFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, DateTime start, DateTime end)
        {
            //Begin & End
            if (start != null && end != null)
            {
                if (start != DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression += $"(((to_date(data ->> 'StartDate', 'YYYY-MM-DD') >= '{start:yyyy-MM-dd}') AND (to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '{$"{end:yyyy-MM-dd}"}')))";
                }
                else if (start != DateTime.MinValue && end == DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression += $"((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '{start:yyyy-MM-dd}'))";
                }
                else if (start == DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression += $"((to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '{end:yyyy-MM-dd}')))";
                }
            }
        }

        [Obsolete]
        private static void EventShortBeginEndSpecialFilterWhere(
            ref string whereexpression, IList<PGParameters> parameters, DateTime start, DateTime end)
        {
            //Begin & End
            if (start != null && end != null)
            {
                if (start != DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression += $"(((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '{start:yyyy-MM-dd}') AND (to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '{$"{end:yyyy-MM-dd}"}')))";
                }
                else if (start != DateTime.MinValue && end == DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression += $"((to_date(data ->> 'EndDate', 'YYYY-MM-DD') >= '{start:yyyy-MM-dd}'))";
                }
                else if (start == DateTime.MinValue && end != DateTime.MaxValue)
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression = whereexpression + " AND ";

                    whereexpression += $"((to_date(data->> 'EndDate', 'YYYY-MM-DD') <= '{end:yyyy-MM-dd}')))";
                }

            }
        }

        [Obsolete]
        private static void AlpineBitsMessageFilterWhere(ref string whereexpression, List<PGParameters> parameters, string messagetype)
        {
            //Highlight
            if (!String.IsNullOrEmpty(messagetype))
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "data @> @MessageType";
                parameters.Add(new PGParameters() { Name = "messagetype", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"MessageType\" : \"" + messagetype.ToString() + "\"}" });
            }
        }

        [Obsolete]
        private static void AlpineBitsSourceFilterWhere(ref string whereexpression, IList<PGParameters> parameters, string source)
        {
            //Highlight
            if (!String.IsNullOrEmpty(source))
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                whereexpression += "data @> @Source";
                parameters.Add(new PGParameters() { Name = "source", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"Source\": \"" + source.ToString() + "\"}" });
            }
        }

        [Obsolete]
        private static void AlpineBitsAccommodationIdFilterWhere(ref string whereexpression, IList<PGParameters> parameters, IReadOnlyCollection<string> accommodationids)
        {
            //AccoType Info schaugn ob des geat!!! umgekearter foll
            if (accommodationids.Count > 0)
            {
                if (!String.IsNullOrEmpty(whereexpression))
                    whereexpression += " AND ";

                //Tuning force to use GIN Index
                if (accommodationids.Count == 1)
                {
                    whereexpression += "data @> @accoid";
                    parameters.Add(new PGParameters() { Name = "accoid", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"AccommodationId\": \"" + accommodationids.FirstOrDefault() + "\"}" });
                }
                else
                {
                    if (!String.IsNullOrEmpty(whereexpression))
                        whereexpression += " AND (";
                    else
                        whereexpression += "(";

                    int counter = 1;
                    string categoryliststring = "";
                    foreach (var accoid in accommodationids)
                    {
                        categoryliststring += $"data @> @accoid {counter} OR ";
                        parameters.Add(new PGParameters() { Name = "accoid" + counter, Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{ \"AccommodationId\": \"" + accoid + "\"}" });
                        counter++;
                    }
                    categoryliststring = categoryliststring.Remove(categoryliststring.Length - 4);

                    whereexpression += categoryliststring + ")";
                }
            }
        }

        #endregion


    }
}
