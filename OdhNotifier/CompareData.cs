// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Helper;
using Helper.Generic;

namespace OdhNotifier
{
    //public class CompareData
    //{

    //    public static bool HasDataChanged<T>(T newdata, T olddata, List<string> propertytoskip) where T : new()
    //    {
    //        return EqualityHelper.CompareClasses<T>(olddata, newdata,  propertytoskip);
    //    }

    //    private static bool HasAccoChanged(Accommodation myacco, Accommodation oldacco, List<AccoRoom> newrooms, List<AccoRoom> oldrooms)
    //    {

    //        bool accoequal = false;
    //        bool ltsroomsequal = false;
    //        bool hgvroomsequal = false;

    //        //Compare Accommodation Object only if Changed send to
    //        accoequal = EqualityHelper.CompareClassesTest<Accommodation>(myacco, oldacco, new List<string>() { "LastChange" });

    //        if (accoequal)
    //        {
    //            if (oldrooms == null && newrooms == null)
    //            {
    //                ltsroomsequal = true;
    //                hgvroomsequal = true;
    //            }
    //            else if ((oldrooms != null && newrooms == null) || (oldrooms == null && newrooms != null))
    //            {
    //                ltsroomsequal = false;
    //                hgvroomsequal = false;
    //            }
    //            else
    //            {
    //                if (oldrooms.Count == newrooms.Count)
    //                {
    //                    //Compare LTS Rooms
    //                    var ltsoldrooms = oldrooms.Where(x => x.Source.ToLower() == "lts").ToList().OrderBy(x => x.Id);
    //                    var ltsnewrooms = newrooms.Where(x => x.Source.ToLower() == "lts").ToList().OrderBy(x => x.Id);

    //                    if (ltsoldrooms == null && ltsnewrooms == null)
    //                    {
    //                        ltsroomsequal = true;
    //                    }
    //                    else if ((ltsoldrooms != null && ltsnewrooms == null) || (ltsoldrooms == null && ltsnewrooms != null))
    //                    {
    //                        ltsroomsequal = false;
    //                    }
    //                    else
    //                    {
    //                        if (ltsoldrooms.Count() == ltsnewrooms.Count())
    //                        {
    //                            ltsroomsequal = true;

    //                            foreach (var ltsnewroom in ltsnewrooms)
    //                            {
    //                                var ltsoldroom = ltsoldrooms.Where(x => x.Id == ltsnewroom.Id).FirstOrDefault();

    //                                if (ltsoldroom != null)
    //                                {
    //                                    var roomresult = EqualityHelper.CompareClasses<AccoRoom>(ltsnewroom, ltsoldroom, new List<string>() { "LastChange" });

    //                                    if (!roomresult)
    //                                    {
    //                                        ltsroomsequal = false;
    //                                        break;
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }

    //                    //Compare HGV Rooms

    //                    var hgvoldrooms = oldrooms.Where(x => x.Source.ToLower() == "hgv").ToList().OrderBy(x => x.HGVId);
    //                    var hgvnewrooms = newrooms.Where(x => x.Source.ToLower() == "hgv").ToList().OrderBy(x => x.HGVId);

    //                    if (hgvoldrooms == null && hgvnewrooms == null)
    //                    {
    //                        hgvroomsequal = true;
    //                    }
    //                    else if ((hgvoldrooms != null && hgvnewrooms == null) || (hgvoldrooms == null && hgvnewrooms != null))
    //                    {
    //                        hgvroomsequal = false;
    //                    }
    //                    else
    //                    {
    //                        if (hgvoldrooms.Count() == hgvnewrooms.Count())
    //                        {
    //                            hgvroomsequal = true;

    //                            foreach (var hgvnewroom in hgvnewrooms)
    //                            {
    //                                var hgvoldroom = hgvoldrooms.Where(x => x.HGVId == hgvnewroom.HGVId).FirstOrDefault();

    //                                if (hgvoldroom != null)
    //                                {
    //                                    var hgvroomresult = EqualityHelper.CompareClassesTest<AccoRoom>(hgvnewroom, hgvoldroom, new List<string>() { "Id", "LastChange" });

    //                                    if (!hgvroomresult)
    //                                    {
    //                                        hgvroomsequal = false;
    //                                        break;
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }

    //                }
    //            }
    //        }


    //        if (!accoequal || !ltsroomsequal || !hgvroomsequal)
    //            return true;
    //        else
    //            return false;
    //    }

    //    private static bool HasAccoImageChanged(Accommodation myacco, Accommodation oldacco, List<AccoRoom> newrooms, List<AccoRoom> oldrooms)
    //    {

    //        bool accoimageoequal = false;
    //        bool ltsroomsimageequal = false;
    //        bool hgvroomsimageequal = false;

    //        //Compare Accommodation Object only if Changed send to
    //        accoimageoequal = EqualityHelper.CompareImageGallery(myacco.ImageGallery, oldacco.ImageGallery, null);

    //        if (accoimageoequal)
    //        {
    //            if (oldrooms == null && newrooms == null)
    //            {
    //                ltsroomsimageequal = true;
    //                hgvroomsimageequal = true;
    //            }
    //            else if ((oldrooms != null && newrooms == null) || (oldrooms == null && newrooms != null))
    //            {
    //                ltsroomsimageequal = false;
    //                hgvroomsimageequal = false;
    //            }
    //            else
    //            {
    //                if (oldrooms.Count == newrooms.Count)
    //                {
    //                    //Compare LTS Rooms
    //                    var ltsoldrooms = oldrooms.Where(x => x.Source.ToLower() == "lts").ToList().OrderBy(x => x.Id);
    //                    var ltsnewrooms = newrooms.Where(x => x.Source.ToLower() == "lts").ToList().OrderBy(x => x.Id);

    //                    if (ltsoldrooms == null && ltsnewrooms == null)
    //                    {
    //                        ltsroomsimageequal = true;
    //                    }
    //                    else if ((ltsoldrooms != null && ltsnewrooms == null) || (ltsoldrooms == null && ltsnewrooms != null))
    //                    {
    //                        ltsroomsimageequal = false;
    //                    }
    //                    else
    //                    {
    //                        if (ltsoldrooms.Count() == ltsnewrooms.Count())
    //                        {
    //                            ltsroomsimageequal = true;

    //                            foreach (var ltsnewroom in ltsnewrooms)
    //                            {
    //                                var ltsoldroom = ltsoldrooms.Where(x => x.Id == ltsnewroom.Id).FirstOrDefault();

    //                                if (ltsoldroom != null)
    //                                {
    //                                    var roomresult = EqualityHelper.CompareImageGallery(ltsnewroom.ImageGallery, ltsoldroom.ImageGallery, null);

    //                                    if (!roomresult)
    //                                    {
    //                                        ltsroomsimageequal = false;
    //                                        break;
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }

    //                    //Compare HGV Rooms

    //                    var hgvoldrooms = oldrooms.Where(x => x.Source.ToLower() == "hgv").ToList().OrderBy(x => x.HGVId);
    //                    var hgvnewrooms = newrooms.Where(x => x.Source.ToLower() == "hgv").ToList().OrderBy(x => x.HGVId);

    //                    if (hgvoldrooms == null && hgvnewrooms == null)
    //                    {
    //                        hgvroomsimageequal = true;
    //                    }
    //                    else if ((hgvoldrooms != null && hgvnewrooms == null) || (hgvoldrooms == null && hgvnewrooms != null))
    //                    {
    //                        hgvroomsimageequal = false;
    //                    }
    //                    else
    //                    {
    //                        if (hgvoldrooms.Count() == hgvnewrooms.Count())
    //                        {
    //                            hgvroomsimageequal = true;

    //                            foreach (var hgvnewroom in hgvnewrooms)
    //                            {
    //                                var hgvoldroom = hgvoldrooms.Where(x => x.HGVId == hgvnewroom.HGVId).FirstOrDefault();

    //                                if (hgvoldroom != null)
    //                                {
    //                                    var hgvroomresult = EqualityHelper.CompareImageGallery(hgvnewroom.ImageGallery, hgvoldroom.ImageGallery, null);

    //                                    if (!hgvroomresult)
    //                                    {
    //                                        hgvroomsimageequal = false;
    //                                        break;
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }

    //                }
    //            }
    //        }


    //        if (!accoimageoequal || !ltsroomsimageequal || !hgvroomsimageequal)
    //            return true;
    //        else
    //            return false;
    //    }

    //    public static bool HasImageChanged(ICollection<ImageGallery> imggallery, ICollection<ImageGallery> oldimggallery)
    //    {
    //        //Compare Accommodation Object only if Changed send to
    //        var result = EqualityHelper.CompareImageGallery(imggallery, oldimggallery, null);

    //        return !result;
    //    }

    //}

    //public class CompareMyObject<T> where T : new()
    //{
    //    T oldobject;

    //    public CompareMyObject(T objectocompare)
    //    {
    //        oldobject = CopyClassHelper.CloneJson<T>(objectocompare);
    //    }

    //    public bool Compare(T newobject, List<string> propertiestonotcheck)
    //    {
    //        return EqualityHelper.CompareClasses<T>(oldobject, newobject, propertiestonotcheck);
    //    }
    //}
}
