using SqlKata.Execution;

namespace OdhApiImporter.Helpers.SUEDTIROLWEIN
{
    public class SuedtirolWeinImportHelper
    {
        private readonly QueryFactory QueryFactory;
        private readonly ISettings settings;

        public SuedtirolWeinImportHelper(ISettings settings, QueryFactory queryfactory)
        {
            this.QueryFactory = queryfactory;
            this.settings = settings;
        }
    }    
}
