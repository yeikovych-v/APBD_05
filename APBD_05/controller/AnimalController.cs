using APBD_05.repository;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.Results;

namespace APBD_05.controller;

[Route("/animals")]
[ApiController]
public class AnimalController(IAnimalDatabase animalDb)
{
    [HttpGet]
    public IResult GetAnimals()
    {
        return Ok(animalDb.FindAll());
    }
    
    
}