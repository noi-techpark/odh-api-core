using Helper;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OdhApiCoreTests.Helper
{
    public class PostgresSQLWhereBuilderTests
    {
        [Fact]
        public void CreateIdListWhereExpression_SingleId()
        {
            var (where, parameters) = PostgresSQLWhereBuilder.CreateIdListWhereExpression("hello");
            Assert.Equal("id LIKE @id", where);
            Assert.Single(parameters);
            var single = parameters.Single();
            Assert.Equal("id", single.Name);
            Assert.Equal(NpgsqlTypes.NpgsqlDbType.Text, single.Type);
            Assert.Equal("hello", single.Value);
        }

        [Fact]
        public void CreateIdListWhereExpression_IdList()
        {
            var idlist = new string[] {
                "foo", "bar", "baz"
            };
            var (where, parameters) =
                PostgresSQLWhereBuilder.CreateIdListWhereExpression(idlist);
            Assert.Equal("Id in (@id1, @id2, @id3)", where);
            Assert.Equal(3, parameters.Count());
            for (var i = 0; i < parameters.Count(); i++)
            {
                var param = parameters.ElementAt(i);
                Assert.Equal($"id{i + 1}", param.Name);
                Assert.Equal(idlist[i], param.Value);
            }
        }

        [Fact]
        public void CreateIdListWhereExpression_EmptyIdListWithDummy()
        {
            var idlist = new List<string>();

            var (where, parameters) =
                PostgresSQLWhereBuilder.CreateIdListWhereExpression(idlist, true);
            Assert.Equal("Id = @dummy", where);
            Assert.Single(parameters);

            var (where2, parameters2) =
                PostgresSQLWhereBuilder.CreateIdListWhereExpression(idlist, false);
            Assert.Equal("", where2);
            Assert.Empty(parameters2);
        }

        [Fact]
        public void CreateLastChangedWhereExpression_Test()
        {
            var (where, parameters) =
                PostgresSQLWhereBuilder.CreateLastChangedWhereExpression("hello");
            Assert.Equal("to_date(data ->> 'LastChange', 'YYYY-MM-DD') > @date", where);
            Assert.Single(parameters);
            var single = parameters.Single();
            Assert.Equal("date", single.Name);
            Assert.Equal(NpgsqlTypes.NpgsqlDbType.Date, single.Type);
            Assert.Equal("hello", single.Value);
        }

        [Fact]
        public void CreateActivityWhereExpression_EmptyParameters()
        {
            IReadOnlyCollection<string> idlist = new string[] { };
            IReadOnlyCollection<string> activitytypelist = new string[] { };
            IReadOnlyCollection<string> suttypelist = new string[] { };
            IReadOnlyCollection<string> difficultylist = new string[] { };
            IReadOnlyCollection<string> smgtaglist = new string[] { };
            IReadOnlyCollection<string> distictlist = new string[] { };
            IReadOnlyCollection<string> municipalitylist = new string[] { };
            IReadOnlyCollection<string> tourismvereinlist = new string[] { };
            IReadOnlyCollection<string> regionlist = new string[] { };
            IReadOnlyCollection<string> arealist = new string[] { };
            bool distance = false;
            int distancemin = 0;
            int distancemax = 0;
            bool duration = false;
            int durationmin = 0;
            int durationmax = 0;
            bool altitude = false;
            int altitudemin = 0;
            int altitudemax = 0;
            bool? highlight = null;
            bool? activefilter = null;
            bool? smgactivefilter = null;
            var (where, parameters) =
                PostgresSQLWhereBuilder.CreateActivityWhereExpression(
                    idlist, activitytypelist, suttypelist, difficultylist, smgtaglist, distictlist, municipalitylist,
                    tourismvereinlist, regionlist, arealist, distance, distancemin, distancemax, duration, durationmin,
                    durationmax, altitude, altitudemin, altitudemax, highlight, activefilter, smgactivefilter);
            Assert.Equal("", where);
            Assert.Empty(parameters);
        }

        [Fact]
        public void CreateActivityWhereExpression_Test()
        {
            IReadOnlyCollection<string> idlist = new string[] {
                "id1", "id2"
            };
            IReadOnlyCollection<string> activitytypelist = new string[] {
                "1024"
            };
            IReadOnlyCollection<string> suttypelist = new string[] { };
            IReadOnlyCollection<string> difficultylist = new string[] { };
            IReadOnlyCollection<string> smgtaglist = new string[] { };
            IReadOnlyCollection<string> distictlist = new string[] { };
            IReadOnlyCollection<string> municipalitylist = new string[] { };
            IReadOnlyCollection<string> tourismvereinlist = new string[] { };
            IReadOnlyCollection<string> regionlist = new string[] {
                "region1"
            };
            IReadOnlyCollection<string> arealist = new string[] {
                "area1"
            };
            bool distance = false;
            int distancemin = 0;
            int distancemax = 0;
            bool duration = false;
            int durationmin = 0;
            int durationmax = 0;
            bool altitude = false;
            int altitudemin = 0;
            int altitudemax = 0;
            bool? highlight = true;
            bool? activefilter = null;
            bool? smgactivefilter = null;
            var (where, parameters) =
                PostgresSQLWhereBuilder.CreateActivityWhereExpression(
                    idlist, activitytypelist, suttypelist, difficultylist, smgtaglist, distictlist, municipalitylist,
                    tourismvereinlist, regionlist, arealist, distance, distancemin, distancemax, duration, durationmin,
                    durationmax, altitude, altitudemin, altitudemax, highlight, activefilter, smgactivefilter);

            Assert.Equal("id in (@id1, @id2) AND data @> @regionid AND (data @> @area1) AND (data @> @type1) AND data @> @highlight", where);

            Assert.Equal(6, parameters.Count());
            var values = parameters.Select(p => (p.Name, p.Value, p.Type));
            Assert.Contains(("id1", "ID1", NpgsqlTypes.NpgsqlDbType.Text), values);
            Assert.Contains((("id2", "ID2", NpgsqlTypes.NpgsqlDbType.Text)), values);
            Assert.Contains(("regionid", "{\"LocationInfo\" : { \"RegionInfo\": { \"Id\": \"region1\" } } }", NpgsqlTypes.NpgsqlDbType.Jsonb), values);
            Assert.Contains(("type1", "{ \"Type\": \"1024\"}", NpgsqlTypes.NpgsqlDbType.Jsonb), values);
            Assert.Contains(("area1", "{ \"AreaId\": [\"area1\"]}", NpgsqlTypes.NpgsqlDbType.Jsonb), values);
            Assert.Contains(("highlight", "{ \"Highlight\" : true}", NpgsqlTypes.NpgsqlDbType.Jsonb), values);
        }
    }
}
