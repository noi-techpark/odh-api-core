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
                        lastchange: null,
                        filterClosedData: false
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
                        lastchange: null,
                        filterClosedData: false
                    );

            var result = compiler.Compile(query);

            Assert.Equal(
                "SELECT * FROM \"activities\" WHERE (\"id\" = ? OR \"id\" = ?) AND data#>>'{LocationInfo,RegionInfo,Id}' = ANY(?) AND (data @> jsonb(?)) AND (data#>>'{Type}' = ?) AND data#>>'{Highlight}' = ?",
                result.RawSql
            );

            Assert.Equal(6, result.Bindings.Count());
            Assert.Equal("ID1", result.NamedBindings["@p0"]);
            Assert.Equal("ID2", result.NamedBindings["@p1"]);
            Assert.Equal(new[] { "region1" }, result.NamedBindings["@p2"]);
            Assert.Equal("{\"AreaId\":[\"area1\"]}", result.NamedBindings["@p3"]);
            Assert.Equal("1024", result.NamedBindings["@p4"]);
            Assert.Equal("true", result.NamedBindings["@p5"]);
        }
    }
}
