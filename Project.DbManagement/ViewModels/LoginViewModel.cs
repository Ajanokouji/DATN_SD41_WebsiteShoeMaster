using System.ComponentModel.DataAnnotations;

namespace Project.DbManagement.ViewModels;

public class LoginViewModel
{
    public Guid Id { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    public string Ten { get; set; }
    public string SDT { get; set; }
    public int? DiemTich { get; set; }
    public int? vaiTro { get; set; }    
}