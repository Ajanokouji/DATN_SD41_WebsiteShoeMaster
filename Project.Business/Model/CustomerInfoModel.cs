using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Business.Model
{
    public class CustomerInfoModel
    {
        public Guid? UserId { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Notes { get; set; }
    }
} 