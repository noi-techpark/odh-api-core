// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helper
{
    public static class JsonTransformer
    {
        public static JsonRaw? TransformRawData(
            this JsonRaw raw,
            string? language,
            string[] fields,
            bool filteroutNullValues,
            Func<string, string> urlGenerator,
            IEnumerable<string>? fieldstohide
        )
        {
            JToken? token = JToken.Parse(raw.Value);
            //Filter out not desired langugae fields
            if (language != null)
                token = JsonTransformerMethods.FilterByLanguage(token, language);
            //Filter by given fields
            if (fields != null && fields.Length > 0)
                token = JsonTransformerMethods.FilterByFields(token, fields, language);
            // Filter out all data where the LicenseInfo does not contain `CC0`
            // Show all images
            //if (checkCC0) token = JsonTransformerMethods.FilterImagesByCC0License(token);
            // Filter out all data where the LicenseInfo contains `hgv` as source.
            //if (filterClosedData) token = JsonTransformerMethods.FilterAccoRoomInfoByHGVSource(token);
            //Filter out all Data
            //var rolefilter = FilterOutPropertiesByRole(userroles);
            //if (rolefilter.Count > 0)
            //    if (checkCC0) token = JsonTransformerMethods.FilterOutProperties(token, rolefilter);

            if (fieldstohide != null && fieldstohide.Count() > 0)
                token = JsonTransformerMethods.FilterOutProperties(token, fieldstohide.ToList());

            //if (filterClosedData) token = token.FilterClosedData();

            //Ensure Self Link is the right url
            if(urlGenerator != null)
                token = token.TransformSelfLink(urlGenerator);

            if (filteroutNullValues)
                token = JsonTransformerMethods.FilterOutNullProperties(token);

            //Filter out meta info
            //token = token.FilterMetaInformations();

            return (token == null) ? null : new JsonRaw(token.ToString(Formatting.Indented));
        }

        public static List<string> FilterOutPropertiesByRole(IEnumerable<string> userroles)
        {
            if (userroles.Contains("IDM"))
                return new List<string>() { };

            return new List<string>()
            {
                "TVMember",
                "Beds",
                "Units",
                "RepresentationRestriction",
                "TrustYouID",
                "TrustYouScore",
                "TrustYouState",
                "TrustYouResults",
                "TrustYouActive",
            };
        }
    }

    sealed class DistinctComparer : IEqualityComparer<(string name, string path)>
    {
        public bool Equals(
            [AllowNull] (string name, string path) x,
            [AllowNull] (string name, string path) y
        ) => x.name == y.name;

        public int GetHashCode([DisallowNull] (string name, string path) obj) =>
            obj.name.GetHashCode();
    }
}
