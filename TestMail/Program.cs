using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Project.Business;

// Load cấu hình từ appsettings.json
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var emailSettings = config.GetSection("EmailSettings").Get<EmailSettings>();

// Tạo email
var message = new MimeMessage();
message.From.Add(new MailboxAddress(emailSettings.SenderName, emailSettings.SenderEmail));
message.To.Add(new MailboxAddress("Người nhận", "vuh1119@gmail.com")); // Thay địa chỉ nhận
message.Subject = "Test Email từ Console App";

message.Body = new TextPart("plain")
{
    Text = "Đây là email gửi từ .NET Console App sử dụng MailKit. 2"
};

// Gửi email
using var client = new SmtpClient();
await client.ConnectAsync(
    emailSettings.SmtpServer,
    emailSettings.Port,
    SecureSocketOptions.StartTls
);

await client.AuthenticateAsync(emailSettings.Username, emailSettings.Password);
await client.SendAsync(message);
await client.DisconnectAsync(true);

Console.WriteLine("✅ Email đã được gửi thành công.");
