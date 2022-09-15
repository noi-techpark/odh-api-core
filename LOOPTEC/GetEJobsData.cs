namespace LOOPTEC
{
    public class GetEJobsData
    {
        public const string serviceurlejobs = @"https://app.onboard-staging.org/exports/v1/jobs/open_data_hub.json";

        public static async Task<string> GetEjobsDataFromService(string user, string pass)
        {
            using (var client = new HttpClient())
            {
                var myresponse = await client.GetAsync(serviceurlejobs);

                var myresponsestring = await myresponse.Content.ReadAsStringAsync();

                return myresponsestring;
            }            
        }        
    }
}