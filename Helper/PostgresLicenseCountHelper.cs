// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Npgsql;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class PostgresLicenseCountHelper
    {
        public static async Task<long> GetTotalCount(QueryFactory QueryFactory, string tablename)
        {
            var query = QueryFactory.Query()
                .SelectRaw("id")
                .From(tablename);

            return await query.CountAsync<long>();
        }

        public static async Task<long> GetTotalCountOpendata(QueryFactory QueryFactory, string tablename)
        {
            var query = QueryFactory.Query()
                .SelectRaw("id")
                .From(tablename)
                .WhereRaw("gen_licenseinfo_closeddata IS NULL")
                .OrWhereRaw("gen_licenseinfo_closeddata = false");

            return await query.CountAsync<long>();
        }

        public static async Task<long> GetTotalCountCC0(QueryFactory QueryFactory, string tablename)
        {
            //string whereexpression = "data @> '{\"LicenseInfo\": { \"License\": \"CC0\" } }'";

            var query = QueryFactory.Query()
                .SelectRaw("id")
                .From(tablename)
                .WhereInJsonb(
                    new List<string>() { "CC0" },
                    tag => new { LicenseInfo = new[] { new { License = tag } } });

            return await query.CountAsync<long>();
        }

        public static async Task<long> GetAllDataWithCC0Image(QueryFactory QueryFactory, string tablename)
        {
            //string whereexpression = "data @> {\"ImageGallery\": [{ \"License\": \"CC0\" }] }";

            //Works but obsolete
            var query = QueryFactory.Query()
                .SelectRaw("id")
                .From(tablename)
                .WhereInJsonb(
                    new List<string>() { "CC0" },
                    tag => new { ImageGallery = new[] { new { License = tag } } });

            //always returning 0
            //var query = QueryFactory.Query()
            //    .SelectRaw("id")
            //    .From(tablename)
            //    .WhereInJsonb(
            //        new List<string>() { "CC0" },
            //        "ImageGallery->>License",
            //        tag => tag);

            return await query.CountAsync<long>();
        }

        public static async Task<long> GetAllImagesWithCC0License(QueryFactory QueryFactory, string tablename)
        {

            //select count(result1) from(select jsonb_array_elements(data-> 'ImageGallery') as result1 from accommodations where
            //data @> '{ "ImageGallery": [{ "License": "CC0" }] }') as subsel where result1 ->> 'License' like 'CC0'

            //var query = new Query().From("Users")
            //    .Select("Id")
            //    .Select(q => q.From("Visits").Where("UserId", 10).Count(), "VisitsCount");

            var subquery = QueryFactory.Query()
                .SelectRaw("jsonb_array_elements(data -> 'ImageGallery') as result1")
                .From(tablename)
                .WhereInJsonb(
                    new List<string>() { "CC0" },
                    tag => new { ImageGallery = new[] { new { License = tag } } });

            var query = QueryFactory.Query()
                .Select(subquery, "result1")
                .From(subquery, "subsel")
                .WhereRaw("result1 ->> 'License' like 'CC0'");
               

            return await query.CountAsync<long>();
        }

        public static async Task<long> GetAllImagesWithNONCC0License(QueryFactory QueryFactory, string tablename)
        {
            var subquery = QueryFactory.Query()
                .SelectRaw("jsonb_array_elements(data -> 'ImageGallery') as result1")
                .From(tablename)
                .WhereRaw("data ->> 'ImageGallery' is not null AND data -> 'ImageGallery' != '[]'");

            var query = QueryFactory.Query()
                .Select(subquery, "result1")
                .From(subquery, "subsel")
                .WhereRaw("result1 ->> 'License' not like 'CC0' OR result1 ->> 'License' is null");

            return await query.CountAsync<long>();
        }
    }
}
