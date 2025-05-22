using Project.Business.Model;
using Project.DbManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.ModelFactory
{
    public interface IBillModelFactory
    {
        Task<BillModel> CreateModel(BillEntity entity);
        Task<BillModel> CreateModel(BillEntity entity, bool getBillDetail =false);
        Task<List<BillModel>> CreateModels(IEnumerable<BillEntity> entities);
        Task<List<BillModel>> CreateModels(IEnumerable<BillEntity> entities, bool getBillDetail = false);
        Task<BillEntity> ConvertEntity(BillModel entity);
        Task<List<BillEntity>> ConvertEntities(IEnumerable<BillModel> entities);
    }
}
