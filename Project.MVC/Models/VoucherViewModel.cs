namespace Project.MVC.Models
{    

    public class VoucherViewModel
    {
        public Guid Id { get; set; }
        public bool? IsDisable { get; set; } = false;
        public string Code { get; set; } = string.Empty;
        public string VoucherName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal? DiscountAmount { get; set; }          // Số tiền giảm cố định
        public decimal? DiscountPercent { get; set; }          // Hoặc giảm %
        public decimal? MaxDiscountAmount { get; set; }          // Trần giảm khi giảm %
        public decimal? MinimumOrderAmount { get; set; }          // Đơn tối thiểu

        public int? TotalMaxUsage { get; set; }
        public int? RedeemCount { get; set; }

        /* ------------ Helper properties ------------ */
        public bool IsPercent => DiscountPercent is > 0;
        public string DiscountLabel
            => IsPercent
               ? $"Giảm {DiscountPercent:0}%"
               : $"Giảm tối đa {DiscountAmount:0,0₫}";
        public string MinOrderLabel
            => MinimumOrderAmount is > 0
               ? $"Đơn Tối Thiểu {MinimumOrderAmount:0,0₫}"
               : "Không yêu cầu giá trị tối thiểu";
        public string ExpiredLabel
            => EndDate.HasValue ? EndDate.Value.ToString("dd.MM.yyyy") : "—";
    }

}
