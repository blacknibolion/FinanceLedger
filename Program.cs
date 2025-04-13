using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;


public class Program
{
// Removed the erroneous declaration of Main
static List<Transaction> transactions = new List<Transaction>();

static void Main()
{
    while (true)
    {
        Console.WriteLine("Welcome to the Finance Ledger!");
        Console.WriteLine("1. Add Transaction");
        Console.WriteLine("2. View Transactions");
        Console.WriteLine("3. View Summary");
        Console.WriteLine("4. Monthly Report");
        Console.WriteLine("5. Exit");
        Console.Write("Select an option: ");
        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1": AddTransaction(); break;
            case "2": ViewTransactions(); break;
            case "3": ViewSummary(); break;
            case "4": SummarybyMonth(); break;
            case "5": return;// Exit the program
            default: Console.WriteLine("Invalid choice."); break;
        }
    }
}
static void AddTransaction()
{
    // Create a new instance of LedgerDbContext
    //  to interact with the database
    // This assumes you have a DbContext class 
    // named LedgerDbContext
    using var db = new LedgerDbContext();

    DateTime date;
    string type;
    string category;
    string account;
    string customer;
    string description;
    decimal amount;

    try
    {
        Console.WriteLine("Enter Date (yyyy-mm-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out date))
        {
            Console.WriteLine("❌ Invalid date format.");
            return;
        }

        Console.WriteLine("Enter Type (Income/Expense): ");
        type = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(type) || 
            !(type.Equals("Income", StringComparison.OrdinalIgnoreCase) || 
              type.Equals("Expense", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("❌ Type must be either 'Income' or 'Expense'.");
            return;
        }

        Console.WriteLine("Enter Category: ");
        category = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine("❌ Category cannot be empty.");
            return;
        }

        Console.WriteLine("Enter Account: ");
        account = Console.ReadLine();

        var existingAccount = db.Transactions.FirstOrDefault(t => t.Account == account);
        if (existingAccount != null)
        {
            Console.WriteLine("✅ Account already exists." + existingAccount.Account);
            customer = existingAccount.Customer;
        }

        else
        {
              Console.WriteLine("Enter Customer Name: ");
              customer = Console.ReadLine();
        }

        Console.WriteLine("Enter Description: ");
        description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("❌ Description cannot be empty.");
            return;
        }

        Console.WriteLine("Enter Amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out amount))
        {
            Console.WriteLine("❌ Invalid amount input.");
            return;
        }

        // At this point, all inputs are valid — you can proceed to save the transaction.
        Console.WriteLine("\n✅ Transaction data captured successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ An error occurred: {ex.Message}");
        return;
    }

    var transactions = new Transaction
    {
        Date = date,
        Type = Enum.TryParse<TransactionType>(type, true, out var transactionType) ? transactionType : throw new ArgumentException("Invalid transaction type"),
        Category = category,
        Account = account,
        Customer = customer,
        Description = description,
        Amount = amount,
    };

    db.Transactions.Add(transactions);
    db.SaveChanges(); // Save changes to the database

    Console.WriteLine("✅Transaction added successfully!");
    Console.WriteLine("Press any key to continue...👉");
}static void ViewTransactions()
{
    using var db = new LedgerDbContext();
    var transactions = db.Transactions.ToList();
    if (transactions.Count == 0)
    {
        Console.WriteLine("No transactions found.");
        return;
    }
    Console.WriteLine("Transactions:");
    foreach (var transaction in transactions)
    {
        Console.WriteLine($"ID: {transaction.Id}, Date: {transaction.Date.ToShortDateString()}, | Type: {transaction.Type}, | Category: {transaction.Category}, | Account: {transaction.Account}, | Account Name: {transaction.Customer}, | Description: {transaction.Description}, | Amount: {transaction.Amount}");
    }
}
static void ViewSummary()
{
    using var db = new LedgerDbContext();

    var Income = db.Transactions
                    .Where(transaction => transaction.Type == TransactionType.Income)// Filter for Income transactions
                    .Sum(t => t.Amount);// Sum the Amount of Income transactions
    var Expense = db.Transactions
                    .Where(transaction => transaction.Type == TransactionType.Expense)// Filter for Expense transactions
                    .Sum(t => t.Amount);// Sum the Amount of Expense transactions
    var Total = Income - Expense;
    Console.WriteLine("Summary:");
    Console.WriteLine($"💰 Income: {Income:C}");
    Console.WriteLine($"💸 Expense: {Expense:C}");
    Console.WriteLine($"🧾 Net Total: {Total:C}");
}
static void SummarybyMonth()
{
    using var db = new LedgerDbContext();

    var summary = db.Transactions
                    .GroupBy(t => new{t.Date.Year, t.Date.Month})
                    .Select(g => new
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                        Expense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                        NetTotal =g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) 
                        - g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                    })
                    .ToList();
                    Console.WriteLine("\n📅 Total by Month:");
    foreach (var item in summary)
    {
        Console.WriteLine($"{item.Month}: {item.NetTotal:C}");
    }
}
}

