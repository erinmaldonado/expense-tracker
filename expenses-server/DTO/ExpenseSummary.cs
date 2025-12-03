namespace Backend.DTO // Use your actual namespace
{
    public class ExpenseSummary
    {
        public required string Category { get; set; }
        public decimal TotalAmount { get; set; }
    }
}