using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.DbManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Intercepter.Implement
{
    public class SendEmailAfterSaveBill : IBillIntercepterAfterSave
    {
        private readonly IBillDetailsBusiness _billDetailsBusiness;
        private readonly EmailSettings EmailSettings;

        public SendEmailAfterSaveBill(IBillDetailsBusiness billDetailsBusiness,EmailSettings emailSettings)
        {
            _billDetailsBusiness = billDetailsBusiness;
            EmailSettings =emailSettings;
        }

        public int Order { get; set; } = 100;

        public async Task Intercept(BillEntity oldNodeEntity, BillEntity updatedNodeEntity)
        {
            if(oldNodeEntity!= null && updatedNodeEntity !=null&&oldNodeEntity.Status != updatedNodeEntity.Status&& oldNodeEntity!= null)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailSettings.SenderName, EmailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("Người nhận", updatedNodeEntity.RecipientEmail)); // Thay địa chỉ nhận
                message.Subject = "Thông tin trạng thái đơn hàng";

                var billDetails = await _billDetailsBusiness.ListAllByIdBill(updatedNodeEntity.Id);
                var template = HTMLTemplate();

                template = template.Replace("{{TenKhachHang}}", updatedNodeEntity.RecipientName);
                template = template.Replace("{{RecipientPhone}}", updatedNodeEntity.RecipientPhone);
                template = template.Replace("{{RecipientAddress}}", updatedNodeEntity.RecipientAddress);
                template = template.Replace("{{TrangThai}}", updatedNodeEntity.Status.ToString());
                template = template.Replace("{{ThoiGianCapNhat}}", updatedNodeEntity.LastModifiedOnDate?.ToString("dd-MM-yyyy") ?? string.Empty);
                template = template.Replace("{{MaDonHang}}", updatedNodeEntity.BillCode.ToString());


                var productContent = string.Empty;

                if(billDetails!=null&&billDetails.Any())
                {
                    foreach (var item in billDetails)
                    {
                        productContent+= $@"
    <tr>
        <td>{item.ProductName} - Size:{item.Size} - Màu sắc:{item.Color}</td>
         <td>{item.Quantity}</td>
         <td>{item.Price}</td>
         <td>{item.TotalPrice}</td>
    <tr>
";
                    }
                }

                template = template.Replace("{{ProductContent}}",productContent);


                message.Body = new TextPart("html")
                {
                    Text = template
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    EmailSettings.SmtpServer,
                    EmailSettings.Port,
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(EmailSettings.Username, EmailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            } 
        }

        private string HTMLTemplate()
        {
            return @"
            <!DOCTYPE html>
<html>
<head>
  <meta charset=""UTF-8"">
  <title>Cập nhật trạng thái đơn hàng</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; margin: 0;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; overflow: hidden;"">
    <tr>
      <td style=""background-color: #007bff; padding: 20px; text-align: center; color: #ffffff; font-size: 24px;"">
        Cập nhật trạng thái đơn hàng
      </td>
    </tr>
    <tr>
      <td style=""padding: 20px;"">
        <p style=""font-size: 16px;"">Xin chào <strong>{{TenKhachHang}}</strong>,</p>
        <p style=""font-size: 16px;"">Đơn hàng của bạn đã được cập nhật với thông tin mới nhất:</p>

        <table width=""100%"" cellpadding=""8"" cellspacing=""0"" border=""0"" style=""font-size: 14px; border-collapse: collapse;"">
          <tr>
            <td style=""background-color: #f0f0f0; width: 40%;""><strong>Mã đơn hàng:</strong></td>
            <td>{{MaDonHang}}</td>
          </tr>
          <tr>
            <td style=""background-color: #f0f0f0;""><strong>Thời gian cập nhật:</strong></td>
            <td>{{ThoiGianCapNhat}}</td>
          </tr>
          <tr>
            <td style=""background-color: #f0f0f0;""><strong>Trạng thái:</strong></td>
            <td><strong style=""color: #28a745;"">{{TrangThai}}</strong></td>
          </tr>
        </table>

        <h3 style=""margin-top: 30px; font-size: 16px; border-bottom: 1px solid #ddd; padding-bottom: 5px;"">Thông tin sản phẩm</h3>
        <table width=""100%"" cellpadding=""8"" cellspacing=""0"" border=""1"" style=""font-size: 14px; border-collapse: collapse; text-align: left;"">
          <thead style=""background-color: #f0f0f0;"">
            <tr>
              <th>Sản phẩm</th>
              <th>Số lượng</th>
              <th>Giá</th>
              <th>Tổng</th>
            </tr>
          </thead>
          <tbody>
           
              {{ProductContent}}
            
          </tbody>
        </table>

        <p style=""margin-top: 20px; font-size: 14px;"">Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email hoặc hotline hỗ trợ khách hàng.</p>

        <p style=""font-size: 14px;"">Trân trọng,<br><strong>Đội ngũ CSKH</strong></p>
      </td>
    </tr>
    <tr>
      <td style=""background-color: #f8f8f8; padding: 15px; text-align: center; font-size: 12px; color: #666;"">
        © 2025 Công ty ABC. Mọi quyền được bảo lưu.
      </td>
    </tr>
  </table>
</body>
</html>
";
        }
    }
}
