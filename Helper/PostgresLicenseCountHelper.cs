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
        public static async Task<long> GetAllLicenses(QueryFactory QueryFactory, string tablename)
        {
            List<PGParameters> parameters = new List<PGParameters>();

            string whereexpression = "data @> '{\"ImageGallery\": [{ \"License\": \"CC0\" }] }'";
            //parameters.Add(new PGParameters() { Name = "gallerylicense", Type = NpgsqlTypes.NpgsqlDbType.Jsonb, Value = "{\"ImageGallery\": [{ \"License\": \"CC0\" }] }" });

            var query = QueryFactory.Query()
                .SelectRaw("id")
                .From(tablename)
                .WhereRaw(whereexpression);

            return await query.CountAsync<long>();

            //Group by not working
            //var accocount = PostgresSQLHelper.CountDataFromTableParametrized(conn, tablename, whereexpression, parameters);

            //return accocount;
        }

        public static async Task<long> GetAllImagesWithLicenseCount(QueryFactory QueryFactory, string tablename)
        {
            string selectexpression = "Count(*) from (select jsonb_array_elements(data -> 'ImageGallery') as result1";
            string whereexpression = "data @> '{ \"ImageGallery\" : [ { \"License\" : \"CC0\" } ] }') as result1 where result1 ->> 'License' like 'CC0'";

            var query = QueryFactory.Query()
                .SelectRaw(selectexpression)
                .From(tablename)
                .WhereRaw(whereexpression);

            return await query.CountAsync<long>();
        }
    }
}
