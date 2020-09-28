using DataModel;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SIAG
{
    public class ParseWeather
    {
        //string urlfrom indicates 'siag' old parsing, 'opendata' new url

        public static Weather ParsemyWeatherResponse(string lang, XDocument weatherdataxml, XDocument weatherresponse, string urlfrom)
        {
            try
            {
                Weather myweather = new Weather();

                myweather.Id = Convert.ToInt32(weatherresponse.Root.Element("Id").Value);
                myweather.date = Convert.ToDateTime(weatherresponse.Root.Element("date").Value.Replace("00:00:00", weatherresponse.Root.Element("hour").Value + ":00"));
                myweather.evolution = weatherresponse.Root.Element("evolution") != null ? weatherresponse.Root.Element("evolution").Value : null;
                myweather.evolutiontitle = weatherresponse.Root.Element("evolutionTitle") != null ? weatherresponse.Root.Element("evolutionTitle").Value : null;

                var mydayforecasts = weatherresponse.Root.Elements("dayForecast");
                var mountaintoday = weatherresponse.Root.Element("mountainToday");
                var mountaintomorrow = weatherresponse.Root.Element("mountainTomorrow");
                var today = weatherresponse.Root.Element("today");
                var tomorrow = weatherresponse.Root.Element("tomorrow");

                //Forecast info
                foreach (XElement forecast in mydayforecasts)
                {
                    Forecast myforecast = new Forecast();

                    myforecast.date = Convert.ToDateTime(forecast.Element("date").Value);

                    if (forecast.Element("reliability") != null)
                    {
                        myforecast.Reliability = forecast.Element("reliability").Value;

                        myforecast.Weathercode = forecast.Element("symbol").Element("code").Value;
                        myforecast.Weatherdesc = forecast.Element("symbol").Element("description").Value;
                        myforecast.WeatherImgurl = forecast.Element("symbol").Element("imageURL").Value;

                        myforecast.TempMaxmax = Convert.ToInt32(forecast.Element("tempMax").Element("max").Value);
                        myforecast.TempMaxmin = Convert.ToInt32(forecast.Element("tempMax").Element("min").Value);

                        myforecast.TempMinmax = Convert.ToInt32(forecast.Element("tempMin").Element("max").Value);
                        myforecast.TempMinmin = Convert.ToInt32(forecast.Element("tempMin").Element("min").Value);

                        myweather.Forecast.Add(myforecast);
                    }
                }

                // mountain Info

                if (mountaintoday != null)
                {
                    Mountain mymountaintoday = new Mountain();
                    mymountaintoday.date = Convert.ToDateTime(mountaintoday.Element("date").Value);
                    mymountaintoday.Title = mountaintoday.Element("title") != null ? mountaintoday.Element("title").Value : "";
                    mymountaintoday.Zerolimit = mountaintoday.Element("zerolimit") != null ? mountaintoday.Element("zerolimit").Value : "";
                    mymountaintoday.Weatherdesc = mountaintoday.Element("weather") != null ? mountaintoday.Element("weather").Value : "";

                    mymountaintoday.Conditions = mountaintoday.Element("conditions") != null ? mountaintoday.Element("conditions").Value : "";
                    mymountaintoday.MountainImgurl = mountaintoday.Element("imageURL").Value;
                    mymountaintoday.Moonrise = mountaintoday.Elements("moonRise").Count() > 0 ? mountaintoday.Element("moonRise").Value : "";
                    mymountaintoday.Moonset = mountaintoday.Elements("moonSet").Count() > 0 ? mountaintoday.Element("moonSet").Value : "";
                    mymountaintoday.Sunrise = mountaintoday.Elements("sunRise").Count() > 0 ? mountaintoday.Element("sunRise").Value : "";
                    mymountaintoday.Sunset = mountaintoday.Elements("sunSet").Count() > 0 ? mountaintoday.Element("sunSet").Value : "";
                    mymountaintoday.Reliability = mountaintoday.Element("reliability").Value;
                    mymountaintoday.Temp1000 = Convert.ToInt32(mountaintoday.Element("temp1000").Value);
                    mymountaintoday.Temp2000 = Convert.ToInt32(mountaintoday.Element("temp2000").Value);
                    mymountaintoday.Temp3000 = Convert.ToInt32(mountaintoday.Element("temp3000").Value);
                    mymountaintoday.Temp4000 = Convert.ToInt32(mountaintoday.Element("temp4000").Value);

                    mymountaintoday.Northcode = mountaintoday.Element("north").Element("code").Value;
                    mymountaintoday.Northdesc = mountaintoday.Element("north").Element("description").Value;
                    mymountaintoday.Northimgurl = mountaintoday.Element("north").Element("imageURL").Value;
                    mymountaintoday.Southcode = mountaintoday.Element("south").Element("code").Value;
                    mymountaintoday.Southdesc = mountaintoday.Element("south").Element("description").Value;
                    mymountaintoday.Southimgurl = mountaintoday.Element("south").Element("imageURL").Value;
                    mymountaintoday.Windcode = mountaintoday.Element("wind").Element("code").Value;
                    mymountaintoday.Winddesc = mountaintoday.Element("wind").Element("description").Value;
                    mymountaintoday.WindImgurl = mountaintoday.Element("wind").Element("imageURL").Value;

                    myweather.Mountain.Add(mymountaintoday);
                }

                // mountain Info Tomorrow
                if (mountaintomorrow != null)
                {
                    if (mountaintomorrow.HasElements)
                    {
                        Mountain mymountaintomorrow = new Mountain();
                        mymountaintomorrow.date = Convert.ToDateTime(mountaintomorrow.Element("date").Value);
                        mymountaintomorrow.Title = mountaintomorrow.Element("title") != null ? mountaintomorrow.Element("title").Value : "";
                        mymountaintomorrow.Zerolimit = mountaintomorrow.Element("zerolimit") != null ? mountaintomorrow.Element("zerolimit").Value : "";
                        mymountaintomorrow.Weatherdesc = mountaintomorrow.Element("weather") != null ? mountaintomorrow.Element("weather").Value : "";

                        mymountaintomorrow.Conditions = mountaintomorrow.Element("conditions") != null ? mountaintomorrow.Element("conditions").Value : "";
                        mymountaintomorrow.MountainImgurl = mountaintomorrow.Element("imageURL") != null ? mountaintomorrow.Element("imageURL").Value : "";
                        mymountaintomorrow.Moonrise = mountaintomorrow.Elements("moonRise").Count() > 0 ? mountaintomorrow.Element("moonRise").Value : "";
                        mymountaintomorrow.Moonset = mountaintomorrow.Elements("moonSet").Count() > 0 ? mountaintomorrow.Element("moonSet").Value : "";
                        mymountaintomorrow.Sunrise = mountaintomorrow.Elements("sunRise").Count() > 0 ? mountaintomorrow.Element("sunRise").Value : "";
                        mymountaintomorrow.Sunset = mountaintomorrow.Elements("sunSet").Count() > 0 ? mountaintomorrow.Element("sunSet").Value : "";
                        mymountaintomorrow.Reliability = mountaintomorrow.Element("reliability") != null ? mountaintomorrow.Element("reliability").Value : "";
                        mymountaintomorrow.Temp1000 = mountaintomorrow.Element("temp1000") != null ? Convert.ToInt32(mountaintomorrow.Element("temp1000").Value) : 0;
                        mymountaintomorrow.Temp2000 = mountaintomorrow.Element("temp2000") != null ? Convert.ToInt32(mountaintomorrow.Element("temp2000").Value) : 0;
                        mymountaintomorrow.Temp3000 = mountaintomorrow.Element("temp3000") != null ? Convert.ToInt32(mountaintomorrow.Element("temp3000").Value) : 0;
                        mymountaintomorrow.Temp4000 = mountaintomorrow.Element("temp4000") != null ? Convert.ToInt32(mountaintomorrow.Element("temp4000").Value) : 0;

                        mymountaintomorrow.Northcode = mountaintomorrow.Element("north").Element("code").Value;
                        mymountaintomorrow.Northdesc = mountaintomorrow.Element("north").Element("description").Value;
                        mymountaintomorrow.Northimgurl = mountaintomorrow.Element("north").Element("imageURL").Value;
                        mymountaintomorrow.Southcode = mountaintomorrow.Element("south").Element("code").Value;
                        mymountaintomorrow.Southdesc = mountaintomorrow.Element("south").Element("description").Value;
                        mymountaintomorrow.Southimgurl = mountaintomorrow.Element("south").Element("imageURL").Value;
                        mymountaintomorrow.Windcode = mountaintomorrow.Element("wind").Element("code").Value;
                        mymountaintomorrow.Winddesc = mountaintomorrow.Element("wind").Element("description").Value;
                        mymountaintomorrow.WindImgurl = mountaintomorrow.Element("wind").Element("imageURL").Value;

                        myweather.Mountain.Add(mymountaintomorrow);
                    }
                }

                //Today Info
                if (today != null)
                {
                    Conditions myconditiontoday = new Conditions();

                    myconditiontoday.date = Convert.ToDateTime(today.Element("date").Value);
                    myconditiontoday.WeatherCondition = today.Element("conditions") != null ? today.Element("conditions").Value : "";
                    myconditiontoday.WeatherImgurl = today.Element("imageURL") != null ? today.Element("imageURL").Value : "";
                    myconditiontoday.Weatherdesc = today.Element("weather") != null ? today.Element("weather").Value : "";
                    myconditiontoday.Title = today.Element("title") != null ? today.Element("title").Value : "";
                    myconditiontoday.Temperatures = today.Element("temperatures") != null ? today.Element("temperatures").Value : "";

                    myweather.Conditions.Add(myconditiontoday);
                }
                //Stationdata today
                if (today != null)
                {
                    var stationstoday = today.Elements("stationData");

                    if (stationstoday != null)
                    {
                        foreach (XElement stationtoday in stationstoday)
                        {
                            Stationdata mystationdatatoday = new Stationdata();

                            mystationdatatoday.date = Convert.ToDateTime(today.Element("date").Value);

                            mystationdatatoday.Id = Convert.ToInt32(stationtoday.Element("Id").Value);
                            var mystationdatacity = weatherdataxml.Root.Elements("Station").Where(x => x.Attribute("Id").Value.Equals(stationtoday.Element("Id").Value)).FirstOrDefault();
                            mystationdatatoday.CityName = mystationdatacity.Attribute("Name" + lang.ToUpper()).Value;

                            if (lang == "en")
                            {
                                if (mystationdatatoday.CityName.Contains("/"))
                                {
                                    mystationdatatoday.CityName = mystationdatatoday.CityName.Replace("/", " / ");
                                }
                            }
                            //mystationdatatoday.stationdatacityrid = mystationdatacity.Attribute("RID").Value;

                            mystationdatatoday.WeatherCode = stationtoday.Element("symbol").Element("code").Value;
                            mystationdatatoday.WeatherDesc = stationtoday.Element("symbol").Element("description").Value;
                            mystationdatatoday.WeatherImgUrl = stationtoday.Element("symbol").Element("imageURL").Value;

                            if (urlfrom == "siag")
                            {
                                mystationdatatoday.Maxtemp = Convert.ToInt32(stationtoday.Element("temperature").Element("max").Value);
                                mystationdatatoday.MinTemp = Convert.ToInt32(stationtoday.Element("temperature").Element("min").Value);
                            }
                            else
                            {
                                mystationdatatoday.Maxtemp = Convert.ToInt32(stationtoday.Element("max").Value);
                                mystationdatatoday.MinTemp = Convert.ToInt32(stationtoday.Element("min").Value);
                            }

                            myweather.Stationdata.Add(mystationdatatoday);
                        }
                    }
                }

                //Tomorrow info
                if (tomorrow != null)
                {
                    if (tomorrow.HasElements)
                    {
                        Conditions myconditiontomorrow = new Conditions();

                        myconditiontomorrow.date = Convert.ToDateTime(tomorrow.Element("date").Value);
                        myconditiontomorrow.WeatherCondition = tomorrow.Element("conditions") != null ? tomorrow.Element("conditions").Value : "";
                        myconditiontomorrow.WeatherImgurl = tomorrow.Element("imageURL") != null ? tomorrow.Element("imageURL").Value : "";
                        myconditiontomorrow.Weatherdesc = tomorrow.Element("weather") != null ? tomorrow.Element("weather").Value : "";
                        myconditiontomorrow.Title = tomorrow.Element("title") != null ? tomorrow.Element("title").Value : "";
                        myconditiontomorrow.Temperatures = tomorrow.Element("temperatures") != null ? tomorrow.Element("temperatures").Value : "";

                        myweather.Conditions.Add(myconditiontomorrow);
                    }
                }

                //Stationdata today
                if (tomorrow != null)
                {
                    if (tomorrow.HasElements)
                    {
                        var stationstomorrow = tomorrow.Elements("stationData");
                        if (stationstomorrow != null)
                        {
                            foreach (XElement stationtomorrow in stationstomorrow)
                            {
                                Stationdata mystationdatatomorrow = new Stationdata();

                                mystationdatatomorrow.date = Convert.ToDateTime(tomorrow.Element("date").Value);

                                mystationdatatomorrow.Id = Convert.ToInt32(stationtomorrow.Element("Id").Value);
                                var mystationdatacity = weatherdataxml.Root.Elements("Station").Where(x => x.Attribute("Id").Value.Equals(stationtomorrow.Element("Id").Value)).FirstOrDefault();
                                mystationdatatomorrow.CityName = mystationdatacity.Attribute("Name" + lang.ToUpper()).Value;
                                //mystationdatatomorrow.stationdatacityrid = mystationdatacity.Attribute("RID").Value;

                                if (lang == "en")
                                {
                                    if (mystationdatatomorrow.CityName.Contains("/"))
                                    {
                                        mystationdatatomorrow.CityName = mystationdatatomorrow.CityName.Replace("/", " / ");
                                    }
                                }

                                mystationdatatomorrow.WeatherCode = stationtomorrow.Element("symbol").Element("code").Value;
                                mystationdatatomorrow.WeatherDesc = stationtomorrow.Element("symbol").Element("description").Value;
                                mystationdatatomorrow.WeatherImgUrl = stationtomorrow.Element("symbol").Element("imageURL").Value;

                                if (urlfrom == "siag")
                                {
                                    mystationdatatomorrow.Maxtemp = Convert.ToInt32(stationtomorrow.Element("temperature").Element("max").Value);
                                    mystationdatatomorrow.MinTemp = Convert.ToInt32(stationtomorrow.Element("temperature").Element("min").Value);
                                }
                                else
                                {
                                    mystationdatatomorrow.Maxtemp = Convert.ToInt32(stationtomorrow.Element("max").Value);
                                    mystationdatatomorrow.MinTemp = Convert.ToInt32(stationtomorrow.Element("min").Value);
                                }

                                myweather.Stationdata.Add(mystationdatatomorrow);
                            }
                        }
                    }
                }

                return myweather;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Weather ParsemyStationWeatherResponse(string lang, XDocument weatherdataxml, XDocument weatherresponse, string stationid, string urlfrom)
        {
            Weather myweather = new Weather();

            myweather.Id = Convert.ToInt32(weatherresponse.Root.Element("Id").Value);
            myweather.date = Convert.ToDateTime(weatherresponse.Root.Element("date").Value.Replace("00:00:00", weatherresponse.Root.Element("hour").Value + ":00"));

            var today = weatherresponse.Root.Element("today");
            var tomorrow = weatherresponse.Root.Element("tomorrow");

            //Stationdata today
            if (today != null)
            {
                var stationstoday = today.Elements("stationData");

                if (stationstoday != null)
                {
                    foreach (XElement stationtoday in stationstoday.Where(x => x.Element("Id").Value == stationid))
                    {
                        Stationdata mystationdatatoday = new Stationdata();

                        mystationdatatoday.date = Convert.ToDateTime(today.Element("date").Value);

                        mystationdatatoday.Id = Convert.ToInt32(stationtoday.Element("Id").Value);
                        var mystationdatacity = weatherdataxml.Root.Elements("Station").Where(x => x.Attribute("Id").Value.Equals(stationtoday.Element("Id").Value)).FirstOrDefault();
                        mystationdatatoday.CityName = mystationdatacity.Attribute("Name" + lang.ToUpper()).Value;

                        if (lang == "en")
                        {
                            if (mystationdatatoday.CityName.Contains("/"))
                            {
                                mystationdatatoday.CityName = mystationdatatoday.CityName.Replace("/", " / ");
                            }
                        }

                        //mystationdatatoday.stationdatacityrid = mystationdatacity.Attribute("RID").Value;

                        mystationdatatoday.WeatherCode = stationtoday.Element("symbol").Element("code").Value;
                        mystationdatatoday.WeatherDesc = stationtoday.Element("symbol").Element("description").Value;
                        mystationdatatoday.WeatherImgUrl = stationtoday.Element("symbol").Element("imageURL").Value;

                        if (urlfrom == "siag")
                        {
                            mystationdatatoday.Maxtemp = Convert.ToInt32(stationtoday.Element("temperature").Element("max").Value);
                            mystationdatatoday.MinTemp = Convert.ToInt32(stationtoday.Element("temperature").Element("min").Value);
                        }
                        else
                        {
                            mystationdatatoday.Maxtemp = Convert.ToInt32(stationtoday.Element("max").Value);
                            mystationdatatoday.MinTemp = Convert.ToInt32(stationtoday.Element("min").Value);
                        }

                        myweather.Stationdata.Add(mystationdatatoday);
                    }
                }
            }

            //Stationdata today
            if (tomorrow != null)
            {
                var stationstomorrow = tomorrow.Elements("stationData");
                if (stationstomorrow != null)
                {
                    foreach (XElement stationtomorrow in stationstomorrow.Where(x => x.Element("Id").Value == stationid))
                    {
                        Stationdata mystationdatatomorrow = new Stationdata();

                        mystationdatatomorrow.date = Convert.ToDateTime(tomorrow.Element("date").Value);

                        mystationdatatomorrow.Id = Convert.ToInt32(stationtomorrow.Element("Id").Value);
                        var mystationdatacity = weatherdataxml.Root.Elements("Station").Where(x => x.Attribute("Id").Value.Equals(stationtomorrow.Element("Id").Value)).FirstOrDefault();
                        mystationdatatomorrow.CityName = mystationdatacity.Attribute("Name" + lang.ToUpper()).Value;

                        if (lang == "en")
                        {
                            if (mystationdatatomorrow.CityName.Contains("/"))
                            {
                                mystationdatatomorrow.CityName = mystationdatatomorrow.CityName.Replace("/", " / ");
                            }
                        }
                        //mystationdatatomorrow.stationdatacityrid = mystationdatacity.Attribute("RID").Value;

                        mystationdatatomorrow.WeatherCode = stationtomorrow.Element("symbol").Element("code").Value;
                        mystationdatatomorrow.WeatherDesc = stationtomorrow.Element("symbol").Element("description").Value;
                        mystationdatatomorrow.WeatherImgUrl = stationtomorrow.Element("symbol").Element("imageURL").Value;

                        if (urlfrom == "siag")
                        {
                            //mystationdatatomorrow.Maxtemp = Convert.ToInt32(stationtomorrow.Element("temperature").Element("max").Value);
                            //mystationdatatomorrow.MinTemp = Convert.ToInt32(stationtomorrow.Element("temperature").Element("min").Value);
                        }
                        else
                        {
                            mystationdatatomorrow.Maxtemp = Convert.ToInt32(stationtomorrow.Element("max").Value);
                            mystationdatatomorrow.MinTemp = Convert.ToInt32(stationtomorrow.Element("min").Value);
                        }

                        myweather.Stationdata.Add(mystationdatatomorrow);
                    }
                }
            }

            return myweather;
        }

        public static BezirksWeather ParsemyBezirksWeatherResponse(string lang, XDocument weatherresponse, string urlfrom)
        {
            BezirksWeather myweather = new BezirksWeather();

            myweather.Id = Convert.ToInt32(weatherresponse.Root.Element("district").Element("Id").Value);
            myweather.DistrictName = weatherresponse.Root.Element("district").Element("name").Value;

            myweather.date = Convert.ToDateTime(weatherresponse.Root.Element("date").Value.Replace("00:00:00", weatherresponse.Root.Element("hour").Value + ":00"));

            var mybezirkforecasts = weatherresponse.Root.Elements("forecast");

            List<BezirksForecast> mybezirksforecastlist = new List<BezirksForecast>();

            //Tomorrow info
            foreach (var mybezirkforecast in mybezirkforecasts)
            {

                BezirksForecast bezirksforecast = new BezirksForecast();

                bezirksforecast.date = Convert.ToDateTime(mybezirkforecast.Element("date").Value);

                bezirksforecast.WeatherCode = mybezirkforecast.Element("symbol").Element("code").Value;
                bezirksforecast.WeatherDesc = mybezirkforecast.Element("symbol").Element("description").Value;
                bezirksforecast.WeatherImgUrl = mybezirkforecast.Element("symbol").Element("imageURL").Value;

                bezirksforecast.Freeze = Convert.ToInt16(mybezirkforecast.Element("freeze").Value);
                bezirksforecast.RainFrom = Convert.ToInt16(mybezirkforecast.Element("rainFrom").Value);
                bezirksforecast.RainTo = Convert.ToInt16(mybezirkforecast.Element("rainTo").Value);

                if (urlfrom == "siag")
                {
                    bezirksforecast.MaxTemp = Convert.ToInt16(mybezirkforecast.Element("temperature").Element("max").Value);
                    bezirksforecast.MinTemp = Convert.ToInt16(mybezirkforecast.Element("temperature").Element("min").Value);

                    bezirksforecast.Part1 = mybezirkforecast.Element("part1") != null ? Convert.ToInt16(mybezirkforecast.Element("part1").Value) : 0;
                    bezirksforecast.Part2 = mybezirkforecast.Element("part2") != null ? Convert.ToInt16(mybezirkforecast.Element("part2").Value) : 0;
                    bezirksforecast.Part3 = mybezirkforecast.Element("part3") != null ? Convert.ToInt16(mybezirkforecast.Element("part3").Value) : 0;
                    bezirksforecast.Part4 = mybezirkforecast.Element("part4") != null ? Convert.ToInt16(mybezirkforecast.Element("part4").Value) : 0;

                    if (mybezirkforecast.Element("part3") == null && mybezirkforecast.Element("part4") == null)
                    {
                        bezirksforecast.Part1 = mybezirkforecast.Element("part1") != null ? Convert.ToInt16(mybezirkforecast.Element("part1").Value) : 0;
                        bezirksforecast.Part2 = mybezirkforecast.Element("part1") != null ? Convert.ToInt16(mybezirkforecast.Element("part1").Value) : 0;
                        bezirksforecast.Part3 = mybezirkforecast.Element("part2") != null ? Convert.ToInt16(mybezirkforecast.Element("part2").Value) : 0;
                        bezirksforecast.Part4 = mybezirkforecast.Element("part2") != null ? Convert.ToInt16(mybezirkforecast.Element("part2").Value) : 0;
                    }

                    bezirksforecast.Thunderstorm = Convert.ToInt16(mybezirkforecast.Element("thunderStorm").Value);
                }
                else
                {
                    bezirksforecast.MaxTemp = Convert.ToInt16(mybezirkforecast.Element("temperatureMax").Value);
                    bezirksforecast.MinTemp = Convert.ToInt16(mybezirkforecast.Element("temperatureMin").Value);

                    bezirksforecast.Part1 = mybezirkforecast.Element("rainTimespan1") != null ? Convert.ToInt16(mybezirkforecast.Element("rainTimespan1").Value) : 0;
                    bezirksforecast.Part2 = mybezirkforecast.Element("rainTimespan2") != null ? Convert.ToInt16(mybezirkforecast.Element("rainTimespan2").Value) : 0;
                    bezirksforecast.Part3 = mybezirkforecast.Element("rainTimespan3") != null ? Convert.ToInt16(mybezirkforecast.Element("rainTimespan3").Value) : 0;
                    bezirksforecast.Part4 = mybezirkforecast.Element("rainTimespan4") != null ? Convert.ToInt16(mybezirkforecast.Element("rainTimespan4").Value) : 0;

                    if (mybezirkforecast.Element("rainTimespan3") == null && mybezirkforecast.Element("rainTimespan4") == null)
                    {
                        bezirksforecast.Part1 = mybezirkforecast.Element("rainTimespan1") != null ? Convert.ToInt16(mybezirkforecast.Element("rainTimespan1").Value) : 0;
                        bezirksforecast.Part2 = mybezirkforecast.Element("rainTimespan1") != null ? Convert.ToInt16(mybezirkforecast.Element("rainTimespan1").Value) : 0;
                        bezirksforecast.Part3 = mybezirkforecast.Element("rainTimespan2") != null ? Convert.ToInt16(mybezirkforecast.Element("rainTimespan2").Value) : 0;
                        bezirksforecast.Part4 = mybezirkforecast.Element("rainTimespan2") != null ? Convert.ToInt16(mybezirkforecast.Element("rainTimespan2").Value) : 0;
                    }

                    bezirksforecast.Thunderstorm = Convert.ToInt16(mybezirkforecast.Element("storms").Value);
                }

                //TODO no 


                mybezirksforecastlist.Add(bezirksforecast);

            }

            myweather.BezirksForecast = mybezirksforecastlist.ToList();

            return myweather;
        }

        //TODO ADD JSON Parser

        public static Weather ParsemyWeatherJsonResponse(string lang, XDocument weatherdataxml, string weatherresponsejson)
        {
            var siagweather = JsonConvert.DeserializeObject<WeatherModel.SiagWeather>(weatherresponsejson);
            
            try
            {
                Weather myweather = new Weather();

                myweather.Id = Convert.ToInt32(siagweather.id);
                myweather.date = siagweather.date;
                myweather.evolution = siagweather.evolution;
                myweather.evolutiontitle = siagweather.evolutionTitle;
          
                //Forecast info
                foreach (var forecast in siagweather.dayForecasts)
                {
                    Forecast myforecast = new Forecast();

                    myforecast.date = forecast.date;
                    myforecast.Reliability = forecast.reliability.ToString();
                    myforecast.Weathercode = forecast.symbol.code;
                    myforecast.Weatherdesc = forecast.symbol.description;
                    myforecast.WeatherImgurl = forecast.symbol.imageUrl;

                    myforecast.TempMaxmax = forecast.tempMax.max;
                    myforecast.TempMaxmin = forecast.tempMax.min;

                    myforecast.TempMinmax = forecast.tempMin.max;
                    myforecast.TempMinmin = forecast.tempMin.min;

                    myweather.Forecast.Add(myforecast);
                }

                // mountain Info

                if (siagweather.mountainToday != null)
                {
                    Mountain mymountaintoday = new Mountain();
                    mymountaintoday.date = siagweather.mountainToday.date;
                    mymountaintoday.Title = siagweather.mountainToday.title;
                    mymountaintoday.Zerolimit = siagweather.mountainToday.zeroLimit.ToString();
                    mymountaintoday.Weatherdesc = siagweather.mountainToday.weather;

                    mymountaintoday.Conditions = siagweather.mountainToday.conditions;
                    mymountaintoday.MountainImgurl = siagweather.mountainToday.imageUrl;
                    mymountaintoday.Moonrise = siagweather.mountainToday.moonRise;
                    mymountaintoday.Moonset = siagweather.mountainToday.moonSet;
                    mymountaintoday.Sunrise = siagweather.mountainToday.sunRise;
                    mymountaintoday.Sunset = siagweather.mountainToday.sunSet;

                    mymountaintoday.Reliability = siagweather.mountainToday.reliability.ToString();
                    mymountaintoday.Temp1000 = siagweather.mountainToday.temp1000;
                    mymountaintoday.Temp2000 = siagweather.mountainToday.temp2000;
                    mymountaintoday.Temp3000 = siagweather.mountainToday.temp3000;
                    mymountaintoday.Temp4000 = siagweather.mountainToday.temp4000;

                    mymountaintoday.Northcode = siagweather.mountainToday.north.code;
                    mymountaintoday.Northdesc = siagweather.mountainToday.north.description;
                    mymountaintoday.Northimgurl = siagweather.mountainToday.north.imageUrl;
                    mymountaintoday.Southcode = siagweather.mountainToday.south.code;
                    mymountaintoday.Southdesc = siagweather.mountainToday.south.description;
                    mymountaintoday.Southimgurl = siagweather.mountainToday.south.imageUrl;
                    mymountaintoday.Windcode = siagweather.mountainToday.wind.code;
                    mymountaintoday.Winddesc = siagweather.mountainToday.wind.description;
                    mymountaintoday.WindImgurl = siagweather.mountainToday.wind.imageUrl;

                    myweather.Mountain.Add(mymountaintoday);
                }

                // mountain Info Tomorrow
                if (siagweather.mountainTomorrow != null)
                {
                    Mountain mymountaintomorrow = new Mountain();
                    mymountaintomorrow.date = siagweather.mountainTomorrow.date;
                    mymountaintomorrow.Title = siagweather.mountainTomorrow.title;
                    mymountaintomorrow.Zerolimit = siagweather.mountainTomorrow.zeroLimit.ToString();
                    mymountaintomorrow.Weatherdesc = siagweather.mountainTomorrow.weather;

                    mymountaintomorrow.Conditions = siagweather.mountainTomorrow.conditions;
                    mymountaintomorrow.MountainImgurl = siagweather.mountainTomorrow.imageUrl;
                    mymountaintomorrow.Moonrise = siagweather.mountainTomorrow.moonRise;
                    mymountaintomorrow.Moonset = siagweather.mountainTomorrow.moonSet;
                    mymountaintomorrow.Sunrise = siagweather.mountainTomorrow.sunRise;
                    mymountaintomorrow.Sunset = siagweather.mountainTomorrow.sunSet;
                    mymountaintomorrow.Reliability = siagweather.mountainTomorrow.reliability.ToString();
                    mymountaintomorrow.Temp1000 = siagweather.mountainTomorrow.temp1000;
                    mymountaintomorrow.Temp2000 = siagweather.mountainTomorrow.temp3000;
                    mymountaintomorrow.Temp3000 = siagweather.mountainTomorrow.temp3000;
                    mymountaintomorrow.Temp4000 = siagweather.mountainTomorrow.temp4000;

                    mymountaintomorrow.Northcode = siagweather.mountainTomorrow.north.code;
                    mymountaintomorrow.Northdesc = siagweather.mountainTomorrow.north.description;
                    mymountaintomorrow.Northimgurl = siagweather.mountainTomorrow.north.imageUrl;
                    mymountaintomorrow.Southcode = siagweather.mountainTomorrow.south.code;
                    mymountaintomorrow.Southdesc = siagweather.mountainTomorrow.south.description;
                    mymountaintomorrow.Southimgurl = siagweather.mountainTomorrow.south.imageUrl;
                    mymountaintomorrow.Windcode = siagweather.mountainTomorrow.wind.code;
                    mymountaintomorrow.Winddesc = siagweather.mountainTomorrow.wind.description;
                    mymountaintomorrow.WindImgurl = siagweather.mountainTomorrow.wind.imageUrl;

                    myweather.Mountain.Add(mymountaintomorrow);                   
                }

                //Today Info
                if (siagweather.today != null)
                {
                    Conditions myconditiontoday = new Conditions();
                    
                    myconditiontoday.date = Convert.ToDateTime(siagweather.today.date.ToShortDateString() + " " + siagweather.today.hour) ; //TODO CHeck
                    myconditiontoday.WeatherCondition = siagweather.today.conditions;
                    myconditiontoday.WeatherImgurl = siagweather.today.imageUrl;
                    myconditiontoday.Weatherdesc = siagweather.today.weather;
                    myconditiontoday.Title = siagweather.today.title;
                    myconditiontoday.Temperatures = siagweather.today.temperatures;

                    myconditiontoday.bulletinStatus = siagweather.today.bulletinStatus;
                    myconditiontoday.Reliability = siagweather.today.reliability.ToString();
                    myconditiontoday.TempMaxmax = siagweather.today.tMaxMax;
                    myconditiontoday.TempMaxmin = siagweather.today.tMaxMin;
                    myconditiontoday.TempMinmax = siagweather.today.tMinMax;
                    myconditiontoday.TempMinmin = siagweather.today.tMinMin;

                    myweather.Conditions.Add(myconditiontoday);
                }
                //Stationdata today
                if (siagweather.today != null)
                {
                    var stationstoday = siagweather.today.stationData;

                    if (stationstoday != null)
                    {
                        int i = 1;

                        foreach (var stationtoday in stationstoday)
                        {
                            Stationdata mystationdatatoday = new Stationdata();

                            mystationdatatoday.date = Convert.ToDateTime(siagweather.today.date.ToShortDateString() + " " + siagweather.today.hour); //TODO CHeck

                            mystationdatatoday.Id = i;
                            var mystationdatacity = weatherdataxml.Root.Elements("Station").Where(x => x.Attribute("Id").Value.Equals(i)).FirstOrDefault();
                            mystationdatatoday.CityName = mystationdatacity.Attribute("Name" + lang.ToUpper()).Value;

                            if (lang == "en")
                            {
                                if (mystationdatatoday.CityName.Contains("/"))
                                {
                                    mystationdatatoday.CityName = mystationdatatoday.CityName.Replace("/", " / ");
                                }
                            }
                            //mystationdatatoday.stationdatacityrid = mystationdatacity.Attribute("RID").Value;

                            mystationdatatoday.WeatherCode = stationtoday.symbol.code;
                            mystationdatatoday.WeatherDesc = stationtoday.symbol.description;
                            mystationdatatoday.WeatherImgUrl = stationtoday.symbol.imageUrl;
                            mystationdatatoday.Maxtemp = stationtoday.max;
                            mystationdatatoday.MinTemp = stationtoday.min;
                            
                            myweather.Stationdata.Add(mystationdatatoday);

                            i++;
                        }
                    }
                }

                //Tomorrow info
                if (siagweather.tomorrow != null)
                {                    
                    Conditions myconditiontomorrow = new Conditions();

                    myconditiontomorrow.date = Convert.ToDateTime(siagweather.tomorrow.date.ToShortDateString() + " " + siagweather.tomorrow.hour); //TODO CHeck
                    myconditiontomorrow.WeatherCondition = siagweather.tomorrow.conditions;
                    myconditiontomorrow.WeatherImgurl = siagweather.tomorrow.imageUrl;
                    myconditiontomorrow.Weatherdesc = siagweather.tomorrow.weather;
                    myconditiontomorrow.Title = siagweather.tomorrow.title;
                    myconditiontomorrow.Temperatures = siagweather.tomorrow.temperatures;

                    myconditiontomorrow.bulletinStatus = siagweather.tomorrow.bulletinStatus;
                    myconditiontomorrow.Reliability = siagweather.tomorrow.reliability.ToString();
                    myconditiontomorrow.TempMaxmax = siagweather.tomorrow.tMaxMax;
                    myconditiontomorrow.TempMaxmin = siagweather.tomorrow.tMaxMin;
                    myconditiontomorrow.TempMinmax = siagweather.tomorrow.tMinMax;
                    myconditiontomorrow.TempMinmin = siagweather.tomorrow.tMinMin;

                    myweather.Conditions.Add(myconditiontomorrow);                 
                }

                //Stationdata today
                if (siagweather.tomorrow != null)
                {                    
                    var stationstomorrow = siagweather.tomorrow.stationData;
                    
                    if (stationstomorrow != null)
                    {
                        int j = 1;

                        foreach (var stationtomorrow in stationstomorrow)
                        {
                            Stationdata mystationdatatomorrow = new Stationdata();

                            mystationdatatomorrow.date = Convert.ToDateTime(siagweather.tomorrow.date.ToShortDateString() + " " + siagweather.tomorrow.hour);

                            mystationdatatomorrow.Id = j;
                            var mystationdatacity = weatherdataxml.Root.Elements("Station").Where(x => x.Attribute("Id").Value.Equals(j)).FirstOrDefault();
                            mystationdatatomorrow.CityName = mystationdatacity.Attribute("Name" + lang.ToUpper()).Value;
                            //mystationdatatomorrow.stationdatacityrid = mystationdatacity.Attribute("RID").Value;

                            if (lang == "en")
                            {
                                if (mystationdatatomorrow.CityName.Contains("/"))
                                {
                                    mystationdatatomorrow.CityName = mystationdatatomorrow.CityName.Replace("/", " / ");
                                }
                            }

                            mystationdatatomorrow.WeatherCode = stationtomorrow.symbol.code;
                            mystationdatatomorrow.WeatherDesc = stationtomorrow.symbol.description;
                            mystationdatatomorrow.WeatherImgUrl = stationtomorrow.symbol.imageUrl;

                            mystationdatatomorrow.Maxtemp = stationtomorrow.max;
                            mystationdatatomorrow.MinTemp = stationtomorrow.min;

                            myweather.Stationdata.Add(mystationdatatomorrow);

                            j++;
                        }
                    }                    
                }

                return myweather;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
