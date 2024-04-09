using APBD_05.model;
using APBD_05.model.animal;

namespace APBD_05.repository;

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