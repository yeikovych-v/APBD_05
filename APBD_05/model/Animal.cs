namespace APBD_05.model;

public class Animal
{
    public long Id { get; set; }
    public string Name { get; set; }
    public AnimalCategory AnimalCategory { get; set; }
    public decimal Weight { get; set; }
    public FurColor FurColor { get; set; }
}