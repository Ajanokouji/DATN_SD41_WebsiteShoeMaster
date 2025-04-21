using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.ModelFactory
{
    public interface IBillDetailModelFactory
    {
        Task<BillDetailModel> CreateModel(BillDetailsEntity entity);
        Task<List<BillDetailModel>> CreateModels(IEnumerable<BillDetailsEntity> entities);
        Task<BillDetailsEntity> ConvertEntity(BillDetailModel entity);
        Task<List<BillDetailsEntity>> ConvertEntities(IEnumerable<BillDetailModel> entities);

    }
}
