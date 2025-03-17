namespace Project.MVC.Models
{
    public class BankingInfoViewModel
    {
        public Guid BillId { get; set; }
        public string BillCode { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string AccountName { get; set; }
    }
} 