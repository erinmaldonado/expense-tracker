using Microsoft.EntityFrameworkCore;


public partial class Expense
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public decimal Amount { get; set; }
    public string Description { get; set; } = null!; 
    public string? Subcategory { get; set; } 

    public int CategoryId { get; set; }

    public virtual required Category Category { get; set; } 

}