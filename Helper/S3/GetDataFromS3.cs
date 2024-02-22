// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Helper.S3
{
    public class GetDataFromS3
    {
        public static async Task GetFileFromS3(string bucketname, string accesskey, string secretkey, string filename, string directorytosave)
        {
            TransferUtility fileTransferUtility =
                new TransferUtility(
                    new AmazonS3Client(accesskey, secretkey, Amazon.RegionEndpoint.EUWest1));

            var request = new TransferUtilityDownloadRequest()
            {
                BucketName = bucketname,
                Key = filename,
                FilePath = directorytosave + filename
            };

            await fileTransferUtility.DownloadAsync(request);
        }
    }
}
