using BL_Tutorial_Izzat.DAL.Models;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL_Tutorial_Izzat.BLL
{
    public class FetchService
    {
        private readonly IDocumentDBRepository<DTOClass> repository;

        public FetchService(IDocumentDBRepository<DTOClass> repository)
        {
            if (this.repository == null)
            {
                this.repository = repository;
            }
        }

        public async Task<DTOClass> GetDtoClassById(string id)
        {
            return await repository.GetByIdAsync(id);
        }

        public async Task<PageResult<DTOClass>> GetAllDtoClass()
        {
            return await repository.GetAsync();
        }

        public async Task<DTOClass> CreateNewDtoClass(DTOClass dto)
        {
            return await repository.CreateAsync(dto);

        }

        public async Task<DTOClass> UpdateDtoClas(string id, DTOClass dto)
        {
            return await repository.UpdateAsync(id, dto);
        }

        public async Task<string> DeleteDtoClass(string id)
        {
            try
            {
                await repository.DeleteAsync(id);
                return "item deleted";
            } catch
            {
                return "fail to delete";
            }
        }
    }
}
