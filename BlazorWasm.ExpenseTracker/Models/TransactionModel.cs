namespace BlazorWasm.ExpenseTracker.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string TransactionType { get; set; }
    }
}
