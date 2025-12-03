using Microsoft.EntityFrameworkCore;
public class Category{
    public int Id { get; set; } 
    public string Name { get; set; } = null!;
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
