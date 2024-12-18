// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text;
using Newtonsoft.Json;

namespace OdhApiCore
{
    public static class Serialization
    {
        public static byte[] ToByteArray<T>(this T objectToSerialize)
            where T : notnull
        {
            return Encoding.Default.GetBytes(JsonConvert.SerializeObject(objectToSerialize));
        }

        public static T? FromByteArray<T>(this byte[] arrayToDeserialize)
            where T : class
        {
            if (arrayToDeserialize == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(arrayToDeserialize));
        }
    }
}
