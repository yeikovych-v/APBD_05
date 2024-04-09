using APBD_05.model.animal;

namespace APBD_05.model.visit;

public class Visit(DateTime dateVisit, Animal animal, string description, decimal price)
{
    public DateTime DateVisit { get; set; } = dateVisit;
    public Animal Animal { get; set; } = animal;
    public string Description { get; set; } = description;
    public decimal Price { get; set; } = price;
}