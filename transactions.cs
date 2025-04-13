public enum TransactionType
{
    Income = 1,
    Expense = 2
}
public class Transaction
{

    public int Id { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; } // income or Expense
    public string? Category { get; set; }
    public string? Account { get; set; }
    public string? Customer { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    

}
