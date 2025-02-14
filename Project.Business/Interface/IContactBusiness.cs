using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IContactBusiness
    {
        Task<Pagination<Contacts>> GetAllAsync(ContactQueryModel queryModel);

        /// <summary>
        /// Gets list of contacts.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of contacts.</returns>
        Task<IEnumerable<Contacts>> ListAllAsync(ContactQueryModel queryModel);

        /// <summary>
        /// Count the number of contacts by query model.
        /// </summary>
        /// <param name="queryModel">The contacts query model.</param>
        /// <returns>The number of contacts by query model.</returns>
        Task<int> GetCountAsync(ContactQueryModel queryModel);

        /// <summary>
        /// Gets list of contacts.
        /// </summary>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of contacts.</returns>
        Task<IEnumerable<Contacts>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a contact.
        /// </summary>
        /// <param name="contactId">The contact id.</param>
        /// <returns>The contact.</returns>
        Task<Contacts> FindAsync(Guid contactId);

        /// <summary>
        /// Deletes a contact.
        /// </summary>
        /// <param name="contactId">The contact id.</param>
        /// <returns>The deleted contact.</returns>
        Task<Contacts> DeleteAsync(Guid contactId);

        /// <summary>
        /// Deletes a list of contacts.
        /// </summary>
        /// <param name="deleteIds">The list of contact ids.</param>
        /// <returns>The deleted contacts.</returns>
        Task<IEnumerable<Contacts>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a contact.
        /// </summary>
        /// <param name="contactEntity"></param>
        /// <returns></returns>
        Task<Contacts> SaveAsync(Contacts contactEntity);

        /// <summary>
        /// Saves contacts.
        /// </summary>
        /// <param name="contactEntities"></param>
        /// <returns></returns>
        Task<IEnumerable<Contacts>> SaveAsync(IEnumerable<Contacts> contactEntities);

        /// <summary>
        /// Updates a contact.
        /// </summary>
        /// <param name="contactEntity"></param>
        /// <returns></returns>
        Task<Contacts> PatchAsync(Contacts contactEntity);
    }
}