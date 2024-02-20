using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIAG.Model
{    
    public class SiagWeatherForecastModel
    {
        public Info info { get; set; }
        public Municipality[] municipalities { get; set; }
    }

    public class Info
    {
        public string model { get; set; }
        public DateTime currentModelRun { get; set; }
        public DateTime nextModelRun { get; set; }
        public string fileName { get; set; }
        public DateTime fileCreationDate { get; set; }
        public int absTempMin { get; set; }
        public int absTempMax { get; set; }
        public int absPrecMin { get; set; }
        public int absPrecMax { get; set; }
    }

    public class Municipality
    {
        public string code { get; set; }
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public Tempmin24 tempMin24 { get; set; }
        public Tempmax24 tempMax24 { get; set; }
        public Temp3 temp3 { get; set; }
        public Ssd24 ssd24 { get; set; }
        public Precprob3 precProb3 { get; set; }
        public Precprob24 precProb24 { get; set; }
        public Precsum3 precSum3 { get; set; }
        public Precsum24 precSum24 { get; set; }
        public Symbols3 symbols3 { get; set; }
        public Winddir3 windDir3 { get; set; }
        public Windspd3 windSpd3 { get; set; }
        public Symbols24 symbols24 { get; set; }
    }

    public class Tempmin24
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public DateTime date { get; set; }
        public int value { get; set; }
    }

    public class DatumFloat : Datum
    {        
        public new float value { get; set; }
    }

    public class DatumString : Datum
    {        
        public new string value { get; set; }
    }

    public class Tempmax24
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
        public Datum[] data { get; set; }
    }
    

    public class Temp3
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
        public DatumFloat[] data { get; set; }
    }
    

    public class Ssd24
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
        public Datum[] data { get; set; }
    }    

    public class Precprob3
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
        public Datum[] data { get; set; }
    }

    public class Precprob24
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
        public Datum[] data { get; set; }
    }


    public class Precsum3
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
        public DatumFloat[] data { get; set; }
    }

    public class Precsum24
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
        public Datum[] data { get; set; }
    }


    public class Symbols3
    {
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public DatumString[] data { get; set; }
    }


    public class Winddir3
    {
        public Datum[] data { get; set; }
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
    }


    public class Windspd3
    {
        public Datum[] data { get; set; }
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
        public string unit { get; set; }
    }    

    public class Symbols24
    {
        public DatumString[] data { get; set; }
        public string nameDe { get; set; }
        public string nameIt { get; set; }
        public string nameEn { get; set; }
        public string nameRm { get; set; }
    }


}
