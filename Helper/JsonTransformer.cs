using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using DataModel;

namespace Helper
{
    public static class JsonTransformer
    {              
        public static JsonRaw? TransformRawData(
            this JsonRaw raw, string? language, string[] fields, bool checkCC0,
            bool filterClosedData, Func<string, string> urlGenerator, IEnumerable<string> userroles)
        {
            JToken? token = JToken.Parse(raw.Value);
            if (language != null) token = JsonTransformerMethods.FilterByLanguage(token, language);
            if (fields != null && fields.Length > 0) token = JsonTransformerMethods.FilterByFields(token, fields, language);
            // Filter out all data where the LicenseInfo does not contain `CC0`
            if (checkCC0) token = JsonTransformerMethods.FilterImagesByCC0License(token);
            // Filter out all data where the LicenseInfo contains `hgv` as source.
            if (checkCC0) token = JsonTransformerMethods.FilterAccoRoomInfoByHGVSource(token);

            //Filter out all Data 
            var rolefilter = FilterOutPropertiesByRole(userroles);
            if (rolefilter.Count > 0)
                if (checkCC0) token = JsonTransformerMethods.FilterOutProperties(token, rolefilter);
            
            if (filterClosedData) token = token.FilterClosedData();
            
            token = token.TransformSelfLink(urlGenerator);
            token = token.FilterMetaInformations();
            return (token == null) ?
                null :
                new JsonRaw(token.ToString(Formatting.Indented));
        }        

        public static List<string> FilterOutPropertiesByRole(IEnumerable<string> userroles)
        {
            if (userroles.Contains("IDM"))
                return new List<string>() { };                

            return new List<string>() { "TVMember", "Beds", "Units" };
        }
    }    

    sealed class DistinctComparer
          : IEqualityComparer<(string name, string path)>
    {
        public bool Equals([AllowNull] (string name, string path) x, [AllowNull] (string name, string path) y) =>
            x.name == y.name;

        public int GetHashCode([DisallowNull] (string name, string path) obj) =>
            obj.name.GetHashCode();
    }

}
