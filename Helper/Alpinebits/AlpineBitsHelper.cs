// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace Helper
{
    public class AlpineBitsHelper
    {
        public static int GetRoomClassificationCode(string roomtype)
        {
            switch (roomtype)
            {
                case "room":
                    return 42;
                case "apartment":
                    return 13;
                case "pitch":
                    return 5;
                case "dorm":
                    return 42;
                default:
                    return 42;
            }
        }

        public static List<int> GetRoomTypeAB(string roomtype)
        {
            switch (roomtype)
            {
                case "room":
                    return new List<int> { 1 };
                case "apartment":
                    return new List<int> { 2, 3, 4, 5 };
                case "pitch":
                    return new List<int> { 6, 7, 8 };
                case "dorm":
                    return new List<int> { 9 };
                default:
                    return new List<int> { 1 };
            }
        }

    }
}
