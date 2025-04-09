using Microsoft.AspNetCore.Mvc;
using Project.AdminSell.IServices;
using Project.DbManagement;
using Project.DbManagement.Entity;

namespace Project.AdminSell.Services;

public class SellOffService : ISellOffService
{
    private readonly ProjectDbContext _context;
    public SellOffService(ProjectDbContext context)
    {
        _context = context;
    }
    public List<BillEntity> GetAllPendingBill()
    {
        return _context.Bills.Where(hd => hd.Status == "Pending").OrderBy(hd => hd.CreatedOnDate).ToList();
    }
    public bool CreatePendingBill(Guid idEmployee)
    {
        try
        {
            BillEntity bill = new BillEntity();
            bill.Id = Guid.NewGuid();
            bill.BillCode = "BILL" + (bill.Id).ToString().Substring(0, 8).ToUpper();
            bill.EmployeeId = idEmployee;
            bill.CreatedOnDate = DateTime.Now;
            bill.Status = "Pending";
            _context.Bills.Add(bill);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }   
    }
    public bool DeletePendingBill(Guid idBill)
    {
        try
        {
            BillEntity bill = _context.Bills.Single(b => b.Id == idBill);
            if (bill != null)
            {
                _context.Bills.Remove(bill);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    public List<ProductEntity> GetAllProduct()
    {
        var lstProduct = _context.Products.ToList();
        return lstProduct;
    }
}