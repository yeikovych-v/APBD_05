using APBD_05.model;
using APBD_05.model.animal;

namespace APBD_05.repository;

public interface IAnimalDatabase
{
    void AddAll(List<Animal> animals);
    List<Animal> FindAll();
    Animal? FindById(long id);
    void Add(Animal animal);
    bool EditById(long id, Animal animal);
    bool DeleteById(long id);
}