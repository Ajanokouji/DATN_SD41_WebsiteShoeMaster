namespace Project.Business.Configuration
{
    public class VNPayConfig
    {
        public static string ConfigName => "VNPay";
        public string Version { get; set; }
        public string TmnCode { get; set; }
        public string HashSecret { get; set; }
        public string PaymentUrl { get; set; }
        public string ReturnUrl { get; set; }
    }
} 