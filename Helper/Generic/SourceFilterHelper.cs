using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Generic
{
    public static class SourceFilterHelper
    {
        public static List<string> ExtendSourceFilterODHActivityPois(List<string> sourcelist)
        {
            List<string> sourcelistnew = new();

            foreach (var source in sourcelist)
            {
                sourcelistnew.Add(source);

                if (source == "idm")
                {
                    if (!sourcelistnew.Contains("none"))
                        sourcelistnew.Add("none");
                    if (!sourcelistnew.Contains("magnolia"))
                        sourcelistnew.Add("magnolia");
                    if (!sourcelistnew.Contains("common"))
                        sourcelistnew.Add("common");

                }
                else if (source == "lts")
                {
                    if (!sourcelistnew.Contains("activitydata"))
                        sourcelistnew.Add("activitydata");
                    if (!sourcelistnew.Contains("poidata"))
                        sourcelistnew.Add("poidata");
                    if (!sourcelistnew.Contains("beacondata"))
                        sourcelistnew.Add("beacondata");
                    if (!sourcelistnew.Contains("gastronomicdata"))
                        sourcelistnew.Add("gastronomicdata");
                    if (!sourcelistnew.Contains("beacondata"))
                        sourcelistnew.Add("beacondata");
                }
                else if (source == "siag")
                {
                    if (!sourcelistnew.Contains("museumdata"))
                        sourcelistnew.Add("museumdata");
                }
                else if (source == "dss")
                {
                    if (!sourcelistnew.Contains("dssliftbase"))
                        sourcelistnew.Add("dssliftbase");
                    if (!sourcelistnew.Contains("dssslopebase"))
                        sourcelistnew.Add("dssslopebase");
                }
                else if (source == "content")
                {
                    if (!sourcelistnew.Contains("none"))
                        sourcelistnew.Add("none");
                }
            }

            return sourcelistnew;
        }
    }
}
