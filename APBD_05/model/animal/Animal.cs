namespace APBD_05.model.animal;

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