// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    //This class contains static helpers to map some outdated deprecated keys to the new keys
    public static class CompatibilityHelper
    {

        //Compatibility Hack EventShort CustomTagging, Technologyfields
        public static string GetOldEventShortTagnames(string taglowercase) => taglowercase switch
        {
            "artsculture" => "Arts&Culture",
            "camp4company" => "Camp4Company",
            "collaboraticerobotics" => "Collaboratice Robotics",
            "companies" => "Companies",
            "elettrotinkering" => "Elettro-Tinkering",
            "euopportunities" => "EU Opportunities",
            "innovationmanagement" => "Innovation Management",
            "labs" => "Labs",
            "legorobotics" => "Lego Robotics",
            "mininoi" => "MiniNOI",
            "noicommunity" => "NOI Community",
            "opendatahub" => "Open Data Hub",
            "openday" => "Open Day",
            "outofthelab" => "Out of the Lab",
            "public" => "Public",
            "publicengagement" => "Public Engagement",
            "researchers" => "Researchers",
            "scratchvideogames" => "Scratch Videogames",
            "sport" => "Sport",
            "square" => "Square",
            "startupincubator " => "Start-up Incubator",
            "startups" => "Startups",
            "summeratnoi" => "Summer at NOI",
            "techtransfer" => "Tech Transfer",            
            "automotiveautomation" => "Automotive/Automation",
            "digital" => "Digital",
            "food" => "Food",
            "green" => "Green",
            "alpine" => "Alpine",            
            _ => taglowercase
        };


        public static string ConvertArticleTypesToTags(string type) => type switch
        {
            "contentartikel" => "contentarticle",
            "rezeptartikel" => "recipearticle",
            "buchtippartikel" => "bookarticle",
            "basisartikel" => "basearticle",
            "reiseveranstalter" => "touroperatorarticle",
            "katalog" => "catalog",
            "idmartikel" => "idmarticle",
            "veranstaltungsartikel" => "eventarticle",
            "presseartikel" => "pressarticle",
            "b2bartikel" => "b2barticle",            
            "newsfeednoi" => "newsfeednoi",
            "specialannouncement" => "specialannouncement",
            _ => type
        };

        public static string ConvertArticleSubTypesToTags(string subtype) => subtype switch
        {
            "allgemeinepresseartikel" => "",
            "Site Content" => "",
            "Trekking-guides" => "",
            "News" => "news",
            "Kultur" => "culture",
            "B2BVeranstaltung" => "",
            "Ausstellungen/Kunst" => "",
            "Gastronomy" => "",
            "Kinder/Familie" => "",
            "Tip" => "",
            "Volksfest" => "",
            "B2BNeuigkeiten" => "",
            "Suggestion" => "",
            "Gastronomie" => "",
            "Geführte Touren" => "",
            "Tradition" => "",
            "Hotel" => "",
            "genussbotschafter" => "",
            "B2BProgrammTipp" => "",
            "General" => "",
            "Detail" => "",
            "pressemitteilungen" => "",
            "Kunsthandwerk" => "",
            "Treffen" => "",
            "Musik" => "music",
            "pressethemenserviceartikel" => "pressthemeservice",
            "Sport" => "sport",
            "pressemeetings" => "pressmeeting",
            "Culture-History" => "",
            "hersteller" => "",
            "B2BDetail" => "",
            "Unterhaltung" => "",
            _ => subtype
        };
    }
}
