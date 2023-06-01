// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SqlKata.Execution;

namespace OdhApiCore.Controllers.alpinebits
{
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class AlpineBitsController : OdhController
    {
        private readonly ISettings settings;

        public AlpineBitsController(IWebHostEnvironment env, ISettings settings, ILogger<AlpineBitsController> logger, QueryFactory queryFactory)
            : base(env, settings, logger, queryFactory)
        {
            this.settings = settings;
        }

        #region SwaggerExposed API

        #endregion

        #region InventoryBasicPush

        /// <summary>
        /// GET All InventoryBasic Objects from User
        /// </summary>        
        /// <returns>AlpineBits InventoryBasicObject</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataReader,AlpineBitsReader,AlpineBitsInventoryBasicReader")]
        [HttpGet, Route("AlpineBits/InventoryBasic")]
        public async Task<IActionResult> Get(
            string? accoid = null, 
            bool last = true)
        {
            if (String.IsNullOrEmpty(accoid))
                return await GetAllAlpineBitsMessagesBySource(this.User.Identity?.Name, "InventoryBasicPush");
            else
                return await GetAlpineBitsMessagesByIdandSource(null, this.User.Identity?.Name, "InventoryBasicPush", accoid, last);
        }

        /// <summary>
        /// GET Single InventoryBasic Object
        /// </summary>
        /// <param name="RequestId">Request ID</param>
        /// <returns>InventoryBasic Object</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataReader,AlpineBitsReader,AlpineBitsInventoryBasicReader")]
        [HttpGet, Route("AlpineBits/InventoryBasic/{RequestId}")]
        public async Task<IActionResult> Get(string RequestId)
        {
            return await GetAlpineBitsMessagesByIdandSource(RequestId, this.User.Identity?.Name, "InventoryBasicPush", null, false);
        }

        /// <summary>
        /// POST AlpineBits Inventory Basic
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,AlpineBitsWriter,AlpineBitsInventoryBasicWriter")]
        [HttpPost, Route("AlpineBits/InventoryBasic")]
        public async Task<IActionResult> PostInventoryBasicData()
        {
            return await PostInventoryBasic(Request);
        }

        #endregion

        #region InventoryHotelInfoPush

        /// <summary>
        /// GET All InventoryHotelInfo Objects from User
        /// </summary>        
        /// <returns>AlpineBits InventoryHotelInfoObject</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataReader,AlpineBitsReader,AlpineBitsInventoryHotelInfoReader")]
        [HttpGet, Route("AlpineBits/InventoryHotelInfo")]
        public async Task<IActionResult> GetInventoryHotelInfo(
            string? accoid = null, 
            bool last = true)
        {
            if (String.IsNullOrEmpty(accoid))
                return await GetAllAlpineBitsMessagesBySource(this.User.Identity?.Name, "InventoryHotelInfoPush");
            else
                return await GetAlpineBitsMessagesByIdandSource(null, this.User.Identity?.Name, "InventoryHotelInfoPush", accoid, last);

        }

        /// <summary>
        /// GET Single InventoryHotelInfo Object
        /// </summary>
        /// <param name="RequestId">RequestId</param>
        /// <returns>InventoryHotelInfo Object</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataReader,AlpineBitsReader,AlpineBitsInventoryHotelInfoReader")]
        [HttpGet, Route("AlpineBits/InventoryHotelInfo/{RequestId}")]
        public async Task<IActionResult> GetInventoryHotelInfoSingle(string RequestId)
        {
            return await GetAlpineBitsMessagesByIdandSource(RequestId, this.User.Identity?.Name, "InventoryHotelInfoPush", null, false);
        }

        /// <summary>
        /// POST AlpineBits Inventory HotelInfo
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,AlpineBitsWriter,AlpineBitsInventoryHotelInfoWriter")]
        [HttpPost, Route("AlpineBits/InventoryHotelInfo")]
        public async Task<IActionResult> PostInventoryHotelInfoData()
        {
            return await PostInventoryHotelInfo(Request);
        }

        #endregion

        #region FreeRoomsPush

        /// <summary>
        /// GET All FreeRooms Objects from User
        /// </summary>        
        /// <returns>AlpineBits FreeRoomsObject</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataReader,AlpineBitsReader,AlpineBitsFreeRoomsReader")]
        [HttpGet, Route("AlpineBits/FreeRooms")]
        public async Task<IActionResult> GetFreeRooms(
            string? accoid = null, 
            bool last = true)
        {
            if (String.IsNullOrEmpty(accoid))
                return await GetAllAlpineBitsMessagesBySource(this.User.Identity?.Name, "FreeRoomsPush");
            else
                return await GetAlpineBitsMessagesByIdandSource(null, this.User.Identity?.Name, "FreeRoomsPush", accoid, last);

        }

        /// <summary>
        /// GET Single FreeRooms Object
        /// </summary>
        /// <param name="RequestId">RequestId</param>
        /// <returns>FreeRooms Object</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataReader,AlpineBitsReader,AlpineBitsFreeRoomsReader")]
        [HttpGet, Route("AlpineBits/FreeRooms/{RequestId}")]
        public async Task<IActionResult> GetFreeRoomsSingle(string RequestId)
        {
            return await GetAlpineBitsMessagesByIdandSource(RequestId, this.User.Identity?.Name, "FreeRoomsPush", null, false);
        }

        /// <summary>
        /// POST AlpineBits FreeRooms
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,AlpineBitsWriter,AlpineBitsFreeRoomsWriter")]
        [HttpPost, Route("AlpineBits/FreeRooms")]
        public async Task<IActionResult> PostFreeRoomsData()
        {
            return await PostFreeRooms(Request); ;
        }

        #endregion

        #region Helpers

        private async Task<IActionResult> GetAllAlpineBitsMessagesBySource(string? source, string? messagetype, string? id = null, string? accoids = null)
        {
            try
            {
                var idlist = CommonListCreator.CreateIdList(id);
                var sourcelist = CommonListCreator.CreateIdList(source);
                var messagetypelist = CommonListCreator.CreateIdList(messagetype);
                var accoidlist = CommonListCreator.CreateIdList(accoids);

                var query =
                 QueryFactory.Query()
                     .SelectRaw("data")
                     .From("alpinebits")
                     .AlpineBitsWhereExpression(idlist, sourcelist, accoidlist, messagetypelist, null)
                     //.OrderByRaw("TO_TIMESTAMP(data ->> 'RequestDate','YYYY-MM-DD T HH24:MI:SS') DESC");
                     .OrderByDesc("gen_requestdate");


                var data = await query.GetAsync<JsonRaw?>();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericResult() { Message = ex.Message });
            }
        }

        private async Task<IActionResult> GetAlpineBitsMessagesByIdandSource(string? id, string? source, string? messagetype, string? accoids, bool last)
        {
            try
            {
                var idlist = CommonListCreator.CreateIdList(id);
                var sourcelist = CommonListCreator.CreateIdList(source);
                var messagetypelist = CommonListCreator.CreateIdList(messagetype);
                var accoidlist = CommonListCreator.CreateIdList(accoids);

                int limit = 0;
                if (last)
                    limit = 1;

                var query =
                 QueryFactory.Query()
                     .SelectRaw("data")
                     .From("alpinebits")
                     .AlpineBitsWhereExpression(idlist, sourcelist, accoidlist, messagetypelist, null)
                     //.OrderByRaw("TO_TIMESTAMP(data ->> 'RequestDate','YYYY-MM-DD T HH24:MI:SS') DESC")
                     .OrderByDesc("gen_requestdate")
                     .Limit(limit);

                var data = await query.GetAsync<JsonRaw?>();

                return Ok(data);                
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericResult() { Message = ex.Message });
            }
        }

        #endregion

        #region POST Methods

        private static async Task<string> ReadStringDataManual(HttpRequest request)
        {
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task<IActionResult> PostInventoryBasic(HttpRequest request)
        {
            try
            {                
                string jsonContent = await ReadStringDataManual(request);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    dynamic input = JsonConvert.DeserializeObject(jsonContent) ?? throw new InvalidOperationException();

                    var id = input.RequestId.Value;

                    input.MessageType = "InventoryBasicPush";
                    input.RequestDate = DateTime.Now;
                    input.Source = this.User.Identity?.Name?.ToLower();

                    var query = await QueryFactory.Query("alpinebits").InsertAsync(new JsonBData() { id = id, data = new JsonRaw(input) });
                  
                    return Ok(new GenericResult() { Message = "INSERT AlpineBits InventoryBasicPush succeeded, Request Id:" + id + " username:" + this.User.Identity?.Name });                    
                }
                else
                    throw new Exception("no Content");
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericResult() { Message = ex.Message });
            }
        }

        private async Task<IActionResult> PostInventoryHotelInfo(HttpRequest request)
        {
            try
            {
                string jsonContent = await ReadStringDataManual(request);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    dynamic input = JsonConvert.DeserializeObject(jsonContent) ?? throw new InvalidOperationException();

                    var id = input.RequestId.Value;

                    input.MessageType = "InventoryHotelInfoPush";
                    input.RequestDate = DateTime.Now;
                    input.Source = this.User.Identity?.Name?.ToLower();

                    var query = await QueryFactory.Query("alpinebits").InsertAsync(new JsonBData() { id = id, data = new JsonRaw(input) });
                    //var query = QueryFactory.Query("alpinebits").AsInsert(new JsonBData() { id = id, data = JsonConvert.SerializeObject(input) });

                    return Ok(new GenericResult() { Message = "INSERT AlpineBits InventoryHotelInfoPush succeeded, Request Id:" + id + " username:" + this.User.Identity?.Name });
                }
                else
                    throw new Exception("no Content");                                              
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericResult() { Message = ex.Message });
            }
        }

        private async Task<IActionResult> PostFreeRooms(HttpRequest request)
        {
            try
            {
                string jsonContent = await ReadStringDataManual(request);

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    dynamic input = JsonConvert.DeserializeObject(jsonContent) ?? throw new InvalidOperationException();

                    var id = input.RequestId.Value;

                    input.MessageType = "FreeRoomsPush";
                    input.RequestDate = DateTime.Now;
                    input.Source = this.User.Identity?.Name?.ToLower();                    

                    var query = await QueryFactory.Query("alpinebits").InsertAsync(new JsonBData() { id = id, data = new JsonRaw(input) });

                    return Ok(new GenericResult() { Message = "INSERT AlpineBits FreeRoomsPush succeeded, Request Id:" + id + " username:" + this.User.Identity?.Name });
                }
                else
                    throw new Exception("no Content");
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericResult() { Message = ex.Message });
            }           
        }


        #endregion
    }
}
