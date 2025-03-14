// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Helper;
using SqlKata;
using SqlKata.Compilers;
using Xunit;

namespace OdhApiCoreTests.Helper
{
    public class PostgresSQLWhereBuilderTests
    {
        private static readonly PostgresCompiler compiler = new();

        [Fact]
        public void CreateActivityWhereExpression_LoggedUser()
        {
            var query = new Query()
                .From("activities")
                .ActivityWhereExpression(
                    languagelist: System.Array.Empty<string>(),
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
                    additionalfilter: null,
                    userroles: new List<string>() { "STA" }
                );

            var result = compiler.Compile(query);

            //Assert.Equal("SELECT * FROM \"activities\" WHERE ((gen_source <> 'lts') OR (gen_source = 'lts' AND gen_reduced = $$))", result.RawSql);
            Assert.Equal(
                "SELECT * FROM \"activities\" WHERE gen_access_role @> array\\[$$\\]",
                result.RawSql
            );
        }

        [Fact]
        public void CreateActivityWhereExpression_Anonymous()
        {
            var query = new Query()
                .From("activities")
                .ActivityWhereExpression(
                    languagelist: System.Array.Empty<string>(),
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
                    additionalfilter: null,
                    new List<string>() { "ANONYMOUS" }
                );

            var result = compiler.Compile(query);

            //Assert.Equal("SELECT * FROM \"activities\" WHERE ((gen_source <> 'lts' AND (gen_licenseinfo_closeddata IS NULL OR gen_licenseinfo_closeddata = $$)) OR (gen_source = 'lts' AND gen_reduced = true AND ((gen_licenseinfo_closeddata IS NULL OR gen_licenseinfo_closeddata = $$))))", result.RawSql);
            Assert.Equal(
                "SELECT * FROM \"activities\" WHERE gen_access_role @> array\\[$$\\]",
                result.RawSql
            );
        }

        [Fact]
        public void CreateActivityWhereExpression_IDMUser()
        {
            var query = new Query()
                .From("activities")
                .ActivityWhereExpression(
                    languagelist: System.Array.Empty<string>(),
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
                    additionalfilter: null,
                    userroles: new List<string>() { "IDM" }
                );

            var result = compiler.Compile(query);

            //Assert.Equal("SELECT * FROM \"activities\" WHERE ((gen_source <> 'lts') OR (gen_source = 'lts' AND gen_reduced = $$))", result.RawSql);
            Assert.Equal(
                "SELECT * FROM \"activities\" WHERE gen_access_role @> array\\[$$\\]",
                result.RawSql
            );
        }
    }
}
