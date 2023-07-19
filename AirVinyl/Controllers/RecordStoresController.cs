using System;
using System.Linq;
using System.Threading.Tasks;
using AirVinyl.API.DbContexts;
using AirVinyl.Helpers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace AirVinyl.Controllers
{
    [Route("odata")]
    public class RecordStoresController : ODataController
    {
        private readonly AirVinylDbContext _airVinylDbContext;

        public RecordStoresController(AirVinylDbContext airVinylDbContext)
        {
            this._airVinylDbContext = airVinylDbContext ?? throw new ArgumentNullException(nameof(airVinylDbContext));
        }

        [EnableQuery]
        [HttpGet("RecordStores")]
        public IActionResult GetAllRecordStores()
        {
            return Ok(_airVinylDbContext.RecordStores);
        }

        [EnableQuery]
        [HttpGet("RecordStores({key})")]
        public IActionResult GetOneRecordStore(int key)
        {
            var recordStores = _airVinylDbContext.RecordStores.Where(p => p.RecordStoreId == key);

            if (!recordStores.Any())
            {
                return NotFound();
            }

            return Ok(SingleResult.Create(recordStores));
        }

        [HttpGet("RecordStores({key})/Tags")]
        [EnableQuery]
        public IActionResult GetRecordStoreTagsProperty(int key)
        {
            // no Include necessary for EF Core - "Tags" isn't a navigation property
            // in the entity model
            var recordStore = _airVinylDbContext.RecordStores.FirstOrDefault(p => p.RecordStoreId == key);

            if (recordStore == null)
            {
                return NotFound();
            }

            var collectionPropertyToGet = new Uri(HttpContext.Request.GetEncodedUrl()).Segments.Last();
            var collectionPropertyValue = recordStore.GetValue(collectionPropertyToGet);
            
            // return the collection of tags
            return Ok(collectionPropertyValue);
        }

        // {key} is bugging in this version => use {id}
        [HttpGet("RecordStores({id})/AirVinyl.Functions.IsHighRated(minimumRating={minimumRating})")]
        public async Task<bool> IsHighRated(int id, int minimumRating)
        {
            // get the RecordStore
            var recordStore = await _airVinylDbContext.RecordStores.FirstOrDefaultAsync(p => p.RecordStoreId == id
                && p.Ratings.Any() && (p.Ratings.Sum(r => r.Value) / p.Ratings.Count) >= minimumRating);

            return (recordStore != null);
        }
    }
}