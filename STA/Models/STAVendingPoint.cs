// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace STA
{
    public class STAVendingPoint
    {
        [Index(0)]
        [Name("Website")]
        public string? Website { get; set; }

        [Index(1)]
        [Name("STA_ID")]
        public string? STA_ID { get; set; }

        [Index(2)]
        [Name("Salepoint Name STA DE")]
        public string? Salepoint_Name_STA_DE { get; set; }

        [Index(3)]
        [Name("Salepoint Name STA IT")]
        public string? Salepoint_Name_STA_IT { get; set; }

        [Index(4)]
        [Name("Salepoint Name STA EN")]
        public string? Salepoint_Name_STA_EN { get; set; }

        [Index(5)]
        [Name("Salepoint Name STA LAD")]
        public string? Salepoint_Name_STA_LAD { get; set; }

        [Index(6)]
        [Name("Name SalePoint Gasser")]
        public string? Name_SalePoint_Gasser { get; set; }

        [Index(7)]
        [Name("Fakturierungsgruppe")]
        public string? Fakturierungsgruppe { get; set; }

        [Index(8)]
        [Name("Codice cliente")]
        public string? Codice_cliente { get; set; }

        [Index(9)]
        [Name("Verkaufspunkttypus")]
        public string? Verkaufspunkttypus { get; set; }

        [Index(10)]
        [Name("Rep. Verkaufsmandat")]
        public string? Rep_Verkaufsmandat { get; set; }

        [Index(11)]
        [Name("Rep. Privacy")]
        public string? Rep_Privacy { get; set; }

        [Index(12)]
        [Name("Rappresentate legale")]
        public string? Rappresentate_legale { get; set; }

        [Index(13)]
        [Name("Sede legale")]
        public string? Sede_legale { get; set; }

        [Index(14)]
        [Name("CAP sede legale")]
        public string? CAP_sede_legale { get; set; }

        [Index(15)]
        [Name("Stadt sede legale")]
        public string? Stadt_sede_legale { get; set; }

        [Index(16)]
        [Name("E-Mail")]
        public string? E_Mail { get; set; }

        [Index(17)]
        [Name("Pec")]
        public string? Pec { get; set; }

        [Index(18)]
        [Name("Tel.")]
        public string? Tel { get; set; }

        [Index(19)]
        [Name("SEPA")]
        public string? SEPA { get; set; }

        [Index(20)]
        [Name("IBAN")]
        public string? IBAN { get; set; }

        [Index(21)]
        [Name("Parita IVA")]
        public string? Parita_IVA { get; set; }

        [Index(22)]
        [Name("VerkaufstellenID Gasser")]
        public string? VerkaufstellenID_Gasser { get; set; }

        [Index(23)]
        [Name("Adresse DE")]
        public string? Adresse_DE { get; set; }

        [Index(24)]
        [Name("Adresse IT/EN/LAD")]
        public string? Adresse_IT_EN_LAD { get; set; }

        [Index(25)]
        [Name("CAP")]
        public string? CAP { get; set; }

        [Index(26)]
        [Name("Stadt")]
        public string? Stadt { get; set; }

        [Index(27)]
        [Name("città IT/EN/LAD")]
        public string? cittaIT_EN_LAD { get; set; }

        [Index(29)]
        [Name("CODICE ISTAT")]
        public string? CODICE_ISTAT { get; set; }

        [Index(30)]
        [Name("Wochentags Beginn")]
        public string? Wochentags_Beginn { get; set; }

        [Index(31)]
        [Name("Wochentags Ende")]
        public string? Wochentags_Ende { get; set; }

        [Index(32)]
        [Name("Pause Start")]
        public string? Pause_Start { get; set; }

        [Index(33)]
        [Name("Pause Ende")]
        public string? Pause_Ende { get; set; }

        [Index(34)]
        [Name("Samstag Beginn")]
        public string? Samstag_Beginn { get; set; }

        [Index(35)]
        [Name("Samstag Ende")]
        public string? Samstag_Ende { get; set; }

        [Index(36)]
        [Name("Pause Samstag Beginn")]
        public string? Pause_Samstag_Beginn { get; set; }

        [Index(37)]
        [Name("Pause Samstag Ende")]
        public string? Pause_Samstag_Ende { get; set; }

        [Index(38)]
        [Name("Sonntag Beginn")]
        public string? Sonntag_Beginn { get; set; }

        [Index(39)]
        [Name("Sonntag Ende")]
        public string? Sonntag_Ende { get; set; }

        [Index(40)]
        [Name("Pause Sonntag Beginn")]
        public string? Pause_Sonntag_Beginn { get; set; }

        [Index(41)]
        [Name("Pause Sonntag Ende")]
        public string? Pause_Sonntag_Ende { get; set; }

        [Index(42)]
        public string? LEER { get; set; }

        [Index(43)]
        [Name("Zusatzinfo DE")]
        public string? Zusatzinfo_DE { get; set; }

        [Index(44)]
        [Name("Zusatzinfo IT")]
        public string? Zusatzinfo_IT { get; set; }

        [Index(45)]
        [Name("Zusatzinfo LAD")]
        public string? Zusatzinfo_LAD { get; set; }

        [Index(46)]
        [Name("Zusatzinfo EN")]
        public string? Zusatzinfo_EN { get; set; }

        [Index(47)]
        [Name("lat")]
        public string? latitude { get; set; }

        [Index(48)]
        [Name("long")]
        public string? longitude { get; set; }

        [Index(49)]
        [Name("* Südtirol Pass Dienste")]
        public string? SuedtirolPassDienste { get; set; }

        [Index(50)]
        [Name("Südtirol Pass 65+ Beantragung")]
        public string? SuedtirolPass65PlusBeantragung { get; set; }

        [Index(51)]
        [Name("** Duplikat")]
        public string? Duplikat { get; set; }

        [Index(52)]
        [Name("Wertkarte")]
        public string? Wertkarte { get; set; }

        [Index(53)]
        [Name("Stadtfahrkarte o. Citybus")]
        public string? StadtfahrkarteoCitybus { get; set; }

        [Index(54)]
        [Name("Mobilcard")]
        public string? Mobilcard { get; set; }

        [Index(55)]
        [Name("bikemobil Card")]
        public string? bikemobilCard { get; set; }

        [Index(56)]
        [Name("Museumobil Card")]
        public string? MuseumobilCard { get; set; }

        [Index(57)]
        [Name("Kartenkreislauf mobilcard")]
        public string? Kartenkreislaufmobilcard { get; set; }

        [Index(58)]
        [Name("Kartenkreislauf museummobil")]
        public string? Kartenkreislaufmuseummobil { get; set; }

        [Index(59)]
        [Name("Kartenkreislauf bikemobil")]
        public string? Kartenkreislaufbikemobil { get; set; }
    }
}
