using CsvHelper.Configuration.Attributes;

    public class RecordsCSV
    {
        public required DateTime Date { get; set; }
        public String? Mode { get; set; }
        public required String Category { get; set; }
        public String? SubCategory { get; set; }

        public String? Note { get; set; }
        public required decimal Amount { get; set; }

        [Name("Income/Expense")]   
        public required String IncomeExpense { get; set; }
        public required String Currency { get; set; }
    }