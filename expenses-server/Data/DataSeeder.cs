using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;   // Added for Path and Directory
using System.Collections.Generic; // Added for List
using System.Linq; // Added for .Any()

public static class DataSeeder
{
    public static void Seed(FinanceDbContext context)
    {
        // 1. Check if data already exists
        if (context.Expenses.Any()) 
        {
            return; // DB has been seeded
        }

        // 2. Path to your file
        // Ensure the "Data" folder name matches exactly where the CSV is
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Transactions.csv");

        // 3. Configure CSV Reader
        CsvConfiguration config = new CsvConfiguration(CultureInfo.GetCultureInfo("en-GB"))
        {
        HeaderValidated = null,
        MissingFieldFound = null
        };

        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, config))
        {
            IEnumerable<RecordsCSV> records = csv.GetRecords<RecordsCSV>();

            List<Expense> expensesList = new List<Expense>();

            // 1. Create a temporary list to track categories we have processed
            // This prevents us from creating "Food" 50 times if it appears 50 times.
            List<Category> knownCategories = context.Categories.ToList();

            foreach (var r in records)
            {
                // 2. Try to find if this category already exists in our list
                // Change '.Name' if your Category class uses a different property name!
                var categoryObj = knownCategories.FirstOrDefault(c => c.Name == r.Category);

                // 3. If it doesn't exist, create a new Category object
                if (categoryObj == null)
    {
        categoryObj = new Category 
        { 
            Name = r.Category 
            // If Category has other required fields, add them here
        };
        
        // Add to our tracker list and the Database context
        knownCategories.Add(categoryObj);
        context.Categories.Add(categoryObj);
    }

    // 4. Now create the Expense using the REAL category object
    expensesList.Add(new Expense
    {
        Date = r.Date,
        Category = categoryObj, // <--- No more red squiggle! We are assigning the Object.
        Amount = r.Amount,
        Description = r.Note
    });
}

            context.Expenses.AddRange(expensesList);
            context.SaveChanges();
        }
    }
}