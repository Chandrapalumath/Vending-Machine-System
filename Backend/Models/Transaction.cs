namespace Backend.Model
{
    public class Transaction
    {
        public string UserName { get; set; } = string.Empty;
        public string[] Items { get; set; } = Array.Empty<string>();
        public float[] Prices { get; set; } = Array.Empty<float>();
        public int[] Quantities { get; set; } = Array.Empty<int>();
        public float TotalAmount { get; set; }
        public DateTime TimeUtc { get; set; }

        public Transaction(string userName, string[] items, float[] prices, int[] quantities, float totalAmount)
        {
            UserName = userName;
            Items = items;
            Prices = prices;
            Quantities = quantities;
            TotalAmount = totalAmount;
            TimeUtc = DateTime.UtcNow;
        }
    }
}
