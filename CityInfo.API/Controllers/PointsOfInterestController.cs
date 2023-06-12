using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        return Ok(city.PointOfInterest);
    }

    [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
            return NotFound();

        //find point of interest
        var pointOfInterest = city.PointOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterest == null)
            return NotFound();

        return Ok(pointOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        [FromRoute] int cityId,
        [FromBody] PointsOfInterestForCreationDto pointOfInterest)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null)
            return NotFound();

        // demo purposes - to be improved
        var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
            c => c.PointOfInterest).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };

        city.PointOfInterest.Add(finalPointOfInterest);

        return CreatedAtRoute(
            "GetPointOfInterest",
            new
            {
                cityId = cityId,
                pointOfInterestId = finalPointOfInterest.Id
            },
            finalPointOfInterest
        );
    }

    [HttpPut("{pointOfInterestid}")]
    public ActionResult UpdatePointOfInterest(
        [FromRoute] int cityId,
        [FromRoute] int pointOfInterestId,
        [FromBody] PointOfInterestForUpdateDto pointOfInterest)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
            return NotFound();

        var pointOfInterestFromStore = city.PointOfInterest
            .FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterestFromStore == null)
            return NotFound();

        pointOfInterestFromStore.Name = pointOfInterest.Name;
        pointOfInterestFromStore.Description = pointOfInterest.Description;

        return NoContent();
    }

    [HttpPatch("{pointofinterestid}")]
    public ActionResult PartiallyUpdatePointOfInterest(
        [FromRoute] int cityId,
        [FromRoute] int pointOfInterestId,
        [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
    )
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
            return NotFound();

        var pointOfInterestFromStore = city.PointOfInterest.
            FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterestFromStore == null)
            return NotFound();

        var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
        {
            Name = pointOfInterestFromStore.Name,
            Description = pointOfInterestFromStore.Description
        };
        
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!TryValidateModel(pointOfInterestToPatch))
            return BadRequest(ModelState);

        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

        return NoContent();
    }
}