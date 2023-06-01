// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class GastronomyListCreator
    {
        public static List<string> CreateGastroCeremonyCodeList(string capacityceremonyfilter)
        {
            List<string> typeids = new List<string>();

            if (!String.IsNullOrEmpty(capacityceremonyfilter))
            {
                if (capacityceremonyfilter != "null")
                {
                    if (capacityceremonyfilter.Substring(capacityceremonyfilter.Length - 1, 1) == ",")
                        capacityceremonyfilter = capacityceremonyfilter.Substring(0, capacityceremonyfilter.Length - 1);

                    var splittedfilter = capacityceremonyfilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {

                        switch (filter)
                        {
                            case "1":
                                typeids.Add("DEC7019ADE6B46CDAE87584821D9B4DB");
                                break;
                            case "2":
                                typeids.Add("648773AE1BBD4001B85DC88E7592ACE2");
                                break;
                            case "3":
                                typeids.Add("085CF94B4F25440AA079E88D8DBA45C2");
                                break;
                            case "4":
                                typeids.Add("0A7DD92FA86B47D18DBFCB5572A93C9F");
                                break;
                            case "5":
                                typeids.Add("22C75C83F99F4D6FADF6D82F7754B4C1");
                                break;
                            case "6":
                                typeids.Add("4FBC28A456AA43E6B01D8BD1072D8CE6");
                                break;
                            case "7":
                                typeids.Add("94E42C7211B9430B8F096ABB7ED59AC2");
                                break;
                            case "8":
                                typeids.Add("38DCFB491D27408C990654CB64C6339D");
                                break;
                        }
                    }
                }
            }

            return typeids;
        }

        public static List<string> CreateGastroDishCodeList(string dishcodefilter)
        {
            List<string> typeids = new List<string>();

            if (!String.IsNullOrEmpty(dishcodefilter))
            {
                if (dishcodefilter != "null")
                {
                    if (dishcodefilter.Substring(dishcodefilter.Length - 1, 1) == ",")
                        dishcodefilter = dishcodefilter.Substring(0, dishcodefilter.Length - 1);

                    var splittedfilter = dishcodefilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {

                        switch (filter)
                        {
                            case "1":
                                typeids.Add("A130EB1985EC41CFB199528BE038399B");
                                break;
                            case "2":
                                typeids.Add("B539399E53D348049B9E710A2B22E74D");
                                break;
                            case "3":
                                typeids.Add("A7601BBA081B4D48A50634E029B3D75A");
                                break;
                            case "4":
                                typeids.Add("E7B9475EC5B24B6F830FBD0339D48F9D");
                                break;
                            case "5":
                                typeids.Add("EB7532946781423D9932121F3D1D7CC4");
                                break;
                            case "6":
                                typeids.Add("78A13E3381FA4A71B21118BDDF84BAFB");
                                break;
                            case "7":
                                typeids.Add("6284265E90D24C909E23A176EEB3B6F7");
                                break;
                            case "8":
                                typeids.Add("AD8426538FCF4D8A81E06BE044088BAA");
                                break;
                            case "9":
                                typeids.Add("5C84265DA5F84F84A7896808ACCB675A");
                                break;
                        }
                    }
                }
            }
            return typeids;
        }

        public static List<string> CreateGastroCategoryCodeList(string categorycodefilter)
        {
            List<string> typeids = new List<string>();

            if (!String.IsNullOrEmpty(categorycodefilter))
            {
                if (categorycodefilter != "null")
                {
                    if (categorycodefilter.Substring(categorycodefilter.Length - 1, 1) == ",")
                        categorycodefilter = categorycodefilter.Substring(0, categorycodefilter.Length - 1);

                    var splittedfilter = categorycodefilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {

                        switch (filter)
                        {
                            case "1":
                                typeids.Add("B0BDC4C2C5938D9B734D97B09C8A47A4");
                                break;
                            case "2":
                                typeids.Add("9095FC003A3E2F393D63A54682359B37");
                                break;
                            case "3":
                                typeids.Add("59FE0B38EB7F4AC3951A5F477A0E1FA2");
                                break;
                            case "4":
                                typeids.Add("43D095A3FE8A450099D33926BBC1ADF8");
                                break;
                            case "5":
                                typeids.Add("8176B5A707E2067708AF18045E068E15");
                                break;
                            case "6":
                                typeids.Add("AC56B3717C3152A428A1D338A638C570");
                                break;
                            case "7":
                                typeids.Add("E8883A596A2463A9B3E1586C9E780F17");
                                break;
                            case "8":
                                typeids.Add("700B02F1BE96B01C34CCF7A637DB3054");
                                break;
                            case "9":
                                typeids.Add("4A14E16888CB07C18C65A6B59C5A19A7");
                                break;
                            case "10":
                                typeids.Add("AB320B063588EA95F45505E940903115");
                                break;
                            case "11":
                                typeids.Add("33B86F5B91A08A0EFD6854DEB0207205");
                                break;
                            case "12":
                                typeids.Add("29BC7A9AE7CF173FBCCE6A48DD001229");
                                break;
                            case "13":
                                typeids.Add("C3CC9C83C32BFA4E9A05133291EA9FFB");
                                break;
                            case "14":
                                typeids.Add("6A2A32E2BFEE270083351B0CFD9BA2E3");
                                break;
                            case "15":
                                typeids.Add("9B158D17F03509C46037C3C7B23F2FE4");
                                break;
                            case "16":
                                typeids.Add("D8B8ABEDD17A139DEDA2695545C420D6");
                                break;
                            case "17":
                                typeids.Add("902D9BA559B1ED889694284F05CFA41E");
                                break;
                            case "18":
                                typeids.Add("2328C37167BBBC5776831B8A262A6C36");
                                break;
                            case "19":
                                typeids.Add("8025DB5CFCBA4FF281DDDE1F2B1D19A2");
                                break;
                            case "20":
                                typeids.Add("B916489A77C94D8D92B03184EE587A31");
                                break;
                        }
                    }
                }
            }

            return typeids;
        }

        public static List<string> CreateGastroFacilityCodeList(string facilitycodefilter)
        {
            List<string> typeids = new List<string>();

            if (!String.IsNullOrEmpty(facilitycodefilter))
            {
                if (facilitycodefilter != "null")
                {
                    if (facilitycodefilter.Substring(facilitycodefilter.Length - 1, 1) == ",")
                        facilitycodefilter = facilitycodefilter.Substring(0, facilitycodefilter.Length - 1);

                    var splittedfilter = facilitycodefilter.Split(',');

                    foreach (var filter in splittedfilter)
                    {

                        switch (filter)
                        {
                            //Credit Card

                            case "1":
                                typeids.Add("5228205D51CA11D18F1400A02427D15E");
                                break;
                            case "2":
                                typeids.Add("5228205E51CA11D18F1400A02427D15E");
                                break;
                            case "3":
                                typeids.Add("5228206051CA11D18F1400A02427D15E");
                                break;
                            case "4":
                                typeids.Add("5228206351CA11D18F1400A02427D15E");
                                break;

                            //Ende Creditcard

                            //Features

                            case "5":
                                typeids.Add("120F297CE74511D181FB006097B896BA");
                                break;
                            case "6":
                                typeids.Add("52281FC851CA11D18F1400A02427D15E");
                                break;
                            case "7":
                                typeids.Add("5228206A51CA11D18F1400A02427D15E");
                                break;
                            case "8":
                                typeids.Add("5228206B51CA11D18F1400A02427D15E");
                                break;
                            case "9":
                                typeids.Add("5228206E51CA11D18F1400A02427D15E");
                                break;
                            case "10":
                                typeids.Add("5228207951CA11D18F1400A02427D15E");
                                break;
                            case "11":
                                typeids.Add("5228207A51CA11D18F1400A02427D15E");
                                break;
                            case "12":
                                typeids.Add("5228207B51CA11D18F1400A02427D15E");
                                break;
                            case "13":
                                typeids.Add("5228207F51CA11D18F1400A02427D15E");
                                break;
                            case "14":
                                typeids.Add("5228209151CA11D18F1400A02427D15E");
                                break;
                            case "15":
                                typeids.Add("5228211651CA11D18F1400A02427D15E");
                                break;
                            case "16":
                                typeids.Add("5228211F51CA11D18F1400A02427D15E");
                                break;
                            case "17":
                                typeids.Add("452422597831423F9F4E2B1A2BA9177A");
                                break;
                            case "18":
                                typeids.Add("63534DC188314AC68DAB0EF0DE6EE5B0");
                                break;
                            case "19":
                                typeids.Add("B3BC8F4D7BA948369515FBA8075D47DB");
                                break;

                            //Ende Facilities

                            //Quality

                            case "20":
                                typeids.Add("46AD7938616B4D4882A006BEF3B199A4");
                                break;
                            case "21":
                                typeids.Add("F0A385D0E8E44944AFCA3893712A1420");
                                break;
                            case "22":
                                typeids.Add("2FA54F6F350748AE9CD1A389A5C9EDD9");
                                break;
                            case "23":
                                typeids.Add("C0E761D71CC44F4C80D75FF68ED72C55");
                                break;
                            case "24":
                                typeids.Add("6797D594C7BF4C7AA6D384B234EC7C44");
                                break;
                            case "25":
                                typeids.Add("E5775068F5644E92B7CF94BDFCDA5175");
                                break;
                            case "26":
                                typeids.Add("1FFD5352501542BF8BCB24B7BF75CF4F");
                                break;
                            case "27":
                                typeids.Add("1641B07E28B9443EAB53E1DB7363F6F3");
                                break;
                            case "28":
                                typeids.Add("5060F78090604B2E97A96D86B97D2E0B");
                                break;
                            case "29":
                                typeids.Add("ED4028BEE0164BF185B923B3DD4FF9A0");
                                break;
                            case "30":
                                typeids.Add("0DBA881DD41340FDA76196EBCEFC9ECD");
                                break;

                            //Ende Quality

                            //Cuisine

                            case "31":
                                typeids.Add("5228204A51CA11D18F1400A02427D15E");
                                break;
                            case "32":
                                typeids.Add("71A7D4A821F7437EA1DC05CEE9655A5A");
                                break;
                            case "33":
                                typeids.Add("11A6BEA7EEFC4716BDF8FBD5E15C0CFB");
                                break;
                            case "34":
                                typeids.Add("F42DBD202D6E4289AF48D138DA09ECB7");
                                break;
                            case "35":
                                typeids.Add("2476B5BBAEB7467C9A0099F06D0ED004");
                                break;
                            case "36":
                                typeids.Add("30DC854F943D42CF8DB140CF4A90EC7E");
                                break;
                            case "37":
                                typeids.Add("D1F124A123554B14AB9600F2313ED051");
                                break;
                            case "38":
                                typeids.Add("CB8AF7CB80E844758B18E9C4E2D84035");
                                break;
                            case "39":
                                typeids.Add("50FFF83EB75944DE9F6F15CC51E85E7A");
                                break;
                            case "40":
                                typeids.Add("6322DE8AFE8E406F886E7C40D0DC1ADD");
                                break;
                            case "41":
                                typeids.Add("0E9721E540FB4D84BADC0DFA24F0543B");
                                break;
                            case "42":
                                typeids.Add("C48E7E7679B04835B6744650E129BABF");
                                break;
                            case "43":
                                typeids.Add("167850CF26984D50A59A5F42EB24A0AD");
                                break;
                            case "44":
                                typeids.Add("4F9335FDAB834B11B36CD4C163F990A7");
                                break;
                            case "45":
                                typeids.Add("69621AE51DF942A1BBED32D460E65132");
                                break;
                            case "46":
                                typeids.Add("22F0D9C42B06423EB63E1F2F27B7CA3A");
                                break;
                            case "47":
                                typeids.Add("BC08B00995564BB28997C55C870120D1");
                                break;
                            case "48":
                                typeids.Add("0E55D7C2A7BC4866BF8438C522C17254");
                                break;
                            case "49":
                                typeids.Add("FC627623C6994E37927F6048E32B79C2");
                                break;
                            case "50":
                                typeids.Add("21A903DE35654070803DFDDF29C67291");
                                break;
                            case "51":
                                typeids.Add("8E28215F82BA430EA016BA5D1C776A30");
                                break;
                            case "52":
                                typeids.Add("D413EF912D18462CA0055A44F55351D1");
                                break;
                            case "53":
                                typeids.Add("BC6B57D90AFB496098DD0D059D04EE7C");
                                break;
                            case "54":
                                typeids.Add("B36D855D60CB4D79BA78F3FEFEE9F9D3");
                                break;
                            case "55":
                                typeids.Add("AD8426538FCF4D8A81E06BE044088BAA");
                                break;
                            case "56":
                                typeids.Add("5C84265DA5F84F84A7896808ACCB675A");
                                break;

                                //Ende Cuisine


                        }
                    }
                }
            }
            return typeids;
        }

        #region Flags

        public static List<string> CreateGastroCeremonyCodeListfromFlag(string? capacityceremonyfilter)
        {
            List<string> ceremonycodeids = new List<string>();

            if (!String.IsNullOrEmpty(capacityceremonyfilter) && capacityceremonyfilter != "null")
            {
                if (int.TryParse(capacityceremonyfilter, out var ceremonycodefilterint))
                {                    
                    GastroCeremonyFlag myceremonycodeflag = (GastroCeremonyFlag)ceremonycodefilterint;

                    var myflags = myceremonycodeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        ceremonycodeids.Add(myflag);
                    }
                }
            }

            return ceremonycodeids;
        }

        public static List<string> CreateGastroDishCodeListfromFlag(string? dishcodefilter)
        {
            List<string> dishcodeids = new List<string>();

            if (!String.IsNullOrEmpty(dishcodefilter) && dishcodefilter != "null")
            {
                if (int.TryParse(dishcodefilter, out var dishcodefilterint))
                {                    
                    GastroDishcodeFlag mydishcodeflag = (GastroDishcodeFlag)dishcodefilterint;

                    var myflags = mydishcodeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        dishcodeids.Add(myflag);
                    }
                }
            }

            return dishcodeids;
        }

        public static List<string> CreateGastroCategoryCodeListfromFlag(string? categorycodefilter)
        {
            List<string> categorycodeids = new List<string>();

            if (!String.IsNullOrEmpty(categorycodefilter) && categorycodefilter != "null")
            {
                if (int.TryParse(categorycodefilter, out var categorycodefilterint))
                {                    
                    GastroCategoryFlag mycategorycodeflag = (GastroCategoryFlag)categorycodefilterint;

                    var myflags = mycategorycodeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        categorycodeids.Add(myflag);
                    }
                }
            }

            return categorycodeids;
        }

        public static List<string> CreateGastroFacilityCodeListfromFlag(string? facilitycodefilter)
        {
            List<string> facilitycodeids = new List<string>();

            if (!String.IsNullOrEmpty(facilitycodefilter) && facilitycodefilter != "null")
            {
                if (long.TryParse(facilitycodefilter, out var facilitycodefilterint))
                {                    
                    GastroFacilityFlag myfacilitycodeflag = (GastroFacilityFlag)facilitycodefilterint;

                    var myflags = myfacilitycodeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        facilitycodeids.Add(myflag);
                    }
                }
            }

            return facilitycodeids;
        }

        public static List<string> CreateGastroCusineCodeListfromFlag(string? facilitycodefilter)
        {
            List<string> facilitycodeids = new List<string>();

            if (!String.IsNullOrEmpty(facilitycodefilter) && facilitycodefilter != "null")
            {
                if (long.TryParse(facilitycodefilter, out var facilitycodefilterint))
                {                    
                    GastroCuisineFlag myfacilitycodeflag = (GastroCuisineFlag)facilitycodefilterint;

                    var myflags = myfacilitycodeflag.GetFlags().GetDescriptionList();


                    foreach (var myflag in myflags)
                    {
                        facilitycodeids.Add(myflag);
                    }
                }
            }

            return facilitycodeids;
        }

        #endregion

    }
}
