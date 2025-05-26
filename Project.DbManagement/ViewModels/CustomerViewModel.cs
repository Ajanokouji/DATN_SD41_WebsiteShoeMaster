namespace Project.DbManagement.ViewModels;

public class CustomerViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool? IsAnonymous { get; set; } = true;
    public Guid? CreatedByUserId { get; set; }
    public Guid? LastModifiedByUserId { get; set; }
    public DateTime? CreatedOnDate { get; set; }
    public DateTime? LastModifiedOnDate { get; set; }
}