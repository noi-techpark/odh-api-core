using Helper;
using SqlKata;
using SqlKata.Compilers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OdhApiCoreTests.Helper
{
    public class PostgresSQLWhereBuilderTests
    {
        private readonly static PostgresCompiler compiler = new PostgresCompiler();

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
            var query =
                new Query()
                    .From("activities")
                    .ActivityWhereExpression(
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
                        lastchange: null
                    );

            var result = compiler.Compile(query);

            Assert.Equal("SELECT * FROM \"activities\"", result.RawSql);
            Assert.Empty(result.Bindings);
        }

        [Fact]
        public void CreateActivityWhereExpression_Test()
        {
            var query =
                new Query()
                    .From("activities")
                    .ActivityWhereExpression(
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
                        lastchange: null
                    );

            var result = compiler.Compile(query);

            Assert.Equal(
                "SELECT * FROM \"activities\" WHERE (\"id\" = ? OR \"id\" = ?) AND (data @> jsonb(?)) AND (data @> jsonb(?)) AND (data @> jsonb(?)) AND data @> jsonb(?)",
                result.RawSql
            );

            Assert.Equal(6, result.Bindings.Count());
            Assert.Equal("ID1", result.NamedBindings["@p0"]);
            Assert.Equal("ID2", result.NamedBindings["@p1"]);
            Assert.Equal("{\"LocationInfo\":{\"RegionInfo\":{\"Id\":\"region1\"}}}", result.NamedBindings["@p2"]);
            Assert.Equal("{\"AreaId\":[\"area1\"]}", result.NamedBindings["@p3"]);
            Assert.Equal("{\"Type\":\"1024\"}", result.NamedBindings["@p4"]);
            Assert.Equal("{\"Highlight\":true}", result.NamedBindings["@p5"]);
        }
    }
}
