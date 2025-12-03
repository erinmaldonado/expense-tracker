using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.DTO;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExpensesController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public ExpensesController(FinanceDbContext context)
    {
        _context = context;
    }

    // GET: api/Expenses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
    {
        return await _context.Expenses
            .Include(e => e.Category) // Fetches the Category data with the expense
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    // GET: api/Expenses/summary
    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<ExpenseSummary>>> GetDashboardSummary()
    {
        List<ExpenseSummary> summary = await _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Category != null)
            .GroupBy(e => e.Category.Name)
            .Select(g => new ExpenseSummary
            {
                Category = g.Key,
                TotalAmount = g.Sum(e => e.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync();

        return summary;
    }

    // GET: api/Expenses/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> GetExpense(int id)
    {
        Expense? expense = await _context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense == null)
        {
            return NotFound();
        }

        return expense;
    }

    // POST: api/Expenses
    [HttpPost]
    public async Task<ActionResult<Expense>> PostExpense(Expense expense)
    {
        // Note: The frontend should send "CategoryId"
        // Validating that the Category exists is good practice
        bool categoryExists = await _context.Categories.AnyAsync(c => c.Id == expense.CategoryId);
        if (!categoryExists)
        {
             return BadRequest("Invalid CategoryId");
        }
        
        // Prevent EF from trying to create a new Category if one is passed in partially
        expense.Category = null; 

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
    }

    // PUT: api/Expenses/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutExpense(int id, Expense expense)
    {
        if (id != expense.Id)
        {
            return BadRequest();
        }

        _context.Entry(expense).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ExpenseExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Expenses/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        Expense? expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ExpenseExists(int id)
    {
        return _context.Expenses.Any(e => e.Id == id);
    }
}