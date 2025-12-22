namespace DataLayer.Models
{
    public class Transaction
    {
        public string UserName { get; set; } = string.Empty;
        public string ItemsCsv { get; set; } = string.Empty;
        public string PricesCsv { get; set; } = string.Empty;
        public string QuantitiesCsv { get; set; } = string.Empty;
        public float TotalAmount { get; set; }
        public DateTime TimeUtc { get; set; }
    }
}
