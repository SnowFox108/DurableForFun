namespace DaprDockerBackend.Models;
public class Basket
{
    public int Id { get; set; }
    public List<Fruit> Fruits { get; set; }

    public Basket()
    {
        Fruits = new List<Fruit>();
    }

}

