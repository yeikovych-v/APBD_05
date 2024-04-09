using APBD_05.model;
using APBD_05.model.animal;
using APBD_05.model.visit;

namespace APBD_05.repository;

public interface IVisitDatabase
{
    void Add(Animal animal, Visit visit);
    List<Visit> FindByAnimal(Animal animal);
    void AddAll(Animal animal, List<Visit> visits);
}