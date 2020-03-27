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
        public void CreateActivityWhereExpression_EmptyParameters()
        {
            var (where, parameters) =
                PostgresSQLWhereBuilder.CreateActivityWhereExpression(
                    idlist: System.Array.Empty<string>(),
                    activitytypelist: System.Array.Empty<string>(),
                    subtypelist: System.Array.Empty<string>(),
                    difficultylist: System.Array.Empty<string>(),
                    smgtaglist: System.Array.Empty<string>(),
                    districtlist: System.Array.Empty<string>(),
                    municipalitylist: System.Array.Empty<string>(),
                    tourismvereinlist: System.Array.Empty<string>(),
                    regionlist: System.Array.Empty<string>(),
                    arealist: System.Array.Empty<string>(),
                    distance: false,
                    distancemin: 0,
                    distancemax: 0,
                    duration: false,
                    durationmin: 0,
                    durationmax: 0,
                    altitude: false,
                    altitudemin: 0,
                    altitudemax: 0,
                    highlight: null,
                    activefilter: null,
                    smgactivefilter: null,
                    searchfilter: null,
                    language: null,
                    lastchange: null);
            Assert.Equal("", where);
            Assert.Empty(parameters);
        }

        [Fact]
        public void CreateActivityWhereExpression_Test()
        {
            var (where, parameters) =
                PostgresSQLWhereBuilder.CreateActivityWhereExpression(
                    idlist: new string[] { "id1", "id2" },
                    activitytypelist: new string[] { "1024" },
                    subtypelist: System.Array.Empty<string>(),
                    difficultylist: System.Array.Empty<string>(),
                    smgtaglist: System.Array.Empty<string>(),
                    districtlist: System.Array.Empty<string>(),
                    municipalitylist: System.Array.Empty<string>(),
                    tourismvereinlist: System.Array.Empty<string>(),
                    regionlist: new string[] { "region1" },
                    arealist: new string[] { "area1" },
                    distance: false,
                    distancemin: 0,
                    distancemax: 0,
                    duration: false,
                    durationmin: 0,
                    durationmax: 0,
                    altitude: false,
                    altitudemin: 0,
                    altitudemax: 0,
                    highlight: true,
                    activefilter: null,
                    smgactivefilter: null,
                    searchfilter: null,
                    language: null,
                    lastchange: null);

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
