using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class HasLanguageHelper
    {        
        private static void FixDetailBaseAndIntroText(Detail mydetail)
        {
            if (!String.IsNullOrEmpty(mydetail.BaseText))
            {
                mydetail.BaseText = mydetail.BaseText.Trim();

                if (mydetail.BaseText == "&#10;&#10;" || mydetail.BaseText == "\n\n")
                    mydetail.BaseText = "";
            }
            if (!String.IsNullOrEmpty(mydetail.IntroText))
            {
                mydetail.IntroText = mydetail.IntroText.Trim();

                if (mydetail.IntroText == "&#10;&#10;" || mydetail.IntroText == "\n\n")
                    mydetail.IntroText = "";
            }
        }

        //For Articles
        public static void CheckMyInsertedLanguages(this ArticleBaseInfos myarticle, List<string> availablelanguages)
        {
            if (myarticle.HasLanguage == null)
                myarticle.HasLanguage = new List<string>();

            //Detail, ImageGallery, ContactInfos, AdditionalArticleInfos, 
            foreach (string language in availablelanguages)
            {
                if (myarticle.Detail.ContainsKey(language) || myarticle.ContactInfos.ContainsKey(language) || myarticle.AdditionalArticleInfos.ContainsKey(language))
                {
                    bool removelang = true;
                    
                    if (myarticle.Detail.ContainsKey(language) && myarticle.Detail[language] != null)
                    {
                        var detailvalues = myarticle.Detail[language];

                        FixDetailBaseAndIntroText(detailvalues);

                        if (!String.IsNullOrEmpty(detailvalues.AdditionalText))
                            if (!String.IsNullOrEmpty(detailvalues.AdditionalText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.BaseText))
                            if (!String.IsNullOrEmpty(detailvalues.BaseText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.GetThereText))
                            if (!String.IsNullOrEmpty(detailvalues.GetThereText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Header))
                            if (!String.IsNullOrEmpty(detailvalues.Header.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.IntroText))
                            if (!String.IsNullOrEmpty(detailvalues.IntroText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.SubHeader))
                            if (!String.IsNullOrEmpty(detailvalues.SubHeader.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Title))
                            if (!String.IsNullOrEmpty(detailvalues.Title.Trim()))
                                removelang = false;
                    }
                    if (myarticle.ContactInfos.ContainsKey(language) && myarticle.ContactInfos[language] != null)
                    {
                        var contactvalues = myarticle.ContactInfos[language];

                        if (!String.IsNullOrEmpty(contactvalues.Address))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.City))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CompanyName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryCode))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Email))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Faxnumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Givenname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.LogoUrl))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.NamePrefix))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Phonenumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Surname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Tax))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Url))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Vat))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.ZipCode))
                            removelang = false;
                    }

                    if (myarticle.AdditionalArticleInfos.ContainsKey(language))
                    {
                        var additionalvalues = myarticle.AdditionalArticleInfos[language];

                        if (additionalvalues.Elements.Count > 0)
                            removelang = false;

                    }

                    //Add Language
                    if (removelang == false)
                    {
                        if (!myarticle.HasLanguage.Contains(language))
                            myarticle.HasLanguage.Add(language);
                    }
                    //Remove Language
                    else if (removelang == true)
                    {
                        if (myarticle.Detail.ContainsKey(language))
                            myarticle.Detail.Remove(language);

                        if (myarticle.ContactInfos.ContainsKey(language))
                            myarticle.ContactInfos.Remove(language);

                        if (myarticle.HasLanguage.Contains(language))
                            myarticle.HasLanguage.Remove(language);
                    }
                }
            }            
        }

        //For Activities, Pois, ODHActivityPois
        public static void CheckMyInsertedLanguages(this PoiBaseInfos mypoiactivity, List<string> availablelanguages)
        {
            if (mypoiactivity.HasLanguage == null)
                mypoiactivity.HasLanguage = new List<string>();

            //Detail, ImageGallery, ContactInfos, AdditionalArticleInfos, 
            foreach (string language in availablelanguages)
            {
                if (mypoiactivity.Detail.ContainsKey(language) || mypoiactivity.ContactInfos.ContainsKey(language)) // || mypoiactivity.AdditionalPoiInfos.ContainsKey(language))
                {
                    bool removelang = true;

                   if (mypoiactivity.Detail.ContainsKey(language) && mypoiactivity.Detail[language] != null)
                    {
                        var detailvalues = mypoiactivity.Detail[language];

                        if (!String.IsNullOrEmpty(detailvalues.AdditionalText))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.BaseText))
                            if (!String.IsNullOrEmpty(detailvalues.BaseText.Trim()))
                                if (detailvalues.BaseText.Trim() != "&#10;&#10;")
                                    removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.GetThereText))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Header))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.IntroText))
                            if (!String.IsNullOrEmpty(detailvalues.IntroText.Trim()))
                                if (detailvalues.IntroText.Trim() != "&#10;&#10;")
                                    removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.SubHeader))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Title))
                            removelang = false;
                    }
                    if (mypoiactivity.ContactInfos.ContainsKey(language) && mypoiactivity.ContactInfos[language] != null)
                    {
                        var contactvalues = mypoiactivity.ContactInfos[language];

                        if (!String.IsNullOrEmpty(contactvalues.Address))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.City))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CompanyName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryCode))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Email))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Faxnumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Givenname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.LogoUrl))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.NamePrefix))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Phonenumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Surname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Tax))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Url))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Vat))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.ZipCode))
                            removelang = false;
                    }

                    //Add Language
                    if (removelang == false)
                    {
                        if (!mypoiactivity.HasLanguage.Contains(language))
                            mypoiactivity.HasLanguage.Add(language);
                    }
                    //Remove Language
                    else if (removelang == true)
                    {
                        if (mypoiactivity.Detail.ContainsKey(language))
                            mypoiactivity.Detail.Remove(language);

                        if (mypoiactivity.ContactInfos.ContainsKey(language))
                            mypoiactivity.ContactInfos.Remove(language);

                        if (mypoiactivity.HasLanguage.Contains(language))
                            mypoiactivity.HasLanguage.Remove(language);
                    }
                }
            }            
        }

        //For Events
        public static void CheckMyInsertedLanguages(this EventBaseInfos mypoiactivity, List<string> availablelanguages)
        {
            if (mypoiactivity.HasLanguage == null)
                mypoiactivity.HasLanguage = new List<string>();

            //Detail, ImageGallery, ContactInfos, AdditionalArticleInfos, 
            foreach (string language in availablelanguages)
            {
                if (mypoiactivity.Detail.ContainsKey(language) || mypoiactivity.ContactInfos.ContainsKey(language)) // || mypoiactivity.AdditionalPoiInfos.ContainsKey(language))
                {
                    bool removelang = true;
                    
                    if (mypoiactivity.Detail.ContainsKey(language) && mypoiactivity.Detail[language] != null)
                    {
                        var detailvalues = mypoiactivity.Detail[language];

                        if (!String.IsNullOrEmpty(detailvalues.AdditionalText))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.BaseText))
                            if (!String.IsNullOrEmpty(detailvalues.BaseText.Trim()))
                                if (detailvalues.BaseText.Trim() != "&#10;&#10;")
                                    removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.GetThereText))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Header))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.IntroText))
                            if (!String.IsNullOrEmpty(detailvalues.IntroText.Trim()))
                                if (detailvalues.IntroText.Trim() != "&#10;&#10;")
                                    removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.SubHeader))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Title))
                            removelang = false;
                    }
                    if (mypoiactivity.ContactInfos.ContainsKey(language) && mypoiactivity.ContactInfos[language] != null)
                    {
                        var contactvalues = mypoiactivity.ContactInfos[language];

                        if (!String.IsNullOrEmpty(contactvalues.Address))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.City))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CompanyName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryCode))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Email))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Faxnumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Givenname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.LogoUrl))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.NamePrefix))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Phonenumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Surname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Tax))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Url))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Vat))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.ZipCode))
                            removelang = false;
                    }
                    //Add Language
                    if (removelang == false)
                    {
                        if (!mypoiactivity.HasLanguage.Contains(language))
                            mypoiactivity.HasLanguage.Add(language);
                    }
                    //Remove Language
                    else if (removelang == true)
                    {
                        if (mypoiactivity.Detail.ContainsKey(language))
                            mypoiactivity.Detail.Remove(language);

                        if (mypoiactivity.ContactInfos.ContainsKey(language))
                            mypoiactivity.ContactInfos.Remove(language);

                        if (mypoiactivity.HasLanguage.Contains(language))
                            mypoiactivity.HasLanguage.Remove(language);
                    }
                }
            }            
        }

        //For EventShort
        public static void CheckMyInsertedLanguages(this EventShort mydata, List<string>? availablelanguages = null)
        {
            if (availablelanguages == null)
                availablelanguages = new List<string> { "de", "it", "en" };

            if (mydata.HasLanguage == null)
                mydata.HasLanguage = new List<string>();

            foreach (string language in availablelanguages)
            {
                bool removelang = true;

                //Check if there is a Name or desc in the language
                if (language == "de")
                {
                    if (!String.IsNullOrEmpty(mydata.EventDescriptionDE))
                        removelang = false;
                    if (!String.IsNullOrEmpty(mydata.EventTextDE))
                        removelang = false;
                }
                else if (language == "it")
                {
                    if (!String.IsNullOrEmpty(mydata.EventDescriptionIT))
                        removelang = false;
                    if (!String.IsNullOrEmpty(mydata.EventTextIT))
                        removelang = false;
                }
                else if (language == "en")
                {
                    if (!String.IsNullOrEmpty(mydata.EventDescriptionEN))
                        removelang = false;
                    if (!String.IsNullOrEmpty(mydata.EventTextEN))
                        removelang = false;
                }

                //Add Language
                if (removelang == false)
                {
                    if (!mydata.HasLanguage.Contains(language))
                        mydata.HasLanguage.Add(language);
                }
                //Remove Language
                else if (removelang == true)
                {
                    if (mydata.HasLanguage.Contains(language))
                        mydata.HasLanguage.Remove(language);
                }
            }
        }

        //TODO: Accommodations, etc...
    }
}
