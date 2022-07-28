namespace DaprDockerTester.Models;
public class Basket
{
    public int Id { get; set; }
    public List<Fruit> Fruits { get; set; }

    public Basket(int id)
    {
        Id = id;
        Fruits = new List<Fruit>()
        {
            new Fruit() {Id = 1, Name = "Apple"},
            new Fruit() {Id = 2, Name = "Banana"},
            new Fruit() {Id = 3, Name = "Coconut"}
        };
    }
}

