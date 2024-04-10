using APBD_05.model.animal;
using APBD_05.model.visit;
using APBD_05.repository;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.Results;

namespace APBD_05.controller;

[Route("api/visits")]
[ApiController]
public class VisitController(IVisitDatabase visitDb, IAnimalDatabase animalDb)
{
    [HttpGet]
    [Route("{animalId:int}")]
    public IResult GetVisit(long animalId)
    {
        var animalNullable = animalDb.FindById(animalId);
        if (animalNullable == null) return BadRequest("Unable to post visit for nonexistent animal.");

        var found = visitDb.FindByAnimal(animalNullable);
        return found.Count == 0 ? BadRequest($"Unable to find visits for animal: [{animalNullable}]") : Ok(found);
    }

    [HttpPost]
    [Route("{animalId:int}")]
    public IResult PostVisit(long animalId, Visit visit)
    {
        var animalNullable = animalDb.FindById(animalId);
        if (animalNullable == null) return BadRequest("Unable to post visit for nonexistent animal.");

        visit.Animal = animalNullable;
        
        visitDb.Add(animalNullable, visit);
        return Ok($"Successfully added visit [{visit}] for animal [{animalNullable}]");
    }
}