// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Text;

namespace MSS
{
    public class Room
    {
        private string roomSeq;
        public string RoomSeq
        {
            get { return roomSeq; }
            set { roomSeq = value; }
        }

        private string roomType;
        public string RoomType
        {
            get { return roomType; }
            set { roomType = value; }
        }

        private List<string> person;
        public List<string> Person
        {
            get { return person; }
            set { person = value; }
        }
    }   
}
