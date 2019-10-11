using System.Collections.Generic;

namespace Helper
{
    public class ActivityPoiListCreator
    {
        #region Normal Typelistcreators

        public static List<string> CreateActivityTypeList(string typefilter)
        {
            List<string> typeids = new List<string>();

            if (typefilter != null)
            {
                if (typefilter.Substring(typefilter.Length - 1, 1) == ",")
                    typefilter = typefilter[0..^1];

                var splittedfilter = typefilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    typeids.Add(filter);
                }
            }

            return typeids;
        }

        public static List<string> CreateActivitySubTypeList(string subtypefilter)
        {
            List<string> typeids = new List<string>();

            if (subtypefilter != null)
            {
                if (subtypefilter.Substring(subtypefilter.Length - 1, 1) == ",")
                    subtypefilter = subtypefilter[0..^1];

                var splittedfilter = subtypefilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    typeids.Add(filter);
                }
            }

            return typeids;
        }

        public static List<string> CreateActivitySubTypeList(string activitytype, string subtypefilter)
        {
            List<string> typeids = new List<string>();

            if (subtypefilter != null)
            {
                if (subtypefilter.Substring(subtypefilter.Length - 1, 1) == ",")
                    subtypefilter = subtypefilter[0..^1];

                var splittedfilter = subtypefilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    switch (activitytype)
                    {
                        case "Berg":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Bergtour");
                                    break;
                                case "two":
                                    typeids.Add("Hochtour");
                                    break;
                                case "three":
                                    typeids.Add("Klettersteig");
                                    break;
                                case "four":
                                    typeids.Add("Klettertour");
                                    break;
                                case "five":
                                    typeids.Add("Skitour");
                                    break;
                                case "six":
                                    typeids.Add("Schneeschuhtour");
                                    break;
                            }

                            break;

                        case "Radfahren":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("MTB");
                                    break;
                                case "two":
                                    typeids.Add("Rennrad");
                                    break;
                                case "three":
                                    typeids.Add("Radtour");
                                    break;
                                case "four":
                                    typeids.Add("Downhill");
                                    break;
                            }

                            break;


                        case "Wandern":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Wandern");
                                    break;
                                case "two":
                                    typeids.Add("Familienwandern");
                                    break;
                                case "three":
                                    typeids.Add("Winterwandern");
                                    break;
                                case "four":
                                    typeids.Add("Themenwandern");
                                    break;
                                case "five":
                                    typeids.Add("Schneeschuhwandern");
                                    break;
                            }

                            break;

                        case "Laufen und Fitness":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Laufstrecke");
                                    break;
                                case "two":
                                    typeids.Add("Berglauf");
                                    break;
                                case "three":
                                    typeids.Add("Orientierungslauf");
                                    break;
                                case "four":
                                    typeids.Add("Schnellwandern (speed hiking)");
                                    break;
                                case "five":
                                    typeids.Add("Fitnessparcours");
                                    break;
                                case "six":
                                    typeids.Add("Schnellgehen mit Stöcken (Nordic Walking)");
                                    break;
                            }

                            break;

                        case "Rodelbahnen":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Rodelbahn");
                                    break;
                                case "two":
                                    typeids.Add("Alpin Bob");
                                    break;
                            }

                            break;

                        case "Aufstiegsanlagen":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Seilbahn");
                                    break;
                                case "two":
                                    typeids.Add("Umlaufbahn");
                                    break;
                                case "three":
                                    typeids.Add("Kabinenbahn");
                                    break;
                                case "four":
                                    typeids.Add("Unterirdische Bahn");
                                    break;
                                case "five":
                                    typeids.Add("Sessellift");
                                    break;
                                case "six":
                                    typeids.Add("Skilift");
                                    break;
                                case "seven":
                                    typeids.Add("Schrägaufzug");
                                    break;
                                case "eight":
                                    typeids.Add("Standseilbahn/Zahnradbahn");
                                    break;
                                case "nine":
                                    typeids.Add("Telemix");
                                    break;
                                case "ten":
                                    typeids.Add("2er Sessellift kuppelbar");
                                    break;
                                case "eleven":
                                    typeids.Add("3er Sessellift kuppelbar");
                                    break;
                                case "twelve":
                                    typeids.Add("4er Sessellift kuppelbar");
                                    break;
                                case "thirtheen":
                                    typeids.Add("6er Sessellift kuppelbar");
                                    break;
                                case "fourteen":
                                    typeids.Add("8er Sessellift kuppelbar");
                                    break;
                                case "fiftheen":
                                    typeids.Add("Klein-Skilift");
                                    break;
                            }

                            break;

                        case "Piste":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Ski alpin");
                                    break;
                                case "two":
                                    typeids.Add("Halfpipe");
                                    break;
                                case "three":
                                    typeids.Add("Tiefschnee");
                                    break;
                                case "four":
                                    typeids.Add("Snowpark");
                                    break;
                                case "five":
                                    typeids.Add("Kids-Funpark");
                                    break;
                                case "six":
                                    typeids.Add("Geschwindigkeitspiste");
                                    break;
                                case "seven":
                                    typeids.Add("Slalompiste");
                                    break;
                            }

                            break;

                        case "Loipen":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("klassisch");
                                    break;
                                case "two":
                                    typeids.Add("Freistil");
                                    break;
                                case "three":
                                    typeids.Add("klassisch und Freistil");
                                    break;
                            }

                            break;
                    }


                }
            }

            return typeids;
        }

        public static List<string> CreatePoiSubTypeList(string poitype, string subtypefilter)
        {
            List<string> typeids = new List<string>();

            if (subtypefilter != null)
            {
                if (subtypefilter.Substring(subtypefilter.Length - 1, 1) == ",")
                    subtypefilter = subtypefilter[0..^1];

                var splittedfilter = subtypefilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    switch (poitype)
                    {
                        case "Sport und Freizeiteinrichtungen":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Fußballplätze");
                                    break;
                                case "two":
                                    typeids.Add("Tennisplätze");
                                    break;
                                case "three":
                                    typeids.Add("Kletterhallen");
                                    break;
                                case "four":
                                    typeids.Add("Eisklettern");
                                    break;
                                case "five":
                                    typeids.Add("Kletterparks");
                                    break;
                                case "six":
                                    typeids.Add("Skischulen/-clubs");
                                    break;
                                case "seven":
                                    typeids.Add("Volleyball - Beachvolleyball");
                                    break;
                                case "eight":
                                    typeids.Add("Sportanlage allgemein");
                                    break;
                                case "nine":
                                    typeids.Add("Nordic Walking Park");
                                    break;
                                case "ten":
                                    typeids.Add("Kinderspielplätze");
                                    break;
                                case "eleven":
                                    typeids.Add("Schlittschuhlaufen");
                                    break;
                                case "twelve":
                                    typeids.Add("Schwimmbäder");
                                    break;
                                case "thirteen":
                                    typeids.Add("Gewässer");
                                    break;
                                case "fourteen":
                                    typeids.Add("Sportschulen");
                                    break;
                                case "fiftheen":
                                    typeids.Add("Hallenbäder");
                                    break;
                                case "sixteen":
                                    typeids.Add("Skiverleih");
                                    break;
                                case "seventeen":
                                    typeids.Add("Radverleih");
                                    break;
                                case "eighteen":
                                    typeids.Add("Radraststätte");
                                    break;
                                case "nineteen":
                                    typeids.Add("Badeseen");
                                    break;
                                case "twenty":
                                    typeids.Add("Golfplätze");
                                    break;
                                case "twentyone":
                                    typeids.Add("Minigolfplätze");
                                    break;
                                case "twentytwo":
                                    typeids.Add("Wasserstellen");
                                    break;
                                case "twentythree":
                                    typeids.Add("Reitställe");
                                    break;
                                case "twentyfour":
                                    typeids.Add("Fischergewässer");
                                    break;
                            }

                            break;

                        case "Kunsthandwerk und Brauchtum":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Handwerk aller Art");
                                    break;
                                case "two":
                                    typeids.Add("Kunsthandwerk");
                                    break;
                            }

                            break;

                        case "Nachtleben und Unterhaltung":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Disco");
                                    break;
                                case "two":
                                    typeids.Add("Nachtklubs");
                                    break;
                            }

                            break;

                        case "Verkehr und Transport":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Taxi/Mietwagen mit Fahrer/Bus");
                                    break;
                                case "two":
                                    typeids.Add("Tankstellen");
                                    break;
                                case "three":
                                    typeids.Add("Parkplätze");
                                    break;
                                case "four":
                                    typeids.Add("Bushaltestellen");
                                    break;
                                case "five":
                                    typeids.Add("Zugbahnhof");
                                    break;
                            }

                            break;

                        case "Öffentliche Einrichtungen":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Öffentliches Wlan");
                                    break;
                                case "two":
                                    typeids.Add("Infopoints");
                                    break;
                                case "three":
                                    typeids.Add("Infobüro");
                                    break;
                                case "four":
                                    typeids.Add("WC");
                                    break;
                                case "five":
                                    typeids.Add("Apotheken");
                                    break;
                            }

                            break;

                        case "Kultur und Sehenswürdigkeiten":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Kirchen, Kapellen");
                                    break;
                                case "two":
                                    typeids.Add("Kulturzentren");
                                    break;
                                case "three":
                                    typeids.Add("Bibliotheken");
                                    break;
                                case "four":
                                    typeids.Add("Gedenkstätten");
                                    break;
                                case "five":
                                    typeids.Add("Naturdenkmäler");
                                    break;
                                case "six":
                                    typeids.Add("Naturparkhäuser");
                                    break;
                                case "seven":
                                    typeids.Add("Industriedenkmäler");
                                    break;
                                case "eight":
                                    typeids.Add("Produktionsstätten");
                                    break;
                                case "nine":
                                    typeids.Add("Theater");
                                    break;
                                case "ten":
                                    typeids.Add("Wasserfälle");
                                    break;
                                case "eleven":
                                    typeids.Add("Aussichtspunkte");
                                    break;
                                case "twelve":
                                    typeids.Add("Schlösser, Burgen");
                                    break;
                                case "thirteen":
                                    typeids.Add("Mystische Stätten");
                                    break;
                                case "fourteen":
                                    typeids.Add("Kulturdenkmäler");
                                    break;
                                case "fiftheen":
                                    typeids.Add("Ansitze, historische Häuser");
                                    break;
                                case "sixteen":
                                    typeids.Add("Ruinen");
                                    break;
                                case "seventeen":
                                    typeids.Add("Klöster");
                                    break;
                                case "eighteen":
                                    typeids.Add("Museen und Ausstellungen");
                                    break;
                                case "nineteen":
                                    typeids.Add("Gärten, Parks");
                                    break;
                                case "twenty":
                                    typeids.Add("Historische Straßen und Plätze");
                                    break;
                                case "twentyone":
                                    typeids.Add("Denkmäler");
                                    break;
                            }

                            break;

                        case "Geschäfte und Dienstleister":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Lebensmittel");
                                    break;
                                case "two":
                                    typeids.Add("Metzgerei");
                                    break;
                                case "three":
                                    typeids.Add("Juwelier");
                                    break;
                                case "four":
                                    typeids.Add("Reinigung");
                                    break;
                                case "five":
                                    typeids.Add("Einkaufszentrum");
                                    break;
                                case "six":
                                    typeids.Add("Drogerie");
                                    break;
                                case "seven":
                                    typeids.Add("Antiquitäten");
                                    break;
                                case "eight":
                                    typeids.Add("Friseur");
                                    break;
                                case "nine":
                                    typeids.Add("Computerzubehör und Technik");
                                    break;
                                case "ten":
                                    typeids.Add("Kellerei");
                                    break;
                                case "eleven":
                                    typeids.Add("Mode/Bekleidung");
                                    break;
                                case "twelve":
                                    typeids.Add("Optiker");
                                    break;
                                case "thirteen":
                                    typeids.Add("Bergführer");
                                    break;
                                case "fourteen":
                                    typeids.Add("Wanderführer");
                                    break;
                                case "fiftheen":
                                    typeids.Add("bike guides");
                                    break;
                                case "sixteen":
                                    typeids.Add("Bankomats");
                                    break;
                                case "seventeen":
                                    typeids.Add("Blumen");
                                    break;
                                case "eighteen":
                                    typeids.Add("Sportartikel");
                                    break;
                                case "nineteen":
                                    typeids.Add("Souvenirs");
                                    break;
                                case "twenty":
                                    typeids.Add("Zeitungen");
                                    break;
                                case "twentyone":
                                    typeids.Add("Lederwaren/Schuhe");
                                    break;
                                case "twentytwo":
                                    typeids.Add("Bildhauer/Schnitzer");
                                    break;
                                case "twentythree":
                                    typeids.Add("Foto");
                                    break;
                            }

                            break;

                        case "Gesundheit und Wohlbefinden":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Hausärzte");
                                    break;
                                case "two":
                                    typeids.Add("Kinderärzte");
                                    break;
                                case "three":
                                    typeids.Add("Massagen");
                                    break;
                                case "four":
                                    typeids.Add("Schönheitssalon");
                                    break;
                                case "five":
                                    typeids.Add("Beauty");
                                    break;
                                case "six":
                                    typeids.Add("Sauna/Heilbäder");
                                    break;
                            }

                            break;
                    }


                }
            }

            return typeids;
        }

        public static List<string> CreateSmgPoiSubTypeList(string poitype, string subtypefilter)
        {
            List<string> typeids = new List<string>();

            if (subtypefilter != null)
            {
                if (subtypefilter.Substring(subtypefilter.Length - 1, 1) == ",")
                    subtypefilter = subtypefilter[0..^1];

                var splittedfilter = subtypefilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    switch (poitype)
                    {
                        case "Wellness Entspannung":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Wellnessbehandlungen");
                                    break;
                                case "two":
                                    typeids.Add("Therme Wasserwelten");
                                    break;
                            }

                            break;

                        case "Winter":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Weihnachtsmärkte");
                                    break;
                                case "two":
                                    typeids.Add("Eisklettern");
                                    break;
                                case "three":
                                    typeids.Add("Eislaufen");
                                    break;
                                case "four":
                                    typeids.Add("Langlaufen");
                                    break;
                                case "five":
                                    typeids.Add("Pferdeschlittenfahrten");
                                    break;
                                case "six":
                                    typeids.Add("Rodeln");
                                    break;
                                case "seven":
                                    typeids.Add("Skirundtouren Pisten");
                                    break;
                                case "eight":
                                    typeids.Add("Snowparks");
                                    break;
                                case "nine":
                                    typeids.Add("Skischulen Skiverleih");
                                    break;
                                case "ten":
                                    typeids.Add("Skitouren");
                                    break;
                                case "eleven":
                                    typeids.Add("Schneeschuhwandern");
                                    break;
                                case "twelve":
                                    typeids.Add("Winterwandern");
                                    break;
                                case "thirteen":
                                    typeids.Add("Skigebiete");
                                    break;
                            }

                            break;

                        case "Sommer":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Wandern");
                                    break;
                                case "two":
                                    typeids.Add("Bergsteigen");
                                    break;
                                case "three":
                                    typeids.Add("Klettern");
                                    break;
                                case "four":
                                    typeids.Add("Radfahren Radtouren");
                                    break;
                                case "five":
                                    typeids.Add("Radverleih");
                                    break;
                                case "six":
                                    typeids.Add("Badeseen Freibäder");
                                    break;
                                case "seven":
                                    typeids.Add("Wassersport");
                                    break;
                                case "eight":
                                    typeids.Add("Reiten");
                                    break;
                                case "nine":
                                    typeids.Add("Freizeit Erlebnis");
                                    break;
                                case "ten":
                                    typeids.Add("Laufen Fitness");
                                    break;
                                case "eleven":
                                    typeids.Add("Golf");
                                    break;
                                case "twelve":
                                    typeids.Add("Paragleiten");
                                    break;
                                case "thirteen":
                                    typeids.Add("Angeln Fischen");
                                    break;
                            }

                            break;

                        case "Kultur Sehenswürdigkeiten":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Architektur");
                                    break;
                                case "two":
                                    typeids.Add("Kirchen Klöster");
                                    break;
                                case "three":
                                    typeids.Add("Museen");
                                    break;
                                case "four":
                                    typeids.Add("Naturparkhäuser");
                                    break;
                                case "five":
                                    typeids.Add("Burgen Schlösser");
                                    break;
                                case "six":
                                    typeids.Add("Bergwerke");
                                    break;
                                case "seven":
                                    typeids.Add("Mystische Plätze");
                                    break;
                            }

                            break;

                        case "Essen Trinken":

                            switch (filter)
                            {
                                //case "one":
                                //    typeids.Add("Restaurants Gasthäuser");
                                //    break;
                                //case "two":
                                //    typeids.Add("Hütten Almen");
                                //    break;
                                //case "three":
                                //    typeids.Add("Bäuerliche Schankbetriebe");
                                //    break;
                                //case "four":
                                //    typeids.Add("Weinkellereien");
                                //    break;
                                case "one":
                                    typeids.Add("Essen Trinken");
                                    break;
                                case "two":
                                    typeids.Add("Weinkellereien");
                                    break;
                            }

                            break;

                        case "Anderes":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Ohne Zuordnung");
                                    break;
                                case "two":
                                    typeids.Add("Familienurlaub");
                                    break;
                                case "three":
                                    typeids.Add("Sagen");
                                    break;
                                case "four":
                                    typeids.Add("Klettertour");
                                    break;
                                case "five":
                                    typeids.Add("Aufstiegsanlagen");
                                    break;
                                case "six":
                                    typeids.Add("Stadtrundgang");
                                    break;
                            }

                            break;





                    }


                }
            }

            return typeids;
        }

        public static List<string> CreateSmgPoiPoiTypeList(string subtype, string poitypefilter)
        {
            List<string> typeids = new List<string>();

            if (poitypefilter != null)
            {
                if (poitypefilter.Substring(poitypefilter.Length - 1, 1) == ",")
                    poitypefilter = poitypefilter[0..^1];

                var splittedfilter = poitypefilter.Split(',');

                foreach (var filter in splittedfilter)
                {
                    switch (subtype)
                    {
                        case "Wandern":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Fernwanderwege");
                                    break;
                                case "two":
                                    typeids.Add("Höhenwege");
                                    break;
                                case "three":
                                    typeids.Add("Themenwanderungen");
                                    break;
                                case "four":
                                    typeids.Add("Waalwege");
                                    break;
                            }

                            break;

                        case "Klettern":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Klettersteige");
                                    break;
                                case "two":
                                    typeids.Add("Kletterparks");
                                    break;
                                case "three":
                                    typeids.Add("Hochseilgärten");
                                    break;
                                case "four":
                                    typeids.Add("Kletterhallen");
                                    break;
                            }

                            break;

                        case "Radfahren Radtouren":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Radtouren");
                                    break;
                                case "two":
                                    typeids.Add("Talradwege");
                                    break;
                                case "three":
                                    typeids.Add("Mountainbike");
                                    break;
                                case "four":
                                    typeids.Add("Rennrad");
                                    break;
                                case "five":
                                    typeids.Add("Freeride");
                                    break;
                                case "six":
                                    typeids.Add("Downhill");
                                    break;
                            }

                            break;

                        case "Museen":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Museen Kultur");
                                    break;
                                case "two":
                                    typeids.Add("Museen Natur");
                                    break;
                                case "three":
                                    typeids.Add("Museen Technik");
                                    break;
                                case "four":
                                    typeids.Add("Museen Kunst");
                                    break;
                            }

                            break;

                        case "Freizeit Erlebnis":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Beachvolleyball");
                                    break;
                                case "two":
                                    typeids.Add("Freizeitparks");
                                    break;
                                case "three":
                                    typeids.Add("Minigolf");
                                    break;
                            }

                            break;

                        case "Kirchen Klöster":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Kirchen");
                                    break;
                                case "two":
                                    typeids.Add("Klöster");
                                    break;
                            }

                            break;

                        case "Laufen Fitness":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Laufstrecken");
                                    break;
                                case "two":
                                    typeids.Add("Nordic Walking");
                                    break;
                            }

                            break;

                        case "Therme Wasserwelten":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Hallenbad");
                                    break;
                                case "two":
                                    typeids.Add("Therme");
                                    break;
                            }

                            break;

                        case "Weihnachtsmärkte":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Ländliche Christkindlmärkte");
                                    break;
                                case "two":
                                    typeids.Add("Original Südtiroler Christkindlmärkte");
                                    break;
                            }

                            break;

                        case "Langlaufen":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Klassisch");
                                    break;
                                case "two":
                                    typeids.Add("Freistil");
                                    break;
                            }

                            break;

                        case "Rodeln":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Rodelbahn");
                                    break;
                                case "two":
                                    typeids.Add("Alpin Bob");
                                    break;
                            }

                            break;

                        case "Skischulen Skiverleih":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Skischulen");
                                    break;
                                case "two":
                                    typeids.Add("Skiverleih");
                                    break;
                            }

                            break;

                        case "Skigebiete":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Dolomiti Superski");
                                    break;
                                case "two":
                                    typeids.Add("Ortler Skiarena");
                                    break;
                                case "three":
                                    typeids.Add("Tauferer Ahrntal");
                                    break;
                                case "four":
                                    typeids.Add("Skiverbund Eisacktaler Wipptal");
                                    break;
                            }

                            break;

                        case "Burgen Schlösser":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Ruine");
                                    break;
                            }

                            break;

                        case "Architektur":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Kulinarik");
                                    break;
                                case "two":
                                    typeids.Add("Kunst Kultur");
                                    break;
                                case "three":
                                    typeids.Add("Freizeitparks");
                                    break;
                                case "four":
                                    typeids.Add("Öffentliches Gebäude");
                                    break;
                                case "five":
                                    typeids.Add("Wohnbauten");
                                    break;
                                case "six":
                                    typeids.Add("Wohnen Hotels");
                                    break;
                            }

                            break;

                        case "Badeseen Freibäder":

                            switch (filter)
                            {
                                case "one":
                                    typeids.Add("Schwimmbäder");
                                    break;
                                case "two":
                                    typeids.Add("Badeseen");
                                    break;
                            }

                            break;

                            //case "Restaurants Gasthäuser":
                            //    switch (filter)
                            //    {
                            //        case "one":
                            //            typeids.Add("Restaurants");
                            //            break;
                            //        case "two":
                            //            typeids.Add("Gasthäuser Gasthöfe  ");
                            //            break;
                            //        case "three":
                            //            typeids.Add("Pizzerias");
                            //            break;
                            //        case "four":
                            //            typeids.Add("Vinotheken");
                            //            break;
                            //        case "five":
                            //            typeids.Add("Bars Cafés Bistros");
                            //            break;                                                      
                            //    }

                            //    break;
                            //case "Hütten Almen":
                            //    switch (filter)
                            //    {
                            //        case "one":
                            //            typeids.Add("Schutzhütten");
                            //            break;
                            //        case "two":
                            //            typeids.Add("Almen");
                            //            break;
                            //        case "three":
                            //            typeids.Add("Skihütten");
                            //            break; 
                            //    }

                            //    break;
                            //case "Bäuerliche Schankbetriebe":
                            //    switch (filter)
                            //    {
                            //        case "one":
                            //            typeids.Add("Buschen Hofschänke");
                            //            break;

                            //    }

                            //    break;



                    }


                }
            }

            return typeids;
        }

        #endregion

        #region Flags

        //Activity Data

        public static List<string> CreateActivityTypefromFlag(string typefilter)
        {
            List<string> typelist = new List<string>();

            if (typefilter != null)
            {
                if (int.TryParse(typefilter, out int typefilterint))
                {
                    ActivityTypeFlag mypoitypeflag = (ActivityTypeFlag)typefilterint;

                    var myflags = mypoitypeflag.GetFlags().GetDescriptionList();

                    foreach (var myflag in myflags)
                    {
                        typelist.Add(myflag);
                    }
                }
                else
                    return new List<string>();
            }

            return typelist;
        }

        public static List<string> CreateActivitySubTypefromFlag(string typefiltertext, string? subtypefilter)
        {
            List<string> subtypelist = new List<string>();

            if (subtypefilter != null)
            {
                if (long.TryParse(subtypefilter, out long typefilterint))
                {

                    switch (typefiltertext)
                    {
                        case "Berg":
                            ActivityTypeBerg mypoitypeflag1 = (ActivityTypeBerg)typefilterint;
                            subtypelist.AddRange(mypoitypeflag1.GetFlags().GetDescriptionList());

                            break;
                        case "Radfahren":
                            ActivityTypeRadfahren mypoitypeflag2 = (ActivityTypeRadfahren)typefilterint;
                            subtypelist.AddRange(mypoitypeflag2.GetFlags().GetDescriptionList());

                            break;
                        case "Stadtrundgang":
                            ActivityTypeOrtstouren mypoitypeflag3 = (ActivityTypeOrtstouren)typefilterint;
                            subtypelist.AddRange(mypoitypeflag3.GetFlags().GetDescriptionList());

                            break;
                        case "Pferdesport":
                            ActivityTypePferde mypoitypeflag4 = (ActivityTypePferde)typefilterint;
                            subtypelist.AddRange(mypoitypeflag4.GetFlags().GetDescriptionList());

                            break;
                        case "Wandern":
                            ActivityTypeWandern mypoitypeflag5 = (ActivityTypeWandern)typefilterint;
                            subtypelist.AddRange(mypoitypeflag5.GetFlags().GetDescriptionList());

                            break;

                        case "Laufen und Fitness":
                            ActivityTypeLaufenFitness mypoitypeflag6 = (ActivityTypeLaufenFitness)typefilterint;
                            subtypelist.AddRange(mypoitypeflag6.GetFlags().GetDescriptionList());

                            break;

                        case "Loipen":
                            ActivityTypeLoipen mypoitypeflag7 = (ActivityTypeLoipen)typefilterint;
                            subtypelist.AddRange(mypoitypeflag7.GetFlags().GetDescriptionList());

                            break;

                        case "Rodelbahnen":
                            ActivityTypeRodeln mypoitypeflag8 = (ActivityTypeRodeln)typefilterint;
                            subtypelist.AddRange(mypoitypeflag8.GetFlags().GetDescriptionList());

                            break;

                        case "Piste":
                            ActivityTypePisten mypoitypeflag9 = (ActivityTypePisten)typefilterint;
                            subtypelist.AddRange(mypoitypeflag9.GetFlags().GetDescriptionList());

                            break;

                        case "Aufstiegsanlagen":
                            ActivityTypeAufstiegsanlagen mypoitypeflag10 = (ActivityTypeAufstiegsanlagen)typefilterint;
                            subtypelist.AddRange(mypoitypeflag10.GetFlags().GetDescriptionList());


                            break;

                    }
                }
                else
                    return new List<string>();
            }

            return subtypelist;
        }



        //Poi Data

        public static List<string> CreatePoiTypefromFlag(string typefilter)
        {
            List<string> typelist = new List<string>();

            if (typefilter != null)
            {
                if (int.TryParse(typefilter, out int typefilterint))
                {
                    PoiTypeFlag mypoitypeflag = (PoiTypeFlag)typefilterint;

                    var myflags = mypoitypeflag.GetFlags().GetDescriptionList();


                    foreach (var myflag in myflags)
                    {
                        typelist.Add(myflag);
                    }
                }
                else
                    return new List<string>();
            }

            return typelist;
        }

        public static List<string> CreatePoiSubTypefromFlag(string typefiltertext, string? subtypefilter)
        {
            List<string> subtypelist = new List<string>();

            if (subtypefilter != null)
            {
                if (long.TryParse(subtypefilter, out long typefilterint))
                {

                    switch (typefiltertext)
                    {
                        case "Ärzte, Apotheken":
                            PoiTypeAerzteApotheken mypoitypeflag1 = (PoiTypeAerzteApotheken)typefilterint;
                            subtypelist.AddRange(mypoitypeflag1.GetFlags().GetDescriptionList());

                            break;
                        case "Kultur und Sehenswürdigkeiten":
                            PoiTypeKulturSehenswuerdigkeiten mypoitypeflag2 = (PoiTypeKulturSehenswuerdigkeiten)typefilterint;
                            subtypelist.AddRange(mypoitypeflag2.GetFlags().GetDescriptionList());

                            break;
                        case "Nachtleben und Unterhaltung":
                            PoiTypeNachtlebenUnterhaltung mypoitypeflag3 = (PoiTypeNachtlebenUnterhaltung)typefilterint;
                            subtypelist.AddRange(mypoitypeflag3.GetFlags().GetDescriptionList());

                            break;
                        case "Öffentliche Einrichtungen":
                            PoiTypeOeffentlicheEinrichtungen mypoitypeflag4 = (PoiTypeOeffentlicheEinrichtungen)typefilterint;
                            subtypelist.AddRange(mypoitypeflag4.GetFlags().GetDescriptionList());

                            break;
                        case "Verkehr und Transport":
                            PoiTypeVerkehrTransport mypoitypeflag5 = (PoiTypeVerkehrTransport)typefilterint;
                            subtypelist.AddRange(mypoitypeflag5.GetFlags().GetDescriptionList());

                            break;

                        case "Sport und Freizeit":
                            PoiTypeSportFreizeit mypoitypeflag6 = (PoiTypeSportFreizeit)typefilterint;
                            subtypelist.AddRange(mypoitypeflag6.GetFlags().GetDescriptionList());

                            break;

                        case "Geschäfte und Dienstleister":
                            PoiTypeGeschaefteDienstleister mypoitypeflag7 = (PoiTypeGeschaefteDienstleister)typefilterint;
                            subtypelist.AddRange(mypoitypeflag7.GetFlags().GetDescriptionList());

                            break;

                        case "Geschäfte":
                            PoiTypeGeschaefte mypoitypeflag8 = (PoiTypeGeschaefte)typefilterint;
                            subtypelist.AddRange(mypoitypeflag8.GetFlags().GetDescriptionList());

                            break;

                        case "Dienstleister":
                            PoiTypeDienstleister mypoitypeflag9 = (PoiTypeDienstleister)typefilterint;
                            subtypelist.AddRange(mypoitypeflag9.GetFlags().GetDescriptionList());

                            break;

                        case "Kunsthandwerker":
                            PoiTypeHandwerk mypoitypeflag10 = (PoiTypeHandwerk)typefilterint;
                            subtypelist.AddRange(mypoitypeflag10.GetFlags().GetDescriptionList());

                            break;


                    }
                }
                else
                    return new List<string>();
            }

            return subtypelist;
        }


        //SmgPois

        public static List<string> CreateSmgPoiTypefromFlag(string typefilter)
        {
            List<string> typelist = new List<string>();

            if (typefilter != null)
            {
                if (int.TryParse(typefilter, out int typefilterint))
                {
                    SmgPoiTypeFlag mypoitypeflag = (SmgPoiTypeFlag)typefilterint;

                    var myflags = mypoitypeflag.GetFlags().GetDescriptionList();


                    foreach (var myflag in myflags)
                    {
                        typelist.Add(myflag);
                    }
                }
                else
                    return new List<string>();
            }

            return typelist;
        }

        public static List<string> CreateSmgPoiSubTypefromFlag(string typefiltertext, string subtypefilter)
        {
            List<string> subtypelist = new List<string>();

            if (subtypefilter != null)
            {
                if (long.TryParse(subtypefilter, out long typefilterint))
                {

                    switch (typefiltertext)
                    {
                        case "Wellness Entspannung":
                            SmgPoiSubTypeFlagWellness mypoitypeflag1 = (SmgPoiSubTypeFlagWellness)typefilterint;
                            subtypelist.AddRange(mypoitypeflag1.GetFlags().GetDescriptionList());

                            break;
                        case "Winter":
                            SmgPoiSubTypeFlagWinter mypoitypeflag2 = (SmgPoiSubTypeFlagWinter)typefilterint;
                            subtypelist.AddRange(mypoitypeflag2.GetFlags().GetDescriptionList());

                            break;
                        case "Sommer":
                            SmgPoiSubTypeFlagSommer mypoitypeflag3 = (SmgPoiSubTypeFlagSommer)typefilterint;
                            subtypelist.AddRange(mypoitypeflag3.GetFlags().GetDescriptionList());

                            break;
                        case "Kultur Sehenswürdigkeiten":
                            SmgPoiSubTypeFlagKultur mypoitypeflag4 = (SmgPoiSubTypeFlagKultur)typefilterint;
                            subtypelist.AddRange(mypoitypeflag4.GetFlags().GetDescriptionList());

                            break;
                        case "Anderes":
                            SmgPoiSubTypeFlagAnderes mypoitypeflag5 = (SmgPoiSubTypeFlagAnderes)typefilterint;
                            subtypelist.AddRange(mypoitypeflag5.GetFlags().GetDescriptionList());

                            break;

                        case "Essen Trinken":
                            SmgPoiSubTypeFlagEssenTrinken mypoitypeflag6 = (SmgPoiSubTypeFlagEssenTrinken)typefilterint;
                            subtypelist.AddRange(mypoitypeflag6.GetFlags().GetDescriptionList());

                            break;

                    }
                }
                else
                    return new List<string>();
            }

            return subtypelist;
        }

        public static List<string> CreateSmgPoiPoiTypefromFlag(string subtypefilter, string poitypefilter)
        {
            List<string> subtypelist = new List<string>();

            if (poitypefilter != null)
            {
                if (long.TryParse(poitypefilter, out long typefilterint))
                {

                    switch (subtypefilter)
                    {
                        case "Wandern":
                            SmgPoiPoiTypeFlagWandern mypoitypeflag1 = (SmgPoiPoiTypeFlagWandern)typefilterint;
                            subtypelist.AddRange(mypoitypeflag1.GetFlags().GetDescriptionList());

                            break;
                        case "Klettern":
                            SmgPoiPoiTypeFlagKlettern mypoitypeflag2 = (SmgPoiPoiTypeFlagKlettern)typefilterint;
                            subtypelist.AddRange(mypoitypeflag2.GetFlags().GetDescriptionList());

                            break;
                        case "Radfahren Radtouren":
                            SmgPoiPoiTypeFlagRadfahren mypoitypeflag3 = (SmgPoiPoiTypeFlagRadfahren)typefilterint;
                            subtypelist.AddRange(mypoitypeflag3.GetFlags().GetDescriptionList());

                            break;
                        case "Museen":
                            SmgPoiPoiTypeFlagMuseen mypoitypeflag4 = (SmgPoiPoiTypeFlagMuseen)typefilterint;
                            subtypelist.AddRange(mypoitypeflag4.GetFlags().GetDescriptionList());

                            break;
                        case "Freizeit Erlebnis":
                            SmgPoiPoiTypeFlagFreizeitErlebnis mypoitypeflag5 = (SmgPoiPoiTypeFlagFreizeitErlebnis)typefilterint;
                            subtypelist.AddRange(mypoitypeflag5.GetFlags().GetDescriptionList());

                            break;

                        case "Kirchen Klöster":
                            SmgPoiPoiTypeFlagKirchenKloester mypoitypeflag6 = (SmgPoiPoiTypeFlagKirchenKloester)typefilterint;
                            subtypelist.AddRange(mypoitypeflag6.GetFlags().GetDescriptionList());

                            break;
                        case "Laufen Fitness":
                            SmgPoiPoiTypeFlagLaufenFitness mypoitypeflag7 = (SmgPoiPoiTypeFlagLaufenFitness)typefilterint;
                            subtypelist.AddRange(mypoitypeflag7.GetFlags().GetDescriptionList());

                            break;

                        case "Therme Wasserwelten":
                            SmgPoiPoiTypeFlagThermeWasserwelten mypoitypeflag9 = (SmgPoiPoiTypeFlagThermeWasserwelten)typefilterint;
                            subtypelist.AddRange(mypoitypeflag9.GetFlags().GetDescriptionList());

                            break;

                        case "Weihnachtsmärkte":
                            SmgPoiPoiTypeFlagWeihnachtsmaerkte mypoitypeflag10 = (SmgPoiPoiTypeFlagWeihnachtsmaerkte)typefilterint;
                            subtypelist.AddRange(mypoitypeflag10.GetFlags().GetDescriptionList());

                            break;

                        case "Langlaufen":
                            SmgPoiPoiTypeFlagLanglaufen mypoitypeflag11 = (SmgPoiPoiTypeFlagLanglaufen)typefilterint;
                            subtypelist.AddRange(mypoitypeflag11.GetFlags().GetDescriptionList());

                            break;

                        case "Rodeln":
                            SmgPoiPoiTypeFlagRodelbahnen mypoitypeflag12 = (SmgPoiPoiTypeFlagRodelbahnen)typefilterint;
                            subtypelist.AddRange(mypoitypeflag12.GetFlags().GetDescriptionList());

                            break;

                        case "Skischulen Skiverleih":
                            SmgPoiPoiTypeFlagSkischulen mypoitypeflag13 = (SmgPoiPoiTypeFlagSkischulen)typefilterint;
                            subtypelist.AddRange(mypoitypeflag13.GetFlags().GetDescriptionList());

                            break;

                        case "Skigebiete":
                            SmgPoiPoiTypeFlagSkigebiete mypoitypeflag113 = (SmgPoiPoiTypeFlagSkigebiete)typefilterint;
                            subtypelist.AddRange(mypoitypeflag113.GetFlags().GetDescriptionList());

                            break;

                        case "Burgen Schlösser":
                            SmgPoiPoiTypeFlagBurgenSchloesser mypoitypeflag14 = (SmgPoiPoiTypeFlagBurgenSchloesser)typefilterint;
                            subtypelist.AddRange(mypoitypeflag14.GetFlags().GetDescriptionList());

                            break;

                        case "Badeseen Freibäder":
                            SmgPoiPoiTypeFlagBadeseenFreibaeder mypoitypeflag15 = (SmgPoiPoiTypeFlagBadeseenFreibaeder)typefilterint;
                            subtypelist.AddRange(mypoitypeflag15.GetFlags().GetDescriptionList());

                            break;

                        case "Architektur":
                            SmgPoiPoiTypeFlagArchitektur mypoitypeflag16 = (SmgPoiPoiTypeFlagArchitektur)typefilterint;
                            subtypelist.AddRange(mypoitypeflag16.GetFlags().GetDescriptionList());

                            break;

                        case "Restaurants Gasthäuser":
                            SmgPoiPoiTypeFlagRestaurantsGasthauser mypoitypeflag17 = (SmgPoiPoiTypeFlagRestaurantsGasthauser)typefilterint;
                            subtypelist.AddRange(mypoitypeflag17.GetFlags().GetDescriptionList());

                            break;

                        case "Hütten Almen":
                            SmgPoiPoiTypeFlagHuettenAlmen mypoitypeflag18 = (SmgPoiPoiTypeFlagHuettenAlmen)typefilterint;
                            subtypelist.AddRange(mypoitypeflag18.GetFlags().GetDescriptionList());

                            break;

                        case "Bäuerliche Schankbetriebe":
                            SmgPoiPoiTypeFlagBauerlicheSchankbetriebe mypoitypeflag19 = (SmgPoiPoiTypeFlagBauerlicheSchankbetriebe)typefilterint;
                            subtypelist.AddRange(mypoitypeflag19.GetFlags().GetDescriptionList());

                            break;

                    }


                }
                else
                    return new List<string>();
            }

            return subtypelist;
        }


        #endregion
    }
}
