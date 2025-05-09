using Newtonsoft.Json;
using Project.DbManagement.Entity;
using System.ComponentModel.DataAnnotations;

public class District: BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Slug { get; set; }

    public string? NameWithType { get; set; }

    public string? Path { get; set; }

    public string? PathWithType { get; set; }

    public string? Code { get; set; }

    public string? ParentCode { get; set; }


}
