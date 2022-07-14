using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace Helper
{
    public class IdGenerator
    {
        /// <summary>
        /// Translates a ODH Type Object to the Type (Metadata) as String
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="odhtype"></param>
        /// <returns></returns>
        public static string GenerateIDFromType<T>(T odhtype)
        {
            return CreateGUID(GetIDStyle(odhtype));
        }

        public static void CheckIdFromType<T>(T odhtype) where T : IIdentifiable
        {
            var style = GetIDStyle(odhtype);

            if (style == IDStyle.uppercase)
                odhtype.Id = odhtype.Id.ToUpper();
            else if (style == IDStyle.lowercase)
                odhtype.Id = odhtype.Id.ToLower();
        }

        private static string CreateGUID(IDStyle style)
        {
            var id = System.Guid.NewGuid().ToString();

            if (style == IDStyle.uppercase)
                id = id.ToUpper();
            else if(style == IDStyle.lowercase)
                id = id.ToLower();

            return id;
        }        

        public static IDStyle GetIDStyle<T>(T odhtype)
        {
            return odhtype switch
            {
                Accommodation or AccommodationLinked => IDStyle.uppercase,
                AccoRoom or AccommodationRoomLinked => IDStyle.uppercase,
                GBLTSActivity or LTSActivityLinked => IDStyle.uppercase,
                GBLTSPoi or LTSPoiLinked => IDStyle.uppercase,
                Gastronomy or GastronomyLinked => IDStyle.uppercase,
                Event or EventLinked => IDStyle.uppercase,
                ODHActivityPoi or ODHActivityPoiLinked => IDStyle.lowercase,
                Package or PackageLinked => IDStyle.uppercase,
                Measuringpoint or MeasuringpointLinked => IDStyle.uppercase,
                WebcamInfo or WebcamInfoLinked => IDStyle.uppercase,
                Article or ArticlesLinked => IDStyle.uppercase,
                DDVenue => IDStyle.uppercase,
                EventShort or EventShortLinked => IDStyle.lowercase,
                ExperienceArea or ExperienceAreaLinked => IDStyle.uppercase,
                MetaRegion or MetaRegionLinked => IDStyle.uppercase,
                Region or RegionLinked => IDStyle.uppercase,
                Tourismverein or TourismvereinLinked => IDStyle.uppercase,
                Municipality or MunicipalityLinked => IDStyle.uppercase,
                District or DistrictLinked => IDStyle.uppercase,
                SkiArea or SkiAreaLinked => IDStyle.uppercase,
                SkiRegion or SkiRegionLinked => IDStyle.uppercase,
                Area or AreaLinked => IDStyle.uppercase,
                Wine or WineLinked => IDStyle.uppercase,
                SmgTags or ODHTagLinked => IDStyle.lowercase,
                _ => throw new Exception("not known odh type")
            };
        }

    }

    public enum IDStyle
    {
        uppercase,
        lowercase,
        mixed
    }
}
