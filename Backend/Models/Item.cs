namespace Backend.Model
{
    public class Item
    {
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public int Quantity { get; set; }

        public Item(string name, float price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }
}
