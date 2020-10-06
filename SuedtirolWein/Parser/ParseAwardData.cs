using DataModel;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SuedtirolWein.Parser
{
    public class ParseAwardData
    {
        //<item>
        //<id>ddaf48f2-ab83-48bf-b354-14734c893d37</id>
        //<title>
        //<![CDATA[ Südtirol Terlaner Chardonnay Rarität ]]>
        //</title>
        //<vintage>2003</vintage>
        //<awardyear>2016</awardyear>
        //<company>Kellerei Terlan</company>
        //<companyid>18635009-4610-4c1b-b403-54480c77ece9</companyid>
        //<wine>Chardonnay</wine>
        //<wineid>2090c3d2-42d1-45cc-bd53-7ecbb1e027e1</wineid>
        //<awards>Veronelli,Slow Wine,Bibenda,AIS</awards>
        //<media>
        //http://www.suedtirolwein.com/media/045d51a5-edf7-4f55-8328-43eebae2fc3a/kellerei-terlan-terlaner-weissburgunder-raritaet-2002-kopie.png
        //</media>
        //</item>

        public static Wine ParsetheAwardData(Wine mywine, XElement myawardde, XElement myawardit, XElement myawarden, List<string> haslanguage)
        {
            mywine.LastChange = DateTime.Now;

            mywine.Id = myawardde.Element("id").Value;

            string titlede = myawardde.Element("title").Value;
            string winenamede = myawardde.Element("wine").Value;

            Detail mydetail = new Detail();
            mydetail.Title = titlede;
            mydetail.Language = "de";
            mydetail.Header = winenamede;

            mywine.Detail.TryAddOrUpdate("de", mydetail);

            if (haslanguage.Contains("it"))
            {
                string titleit = myawardit.Element("title").Value;
                string winenameit = myawardit.Element("wine").Value;

                Detail mydetailit = new Detail();
                mydetailit.Title = titleit;
                mydetailit.Language = "it";
                mydetailit.Header = winenameit;

                mywine.Detail.TryAddOrUpdate("it", mydetailit);
            }

            if (haslanguage.Contains("en"))
            {
                string titleen = myawarden.Element("title").Value;
                string winenameen = myawarden.Element("wine").Value;

                Detail mydetailen = new Detail();
                mydetailen.Title = titleen;
                mydetailen.Language = "en";
                mydetailen.Header = winenameen;

                mywine.Detail.TryAddOrUpdate("en", mydetailen);
            }

            mywine.Vintage = !String.IsNullOrEmpty(myawardde.Element("vintage").Value) ? Convert.ToInt32(myawardde.Element("vintage").Value) : 0;
            mywine.Awardyear = !String.IsNullOrEmpty(myawardde.Element("awardyear").Value) ? Convert.ToInt32(myawardde.Element("awardyear").Value) : 0;

            mywine.CompanyId = myawardde.Element("companyid").Value;
            mywine.Awards = myawardde.Element("awards").Value.Split(',').ToList();

            mywine.CustomId = myawardde.Element("wineid").Value;



            if (!String.IsNullOrEmpty(myawardde.Element("media").Value))
            {

                List<ImageGallery> myimglist = new List<ImageGallery>();

                if (mywine.ImageGallery != null)
                {
                    if (mywine.ImageGallery.Count > 0)
                    {
                        myimglist.AddRange(mywine.ImageGallery.Where(x => x.ImageSource == "SMG"));
                    }
                }

                ImageGallery myimage = new ImageGallery();
                myimage.ImageName = myawardde.Element("title").Value;
                myimage.IsInGallery = true;
                myimage.ListPosition = 0;
                myimage.ImageSource = "SuedtirolWein";
                myimage.ImageUrl = myawardde.Element("media").Value;

                if (myimage.ImageUrl.StartsWith("https://www.suedtirolwein.secure.consisto.net/"))
                {
                    myimage.ImageUrl.Replace("https://www.suedtirolwein.secure.consisto.net/", "https://intranet.suedtirolwein.com/");
                }

                myimage.Height = 0;
                myimage.Width = 0;

                myimglist.Add(myimage);

                mywine.ImageGallery = myimglist.ToList();

                mywine.HasLanguage = haslanguage;
            }

            return mywine;
        }

    }
}
