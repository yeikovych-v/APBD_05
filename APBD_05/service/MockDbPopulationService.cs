using APBD_05.model;
using APBD_05.model.animal;
using APBD_05.model.visit;
using APBD_05.repository;

namespace APBD_05.service;

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

    // public Task StartAsync(CancellationToken cancellationToken)
    // {
    //     using var scope = serviceProvider.CreateScope();
    //     var mockDbPopulationService = scope.ServiceProvider.GetRequiredService<MockDbPopulationService>();
    //         
    //     mockDbPopulationService.MockAnimalRecords();
    //     mockDbPopulationService.MockAnimalVisits();
    //     
    //     return Task.CompletedTask;
    // }
    //
    // public Task StopAsync(CancellationToken cancellationToken)
    // {
    //     return Task.CompletedTask;
    // }
}