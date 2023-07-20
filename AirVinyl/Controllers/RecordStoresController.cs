using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirVinyl.API.DbContexts;
using AirVinyl.Entities;
using AirVinyl.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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

        [HttpGet("RecordStores/AirVinyl.Functions.AreRatedBy(personIds={people})")]
        public async Task<IActionResult> AreRatedBy([FromODataUri] IEnumerable<int> people)
        {
            var recordStores = await _airVinylDbContext.RecordStores
                .Where(p => p.Ratings.Any(r => people.Contains(r.RatedBy.PersonId))).ToListAsync();

            return Ok(recordStores);
        }

        [HttpGet("GetHighRatedRecordStores(minimumRating={minimumRating})")]
        public async Task<IActionResult> GetHighRatedRecordStores(int minimumRating)
        {
            var recordStores = await _airVinylDbContext.RecordStores
                .Where(p => p.Ratings.Any() && (p.Ratings.Sum(r => r.Value) / p.Ratings.Count) >= minimumRating)
                .ToListAsync();
            return Ok(recordStores);
        }

        [HttpPost("RecordStores({id})/AirVinyl.Actions.Rate")]
        public async Task<IActionResult> Rate(int id, ODataActionParameters parameters)
        {
            // get the RecordStore
            var recordStore = await _airVinylDbContext.RecordStores.FirstOrDefaultAsync(p => p.RecordStoreId == id);

            if (recordStore == null)
            {
                return NotFound();
            }

            if (!parameters.TryGetValue("rating", out object outputFromDictionary))
            {
                return NotFound();
            }

            if (!int.TryParse(outputFromDictionary.ToString(), out int rating))
            {
                return NotFound();
            }

            if (!parameters.TryGetValue("personId", out outputFromDictionary))
            {
                return NotFound();
            }

            if (!int.TryParse(outputFromDictionary.ToString(), out int personId))
            {
                return NotFound();
            }

            // the person must exist
            var person = await _airVinylDbContext.People.FirstOrDefaultAsync(p => p.PersonId == personId);

            if (person == null)
            {
                return NotFound();
            }

            // everything checks out, add he rating
            recordStore.Ratings.Add(new Rating() { RatedBy = person, Value = rating });

            // save changes
            if (await _airVinylDbContext.SaveChangesAsync() > 0)
            {
                // return true
                return Ok(true);
            }
            else
            {
                // Something went wrong - we expect our
                // action to return false in that case.
                // The request s still successful
                // false is a valid response
                return Ok(false);
            }
        }

        /* Not working yet */
        [HttpPost("RecordStores/AirVinyl.Actions.RemoveRatings")]
        public async Task<IActionResult> RemoveRatings(ODataActionParameters parameters)
        {
            // from the param dictionary, get the personId
            if (!parameters.TryGetValue("personId", out object outputFromDictionary))
            {
                return BadRequest();
            }

            if (!int.TryParse(outputFromDictionary.ToString(), out int personId))
            {
                return BadRequest();
            }

            // get the RecordStores that were rated by the person with personId
            var recordStoresRatedByCurrentPerson =
                await _airVinylDbContext.RecordStores.Include("Ratings").Include("Ratings.RatedBy")
                    .Where(p => p.Ratings.Any(r => r.RatedBy.PersonId == personId)).ToListAsync();

            // remove those ratings
            foreach (var store in recordStoresRatedByCurrentPerson)
            {
                // get the ratings by the current person 
                var ratingsByCurrentPerson = store.Ratings
                    .Where(r => r.RatedBy.PersonId == personId).ToList();

                for (int i = 0; i < ratingsByCurrentPerson.Count(); i++)
                {
                    store.Ratings.Remove(ratingsByCurrentPerson[i]);
                }
            }

            // save changes
            if (await _airVinylDbContext.SaveChangesAsync() > 0)
            {
                // return true
                return Ok(true);
            }
            else
            {
                // Something went wrong - we expect our
                // action to return false in that case.
                // The request is still successful
                // false is a valid response
                return Ok(false);
            }
        }

        /* Unbound Action */
        [HttpPost("RemoveRecordStoreRatings")]
        public async Task<IActionResult> RemoveRecordStoreRatings(ODataActionParameters parameters)
        {
            // from the param dictionary, get the personid
            if (!parameters.TryGetValue("personId", out object outputFromDictionary))
            {
                return BadRequest();
            }

            if (!int.TryParse(outputFromDictionary.ToString(), out int personId))
            {
                return BadRequest();
            }

            // get the RecordStores that were rated by the person with personId
            var recordStoresRatedByCurrentPerson = await _airVinylDbContext.RecordStores.Include("Ratings")
                .Include("Ratings.RatedBy").Where(p => p.Ratings.Any(r => r.RatedBy.PersonId == personId))
                .ToListAsync();

            // remove those ratings
            foreach (var store in recordStoresRatedByCurrentPerson)
            {
                // get the ratings by the current person
                var ratingsByCurrentPerson = store.Ratings.Where(r => r.RatedBy.PersonId == personId).ToList();

                for (int i = 0; i < ratingsByCurrentPerson.Count(); i++)
                {
                    store.Ratings.Remove(ratingsByCurrentPerson[i]);
                }
            }

            // save changes
            if (await _airVinylDbContext.SaveChangesAsync() > 0)
            {
                return NoContent();
            }
            else
            {
                // something went wrong
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [EnableQuery]
        [HttpGet("RecordStores/AirVinyl.SpecializedRecordStore")]
        public IActionResult GetSpecializedRecordStores()
        {
            var specializedStores =
                _airVinylDbContext.RecordStores.Where(r => r is SpecializedRecordStore).ToList();
            return Ok(specializedStores.Select(s => s as SpecializedRecordStore));
        }

        [EnableQuery]
        [HttpGet("RecordStores({id})/AirVinyl.SpecializedRecordStore")]
        public IActionResult GetSpecializedRecordStore(int id)
        {
            var specializedStore =
                _airVinylDbContext.RecordStores.Where(r => r.RecordStoreId == id && r is SpecializedRecordStore);

            if (!specializedStore.Any())
            {
                return NotFound();
            }

            return Ok(SingleResult.Create(specializedStore.Select(s => s as SpecializedRecordStore)));
        }
    }
}