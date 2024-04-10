using APBD_05.model;
using APBD_05.model.animal;
using APBD_05.model.visit;

namespace APBD_05.repository;

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