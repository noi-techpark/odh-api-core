using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Helper.MSS
{
    public class GetMssRoomlist
    {
        public static async Task<List<AccoRoom>> GetMssRoomlistAsync(string lang, string hotelid, string hotelidofchannel, XElement roomdetails, XDocument roomamenities, string source, string version, string mssuser, string msspswd)
        {
            try
            {


                XDocument myrequest = MssRequest.BuildRoomlistPostData(roomdetails, hotelid, hotelidofchannel, lang, source, version, mssuser, msspswd);
                var myresponses = MssRequest.RequestRoomAsync(myrequest);

                await Task.WhenAll(myresponses);

                Task<string> roomresponsecontent = myresponses.Result.Content.ReadAsStringAsync();

                await Task.WhenAll(roomresponsecontent);

                XElement fullresponse = XElement.Parse(roomresponsecontent.Result);

                //do muassmen iatz nuie method schreiben
                var myparsedresponse = ParseMssRoomResponse.ParseMyRoomResponse(lang, fullresponse, roomamenities);

                return myparsedresponse;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<AccoRoom> GetMssRoomlistSync(string lang, string hotelid, string hotelidofchannel, XElement roomdetails, XDocument roomamenities, string source, string version, string mssuser, string msspswd)
        {
            try
            {


                XDocument myrequest = MssRequest.BuildRoomlistPostData(roomdetails, hotelid, hotelidofchannel, lang, source, version, mssuser, msspswd);
                var myresponses = MssRequest.RequestRoom(myrequest);

                string roomresponsecontent = myresponses.Content.ReadAsStringAsync().Result;

                XElement fullresponse = XElement.Parse(roomresponsecontent);

                //do muassmen iatz nuie method schreiben
                var myparsedresponse = ParseMssRoomResponse.ParseMyRoomResponse(lang, fullresponse, roomamenities);

                return myparsedresponse;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
