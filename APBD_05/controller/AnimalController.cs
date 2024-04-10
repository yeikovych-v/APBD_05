using APBD_05.model.animal;
using APBD_05.repository;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.Results;

namespace APBD_05.controller;

[Route("api/animals")]
[ApiController]
public class AnimalController(IAnimalDatabase animalDb)
{
    [HttpGet]
    public IResult GetAllAnimals()
    {
        return Ok(animalDb.FindAll());
    }
    
    [HttpGet]
    [Route("{id:long}")]
    public IResult GetAnimal(long id)
    {
        var found = animalDb.FindById(id);
        return found == null ? BadRequest($"Unable to find animal with given id: [{id}]") : Ok(animalDb.FindById(id));
    }

    [HttpPost]
    public IResult PostAnimal(Animal animal)
    {
        animalDb.Add(animal);
        return Ok($"Successfully added animal: {animal}");
    }
    
    [HttpPost]
    [Route("list")]
    public IResult PostAnimals(List<Animal> animals)
    {
        animalDb.AddAll(animals);
        return Ok($"Successfully added list of animals: {string.Join(", ", animals.Select(animal => animal.ToString()).ToList())}");
    }

    [HttpPut]
    [Route("{id:long}")]
    public IResult EditAnimal(long id, Animal animal)
    {
        var wasEdited = animalDb.EditById(id, animal);
        return wasEdited ? Ok($"Successfully edited animal: [{animal}]") : BadRequest($"Unable to find animal with given id: [{id}]");
    }

    [HttpDelete]
    [Route("{id:long}")]
    public IResult DeleteAnimal(long id)
    {
        var wasDeleted = animalDb.DeleteById(id);
        return wasDeleted ? Ok($"Successfully deleted animal with id: [{id}]") : BadRequest($"Unable to find animal with given id: [{id}]");
    }
}