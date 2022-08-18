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
        public static CheapestRoomCombination CalculateCheapestRooms(
            IEnumerable<CheapestOffer> cheapestofferlist,
            int rooms,
            string service
        )
        {
            if (
                cheapestofferlist != null
                && cheapestofferlist.Count() > 0
                && cheapestofferlist.Count() >= rooms
            )
            {
                if (rooms == 1)
                {
                    var cheapestorffersingle = cheapestofferlist
                        .OrderBy(x => x.Price)
                        .Take(1)
                        .FirstOrDefault();
                    CheapestRoomCombination cheapestroomcombinationresult =
                        new CheapestRoomCombination();
                    if (cheapestorffersingle is { })
                        cheapestroomcombinationresult.CheapestRoomCombinationDetail =
                            new List<CheapestOffer>() { cheapestorffersingle };
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
                return new CheapestRoomCombination()
                {
                    Service = service,
                    CheapestRoomCombinationDetail = new List<CheapestOffer>()
                };
            }
        }

        private static CheapestRoomCombination CalculateCheapestRoomCombinations(
            IEnumerable<CheapestOffer> cheapestofferlist,
            int rooms,
            string service
        )
        {
            List<CheapestRoomCombination> mycombinationresult = new List<CheapestRoomCombination>();

            //Create combinations TO verify use variations??
            Combinations<CheapestOffer> combinations = new Combinations<CheapestOffer>(
                cheapestofferlist,
                rooms
            );

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
                    mycombinationresult.Add(
                        new CheapestRoomCombination()
                        {
                            CheapestRoomCombinationDetail = c,
                            Service = service
                        }
                    );
                }
            }

            //Return Cheapestroomcombination
            var cheapestcombination = mycombinationresult
                .OrderBy(x => x.Price)
                .Take(1)
                .FirstOrDefault();

            return cheapestcombination ?? new();
        }

        //Hack because of CPU always over 90%
        private static CheapestRoomCombination CalculateCheapestRoomCombinationsTemp(
            IEnumerable<CheapestOffer> cheapestofferlist,
            int rooms,
            string service
        )
        {
            CheapestRoomCombination cheapestroomcombinationresult = new CheapestRoomCombination();
            cheapestroomcombinationresult.Service = service;
            cheapestroomcombinationresult.CheapestRoomCombinationDetail = new List<CheapestOffer>()
            {
                };

            //Get always the cheapest
            for (int i = 1; i <= rooms; i++)
            {
                var cheapestorffersingle = cheapestofferlist
                    .Where(x => x.RoomSeq == i)
                    .OrderBy(x => x.Price)
                    .Take(1)
                    .FirstOrDefault();

                if (cheapestorffersingle != null)
                    cheapestroomcombinationresult.CheapestRoomCombinationDetail.Add(
                        cheapestorffersingle
                    );
            }

            return cheapestroomcombinationresult;
        }
    }
}
