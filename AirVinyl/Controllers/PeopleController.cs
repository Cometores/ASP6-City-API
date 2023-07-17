using AirVinyl.API.DbContexts;
using AirVinyl.Helpers;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirVinyl.Entities;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AirVinyl.Controllers
{
    /* Convention based routing for this controller */
    public class PeopleController : ODataController
    {
        private readonly AirVinylDbContext _airVinylDbContext;

        public PeopleController(AirVinylDbContext airVinylDbContext)
        {
            _airVinylDbContext = airVinylDbContext
                ?? throw new ArgumentNullException(nameof(airVinylDbContext));
        }

        /* .NET 5 not supporting [EnableQuery] for async method*/
        // [EnableQuery]
        // public async Task<IActionResult> Get()
        // {
        //     return Ok(await _airVinylDbContext.People.ToListAsync());
        // }
        
        [EnableQuery(MaxExpansionDepth = 3, MaxSkip = 10, MaxTop = 5, PageSize = 4)]
        public IActionResult Get()
        {
            return Ok(_airVinylDbContext.People);
        }

        // People(1)
        [EnableQuery]
        public IActionResult Get(int key)
        {
            var people = _airVinylDbContext.People
                .Where(p => p.PersonId == key);

            if (!people.Any())
            {
                return NotFound();
            }

            return Ok(SingleResult.Create(people));
        }

        [HttpGet("odata/People({key})/Email")]
        [HttpGet("odata/People({key})/FirstName")]
        [HttpGet("odata/People({key})/LastName")]
        [HttpGet("odata/People({key})/DateOfBirth")]
        [HttpGet("odata/People({key})/Gender")]
        public async Task<IActionResult> GetPersonProperty(int key)
        {
            var person = await _airVinylDbContext.People
                .FirstOrDefaultAsync(p => p.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            var propertyToGet = new Uri(HttpContext.Request.GetEncodedUrl()).Segments.Last();

            if (!person.HasProperty(propertyToGet))
            {
                return NotFound();
            }

            var propertyValue = person.GetValue(propertyToGet);

            if (propertyValue == null)
            {
                // null = no content
                return NoContent();
            }

            return Ok(propertyValue);
        }


        [HttpGet("odata/People({key})/Email/$value")]
        [HttpGet("odata/People({key})/FirstName/$value")]
        [HttpGet("odata/People({key})/LastName/$value")]
        [HttpGet("odata/People({key})/DateOfBirth/$value")]
        [HttpGet("odata/People({key})/Gender/$value")]
        public async Task<IActionResult> GetPersonPropertyRawValue(int key)
        {
            var person = await _airVinylDbContext.People
              .FirstOrDefaultAsync(p => p.PersonId == key);

            if (person == null)
            {
                return NotFound();
            }

            var url = HttpContext.Request.GetEncodedUrl();
            var propertyToGet = new Uri(url).Segments[^2].TrimEnd('/');

            if (!person.HasProperty(propertyToGet))
            {
                return NotFound();
            }

            var propertyValue = person.GetValue(propertyToGet);

            if (propertyValue == null)
            {
                // null = no content
                return NoContent();
            }

            return Ok(propertyValue.ToString());
        }

        // odata/People(key)/VinylRecords
        // [EnableQuery]
        // [HttpGet("odata/People({key})/VinylRecords")]
        // public IActionResult GetPersonCollectionProperty(int key)
        // {
        //     var collectionPropertyToGet = new Uri(HttpContext.Request.GetEncodedUrl())
        //         .Segments.Last();
        //
        //     var person = _airVinylDbContext.People
        //           .Include(collectionPropertyToGet)
        //           .FirstOrDefaultAsync(p => p.PersonId == key);
        //
        //     if (person == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     if (!person.HasProperty(collectionPropertyToGet))
        //     {
        //         return NotFound();
        //     }
        //
        //     return Ok(person.GetValue(collectionPropertyToGet));
        // }

        [EnableQuery]
        [HttpGet("odata/People({key})/VinylRecords")]
        public IActionResult GetVinylRecordsForPerson(int key)
        {
            var person = _airVinylDbContext.People.FirstOrDefault(p => p.PersonId == key);
            if (person == null)
            {
                return NotFound();
            }

            return Ok(_airVinylDbContext.VinylRecords.Where(v => v.Person.PersonId == key));
        }

        [HttpPost("odata/People")]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // add the person to the People collection
            _airVinylDbContext.People.Add(person);
            await _airVinylDbContext.SaveChangesAsync();
            
            // return the created person
            return Created(person);
        }

        [HttpPut("odata/People({key})")]
        public async Task<IActionResult> UpdatePerson(int key, [FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentPerson = await _airVinylDbContext.People
                .FirstOrDefaultAsync(p => p.PersonId == key);

            if (currentPerson == null)
            {
                return NotFound();
            }
            
            // Alternative: if the person isn't found: Upsert. This must only be used
            // if the responsibility for creating the key isn't at server-level. In 
            // our case, we're using auto-increment fields, so this isn't allowed - 
            // code is for illustration
            // if(currentPerson == null)
            // {
            //     // the key from the URI is the key we should use
            //     person.PersonId = key;
            //     _airVinylDbContext.People.Add(person);
            //     await _airVinylDbContext.SaveChangesAsync();
            //     return Created(person);
            // }

            person.PersonId = currentPerson.PersonId;
            _airVinylDbContext.Entry(currentPerson).CurrentValues.SetValues(person);
            await _airVinylDbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("odata/People({key})")]
        public async Task<IActionResult> PartiallyUpdatePerson(int key, [FromBody] Delta<Person> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentPerson = await _airVinylDbContext.People.FirstOrDefaultAsync(p => p.PersonId == key);

            if (currentPerson == null)
            {
                return NotFound();
            }
            
            patch.Patch(currentPerson);
            await _airVinylDbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("odata/People({key})")]
        public async Task<IActionResult> DeleteOnePerson(int key)
        {
            var currentPerson = await _airVinylDbContext.People.FirstOrDefaultAsync(p => p.PersonId == key);

            if (currentPerson == null)
            {
                return NotFound();
            }

            _airVinylDbContext.People.Remove(currentPerson);
            await _airVinylDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
