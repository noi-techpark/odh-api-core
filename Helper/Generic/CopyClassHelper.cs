using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class CopyClassHelper
    {
        /// <summary>
        /// Methode die mir geändertes Objekt einer Aktivität POI kopiert für Aktivitäten Pois wo sich der HautpTyp geändert hat
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyPropertyValues(object source, object destination)
        {
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (
                        destProperty.Name == sourceProperty.Name
                        && destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)
                    )
                    {
                        destProperty.SetValue(
                            destination,
                            sourceProperty.GetValue(source, new object[] { }),
                            new object[] { }
                        );

                        break;
                    }
                }
            }
        }
    }
}
