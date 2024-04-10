using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.Results;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddControllers().AddXmlSerializerFormatters();
        builder.Services.AddScoped<IAnimalDatabase, RuntimeAnimalDb>();
        builder.Services.AddScoped<IVisitDatabase, RuntimeVisitDb>();
        builder.Services.AddTransient<MockDbPopulationService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        using var scope = app.Services.CreateScope();

        var mockDbPopulationService = scope.ServiceProvider.GetRequiredService<MockDbPopulationService>();

        mockDbPopulationService.MockAnimalRecords();
        mockDbPopulationService.MockAnimalVisits();

        app.MapControllers();

        app.Run();
    }
}

public class MockDbPopulationService(IAnimalDatabase animalDb, IVisitDatabase visitDb)
{
    public void MockAnimalRecords()
    {
        animalDb.AddAll(CreateMockAnimalRecords());
    }

    public void MockAnimalVisits()
    {
        var animals = animalDb.FindAll(); 
        if (animals.FirstOrDefault() == null)
        {
            MockAnimalRecords();
        }
        
        visitDb.AddAll(animals.First(), CreateMockVisitRecords(animals.First()));
    }

    private List<Visit> CreateMockVisitRecords(Animal animal)
    {
        return
        [
            new Visit(DateTime.Now, animal, "A regular check Up", 99.99m),
            new Visit(DateTime.Now.AddMonths(1), animal, "A second regular check Up", 124.99m),
            new Visit(DateTime.Now.AddMonths(2), animal, "A third regular check Up", 149.99m),
            new Visit(DateTime.Now.AddMonths(3), animal, "The last regular check Up", 199.99m)
        ];
    }

    private List<Animal> CreateMockAnimalRecords()
    {
        return
        [
            new Animal(1, "A Cat", AnimalCategory.Cat, 6.2m, FurColor.White),
            new Animal(2, "A Dog", AnimalCategory.Dog, 30.3m, FurColor.Mixed),
            new Animal(3, "A Dragon", AnimalCategory.Dragon, 5239.8m, FurColor.NoFur),
            new Animal(4, "Chessy", AnimalCategory.Cat, 8.9m, FurColor.Black),
            new Animal(5, "Lilith", AnimalCategory.Cat, 7.8m, FurColor.Black)
        ];
    }
}

public interface IAnimalDatabase
{
    void AddAll(List<Animal> animals);
    List<Animal> FindAll();
    Animal? FindById(long id);
    void Add(Animal animal);
    bool EditById(long id, Animal animal);
    bool DeleteById(long id);
}
public class RuntimeAnimalDb : IAnimalDatabase
{
    private static readonly List<Animal> Animals = new();
    
    public void AddAll(List<Animal> animals)
    {
        Animals.AddRange(animals);
    }

    public List<Animal> FindAll()
    {
        return [..Animals];
    }

    public Animal? FindById(long id)
    {
        return Animals.FirstOrDefault(animal => animal.Id == id);
    }

    public void Add(Animal animal)
    {
        Animals.Add(animal);
    }

    public bool EditById(long id, Animal animal)
    {
        var animalNullable = FindById(id);
        if (animalNullable == null) return false;

        animalNullable.Name = animal.Name;
        animalNullable.AnimalCategory = animal.AnimalCategory;
        animalNullable.Weight = animal.Weight;
        animalNullable.FurColor = animal.FurColor;
        
        return true;
    }

    public bool DeleteById(long id)
    {
        var animalNullable = FindById(id);
        if (animalNullable == null) return false;

        Animals.Remove(animalNullable);
        
        return true;
    }
}

public interface IVisitDatabase
{
    void Add(Animal animal, Visit visit);
    List<Visit> FindByAnimal(Animal animal);
    void AddAll(Animal animal, List<Visit> visits);
}

public class RuntimeVisitDb : IVisitDatabase
{

    private static readonly Dictionary<Animal, List<Visit>> AnimalVisits = new();

    public void Add(Animal animal, Visit visit)
    {
        if (!AnimalVisits.ContainsKey(animal))
        {
            AnimalVisits.Add(animal, []);
        }

        AnimalVisits[animal].Add(visit);
    }

    public List<Visit> FindByAnimal(Animal animal)
    {
        return AnimalVisits.FirstOrDefault(pair => pair.Key == animal).Value ?? [];
    }

    public void AddAll(Animal animal, List<Visit> visits)
    {
        visits.ForEach(visit => Add(animal, visit));
    }
}

public class Visit(DateTime dateVisit, Animal animal, string description, decimal price)
{
    public DateTime DateVisit { get; set; } = dateVisit;
    public Animal Animal { get; set; } = animal;
    public string Description { get; set; } = description;
    public decimal Price { get; set; } = price;
    
    public override string ToString()
    {
        return $"Animal: [dateVisit={DateVisit}, animal={Animal}, description={Description}, price={Price}]";
    }
}

public enum FurColor
{
    Red,
    Black,
    White,
    Mixed,
    NoFur
}

public enum AnimalCategory
{
    Cat,
    Dog,
    Dragon
}

public class Animal(long id, string name, AnimalCategory animalCategory, decimal weight, FurColor furColor)
{
    public long Id { get; set; } = id;
    public string Name { get; set; } = name;
    public AnimalCategory AnimalCategory { get; set; } = animalCategory;
    public decimal Weight { get; set; } = weight;
    public FurColor FurColor { get; set; } = furColor;

    public override string ToString()
    {
        return $"Animal: [id={Id}, name={Name}, category={AnimalCategory}, weight={Weight}, furColor={FurColor}]";
    }
}

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