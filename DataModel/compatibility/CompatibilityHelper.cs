using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.compatibility
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


    }
}
