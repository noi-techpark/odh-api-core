using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    /// <summary>
    /// OLD Classes for Mobile Api not used at the moment
    /// </summary>

    #region Mobile
    
    public class SmgPoisMobileTypes
    {
        public SmgPoisMobileTypes()
        {
            TypeDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        //public int Bitmask { get; set; }
        public string? Key { get; set; }
        public string? Type { get; set; }
        public string? IconURL { get; set; }
        public int SortOrder { get; set; }
        public bool active { get; set; }

        public Dictionary<string, string>? TypeDesc { get; set; }
    }

    public class SmgPoisMobileTypesExtended : SmgPoisMobileTypes
    {
        public ICollection<SmgPoisMobileFilters>? SubTypes { get; set; }
    }

    public class SmgPoisMobileFilters
    {
        public SmgPoisMobileFilters()
        {
            FilterText = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public string? MainTypeId { get; set; }
        public int SortOrder { get; set; }

        public int Bitmask { get; set; }
        public IDictionary<string, string> FilterText { get; set; }
        public string? FilterReference { get; set; }

        public ICollection<SmgPoisMobileFilterDetail>? FilterDetails { get; set; }
    }

    public class SmgPoisMobileFilterDetail
    {
        public SmgPoisMobileFilterDetail()
        {
            FilterText = new Dictionary<string, string>();
            StartingDesc = new Dictionary<string, string>();
            EndDesc = new Dictionary<string, string>();
        }

        public string? Id { get; set; }                                  //Unique Id
        //public string MainTypeId { get; set; }                          //Reference to Maintype
        public int SortOrder { get; set; }                              //Sort Order of the Filter
        public int Bitmask { get; set; }

        public string? Filtertype { get; set; }                          //Type of the Filter (checkbox, scroller, rating)
        public IDictionary<string, string> FilterText { get; set; }    //Values of the Filter, Key is intended to use on the Filter Api, Value is what we want to display.  

        public string? FilterReference { get; set; }
        public string? FilterString { get; set; }

        public IDictionary<string, string> StartingDesc { get; set; }    //For a scroller Filter, there can be provided a Starting Description (like 1 km)
        public string? StartingValue { get; set; }                       //For a scroller Filter a startingvalue can be defined
        public IDictionary<string, string> EndDesc { get; set; }         //For a scroller Filter, there can be provided a Ending Description (like >20 km)
        public string? EndValue { get; set; }                            //For a scroller Filter a endingvalue can be defined

        public int RatingItems { get; set; }                            //For a rating Filter there can be set a Rating Items (this well be 6)
        public string? SelectedValue { get; set; }                       //For a rating Filter the Initially selected Value can be defined

    }

    public class MobileHtml
    {
        public MobileHtml()
        {
            HtmlText = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public Dictionary<string, string> HtmlText { get; set; }
    }

    public class Tutorial
    {
        public Tutorial()
        {
            image_url = new Dictionary<string, string>();
            title = new Dictionary<string, string>();
            description = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public Dictionary<string, string> image_url { get; set; }
        public Dictionary<string, string> title { get; set; }
        public Dictionary<string, string> description { get; set; }

        public string? category { get; set; }
        public int sortorder { get; set; }

    }

    public class AppMessage
    {
        public AppMessage()
        {
            Text = new Dictionary<string, string>();
            Title = new Dictionary<string, string>();
            VideoUrl = new Dictionary<string, string>();
            Images = new Dictionary<string, List<AppMessageImage>>();
        }


        public string? Id { get; set; }
        public string? Type { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public Dictionary<string, string> Title { get; set; }
        public Dictionary<string, string> Text { get; set; }

        public Dictionary<string, List<AppMessageImage>> Images { get; set; }
        public Dictionary<string, string> VideoUrl { get; set; }
    }

    public class AppMessageImage
    {
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; }
    }

    //public class FiltersByMainType
    //{
    //    public Dictionary<string, >

    //}

    //public class SmgPoisMobileFilterLocalized
    //{
    //    public string Id { get; set; }                                  //Unique ID
    //    public string MainTypeId { get; set; }                          //Reference to Maintype

    //    public string Filtername { get; set; }                          //Filtername to display
    //    public string Filterkey { get; set; }                           //Effective Value you use on the Filter Api
    //    public string FilterReference { get; set; }

    //    public string language { get; set; }                            //Current Language
    //    public int SortOrder { get; set; }                              //Sort Order

    //    public ICollection<SmgPoisMobileFilterDetailLocalized> FilterDetails { get; set; }     //List with Detailed Filters
    //}

    //public class SmgPoisMobileFilterDetailLocalized
    //{       
    //    public string Id { get; set; }                                  //Unique Id
    //    public string SubTypeId { get; set; }                           //Reference to SmgPoisMobileFilterListLocalized ID

    //    public string Filtername { get; set; }                          //Filtername to display
    //    public string Filterkey { get; set; }                           //Effective Value you use on the Filter Api
    //    public string FilterReference { get; set; }

    //    public string Filtertype { get; set; }                          //Type of the Filter (checkbox, scroller, rating)

    //    public string language { get; set; }                            //Current Language
    //    public int SortOrder { get; set; }                              //Sort Order of the Filter

    //    public string StartingDesc { get; set; }                        //For a scroller Filter, there can be provided a Starting Description (like 1 km)
    //    public string StartingValue { get; set; }                       //For a scroller Filter a startingvalue can be defined
    //    public string EndDesc { get; set; }                             //For a scroller Filter, there can be provided a Ending Description (like >20 km)
    //    public string EndValue { get; set; }                            //For a scroller Filter a endingvalue can be defined

    //    public int RatingItems { get; set; }                            //For a rating Filter there can be set a Rating Items (this well be 6)
    //    public string SelectedValue { get; set; }                       //For a rating Filter the Initially selected Value can be defined
    //}

    public class AccoThemesMobile
    {
        public AccoThemesMobile()
        {
            Name = new Dictionary<string, string>();
        }

        public string? Id { get; set; }
        public int Bitmask { get; set; }
        public string? Type { get; set; }
        public string? Key { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public string? ImageURL { get; set; }
        //public int AccoCount { get; set; }
        public int SortOrder { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> AccoCount { get; set; }
    }

    public class AccoThemesFull
    {
        public string? Id { get; set; }
        public int Bitmask { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? ImageURL { get; set; }
        public int AccoCount { get; set; }
        public int SortOrder { get; set; }
    }

    public class AppCustomTips
    {
        public AppCustomTips()
        {
            Title = new Dictionary<string, string>();
            Description = new Dictionary<string, string>();
            Region = new Dictionary<string, string>();
            Tv = new Dictionary<string, string>();
            LinkText = new Dictionary<string, string>();
            Category = new Dictionary<string, string>();
            ValidForLanguage = new Dictionary<string, bool>();
        }

        public string? Id { get; set; }
        public string? ImageUrl { get; set; }
        public IDictionary<string, string> Title { get; set; }
        public IDictionary<string, string> Description { get; set; }
        public IDictionary<string, string> Region { get; set; }
        public IDictionary<string, string> Tv { get; set; }
        public IDictionary<string, string> LinkText { get; set; }
        public string? Link { get; set; }
        public bool Active { get; set; }
        public DateTime LastChanged { get; set; }
        public string? Type { get; set; }

        public string? TvId { get; set; }

        //additional
        public IDictionary<string, string> Category { get; set; }
        public string? Difficulty { get; set; }
        public string? Duration { get; set; }
        public string? Length { get; set; }

        //Settings
        public ICollection<AppCustomTipsSettings>? AppCustomTipsSettings { get; set; }

        public IDictionary<string, bool> ValidForLanguage { get; set; }
    }

    public class AppCustomTipsSettings
    {
        public int Fixedposition { get; set; }  //position 0 is random
        public bool Randomposition { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }

    public class AppSuggestion
    {
        public AppSuggestion()
        {
            Suggestion = new Dictionary<string, Suggestion>();
        }

        public string? Id { get; set; }
        public string? Platform { get; set; }

        public List<AppSuggestionValidFor>? Validfor { get; set; }

        public IDictionary<string, Suggestion> Suggestion { get; set; }

    }

    public class AppSuggestionValidFor
    {
        public string? MainEntity { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
    }

    public class Suggestion
    {
        public string? Title { get; set; }
        public string? Icon { get; set; }
        public string? Package { get; set; }
        public string? Developer { get; set; }
        public string? Description { get; set; }
    }

    #endregion


}
