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
            return odhtype switch
            {
                Accommodation or AccommodationLinked => CreateGUID(true),
                AccoRoom or AccommodationRoomLinked => CreateGUID(true),
                GBLTSActivity or LTSActivityLinked => CreateGUID(true),
                GBLTSPoi or LTSPoiLinked => CreateGUID(true),
                Gastronomy or GastronomyLinked => CreateGUID(true),
                Event or EventLinked => CreateGUID(true),
                ODHActivityPoi or ODHActivityPoiLinked => CreateGUID(false),
                Package or PackageLinked => CreateGUID(true),
                Measuringpoint or MeasuringpointLinked => CreateGUID(true),
                WebcamInfo or WebcamInfoLinked => CreateGUID(true),
                Article or ArticlesLinked => CreateGUID(true),
                DDVenue => CreateGUID(true),
                EventShort or EventShortLinked => CreateGUID(true),
                ExperienceArea or ExperienceAreaLinked => CreateGUID(true),
                MetaRegion or MetaRegionLinked => CreateGUID(true),
                Region or RegionLinked => CreateGUID(true),
                Tourismverein or TourismvereinLinked => CreateGUID(true),
                Municipality or MunicipalityLinked => CreateGUID(true),
                District or DistrictLinked => CreateGUID(true),
                SkiArea or SkiAreaLinked => CreateGUID(true),
                SkiRegion or SkiRegionLinked => CreateGUID(true),
                Area or AreaLinked => CreateGUID(true),
                Wine or WineLinked => CreateGUID(true),
                SmgTags or ODHTagLinked => CreateGUID(false),
                _ => throw new Exception("not known odh type")
            };
        }

        private static string CreateGUID(bool uppercase)
        {
            var id = System.Guid.NewGuid().ToString();

            if (uppercase)
                id = id.ToUpper();
            else
                id = id.ToLower();

            return id;
        }
    }
}
