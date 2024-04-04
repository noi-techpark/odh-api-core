// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Combinatorics.Collections;
using DataModel;

namespace Helper
{
    public class RoomCalculationHelper
    {
        public static CheapestRoomCombination CalculateCheapestRooms(IEnumerable<CheapestOffer> cheapestofferlist, int rooms, string service)
        {
            if (cheapestofferlist != null && cheapestofferlist.Count() > 0 && cheapestofferlist.Count() >= rooms)
            {
                if (rooms == 1)
                {
                    var cheapestorffersingle = cheapestofferlist.OrderBy(x => x.Price).Take(1).FirstOrDefault();
                    CheapestRoomCombination cheapestroomcombinationresult = new CheapestRoomCombination();
                    if (cheapestorffersingle is { })
                        cheapestroomcombinationresult.CheapestRoomCombinationDetail = new List<CheapestOffer>() { cheapestorffersingle };
                    cheapestroomcombinationresult.Service = service;

                    return cheapestroomcombinationresult;
                }                
                else
                    return CalculateCheapestRoomCombinations(cheapestofferlist, rooms, service);
                //else
                //{
                //    //count if all romseqs are in the cheapestofferlist
                //    var romseqspresent = cheapestofferlist.Select(x => x.RoomSeq).Distinct().Count();

                //    if (romseqspresent == rooms)
                //        return CalculateCheapestRoomCombinationsTemp(cheapestofferlist, rooms, service);
                //    else
                //        return new CheapestRoomCombination() { Service = service, CheapestRoomCombinationDetail = new List<CheapestOffer>() };
                //}
            }
            else
            {
                return new CheapestRoomCombination() { Service = service, CheapestRoomCombinationDetail = new List<CheapestOffer>() };
            }
        }

        private static CheapestRoomCombination CalculateCheapestRoomCombinationsWithCombinations(IEnumerable<CheapestOffer> cheapestofferlist, int rooms, string service)
        {
            List<CheapestRoomCombination> mycombinationresult = new List<CheapestRoomCombination>();

            //Create combinations TO verify use variations??
            Combinations<CheapestOffer> combinations = new Combinations<CheapestOffer>(cheapestofferlist, rooms);

            foreach (IList<CheapestOffer> c in combinations)
            {
                bool addcombination = true;

                //Check with roomseqdict if the combination is valid
                for (int i = 0; i < rooms; i++)
                {
                    if (c.Where(x => x.RoomSeq == c[i].RoomSeq).Count() > 1)
                    {
                        //Removing combination - room in the same roomseq
                        addcombination = false;
                    }

                    if (!addcombination)
                        break;
                }

                //Remove all combinations where more rooms are used than roomfree                 
                if (addcombination)
                {
                    for (int i = 0; i < rooms; i++)
                    {
                        //Check how often room with this id is used
                        if (c.Where(x => x.RoomId == c[i].RoomId).Count() > c[i].RoomFree)
                        {
                            //Removing combination - room used more than available
                            addcombination = false;
                        }

                        if (!addcombination)
                            break;
                    }
                }

                if (addcombination)
                {
                    mycombinationresult.Add(new CheapestRoomCombination() { CheapestRoomCombinationDetail = c, Service = service });
                }
            }

            //Return Cheapestroomcombination
            var cheapestcombination = mycombinationresult.OrderBy(x => x.Price).Take(1).FirstOrDefault();

            return cheapestcombination ?? new();
        }


        //TO TEST
        private static CheapestRoomCombination CalculateCheapestRoomCombinations(IEnumerable<CheapestOffer> cheapestofferlist, int rooms, string service)
        {            
            //Create an Array with a Collection
            List<CheapestOffer>[] offerbyroomseq = new List<CheapestOffer>[rooms];

            CheapestRoomCombination cheapestroomcombination = new CheapestRoomCombination();
            cheapestroomcombination.Service = service;
            cheapestroomcombination.CheapestRoomCombinationDetail = new List<CheapestOffer>();

            for (int i = 1; i <= rooms; i++)
            {
                //To check could it be that a roomseq does not exist?
                offerbyroomseq[i - 1] = cheapestofferlist.Where(x => x.RoomSeq == i).OrderBy(x => x.Price).ToList();
            }

            //TO check, are there offerbyroomseq with not enough rooms?
            if (offerbyroomseq.Count() < rooms)
                throw new Exception("there is something not working");

            foreach (var roomseq in offerbyroomseq.OrderBy(x => x.Count))
            {
                foreach (var room in roomseq)
                {
                    //roomfre
                    int? roomfree = room.RoomFree;
                    //Count total rooms

                    //Count used rooms in cheapestroomcombination
                    var usedroomscount = 0;
                    if (cheapestroomcombination.CheapestRoomCombinationDetail.Count > 0)
                    {
                        usedroomscount = cheapestroomcombination.CheapestRoomCombinationDetail.Count(x => x.RoomId == room.RoomId);
                    }

                    //if roomfree > cheapestroomcombination
                    if (roomfree > usedroomscount)
                    {
                        cheapestroomcombination.CheapestRoomCombinationDetail.Add(room);
                        break;
                    }
                }            
            }

            //If there are cheapestroomcombinations with not enough rooms return null
            if (cheapestroomcombination.CheapestRoomCombinationDetail.Count < rooms)
                return new(); //throw new Exception("there is something not working");

            return cheapestroomcombination;
        }
    }

    public static class CombinationsGeneric
    {
        private static void InitIndexes(int[] indexes)
        {
            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = i;
            }
        }

        private static void SetIndexes(int[] indexes, int lastIndex, int count)
        {
            indexes[lastIndex]++;
            if (lastIndex > 0 && indexes[lastIndex] == count)
            {
                SetIndexes(indexes, lastIndex - 1, count - 1);
                indexes[lastIndex] = indexes[lastIndex - 1] + 1;
            }
        }

        private static List<T> TakeAt<T>(int[] indexes, IEnumerable<T> list)
        {
            List<T> selected = new List<T>();
            for (int i = 0; i < indexes.Length; i++)
            {
                selected.Add(list.ElementAt(indexes[i]));
            }
            return selected;
        }

        private static bool AllPlacesChecked(int[] indexes, int places)
        {
            for (int i = indexes.Length - 1; i >= 0; i--)
            {
                if (indexes[i] != places)
                    return false;
                places--;
            }
            return true;
        }

        public static IEnumerable<List<T>> GetDifferentCombinations<T>(this IEnumerable<T> collection, int count)
        {
            int[] indexes = new int[count];
            int listCount = collection.Count();
            if (count > listCount)
                throw new InvalidOperationException($"{nameof(count)} is greater than the collection elements.");
            InitIndexes(indexes);
            do
            {
                var selected = TakeAt(indexes, collection);
                yield return selected;
                SetIndexes(indexes, indexes.Length - 1, listCount);
            }
            while (!AllPlacesChecked(indexes, listCount));

        }
    }    
}
