using BL_Tutorial_Izzat.DAL.Models;
using Nexus.Base.CosmosDBRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BL_Tutorial_Izzat.BLL
{
    public class ClassService
    {
        private readonly IDocumentDBRepository<DTOClass> _repository;
        public ClassService(IDocumentDBRepository<DTOClass> repository)
        {
            if (this._repository == null)
            {
                this._repository = repository;
            }
        }

        public async Task<DTOClass> GetClassById(string id, Dictionary<string, string> pk)
        {
            return await _repository.GetByIdAsync(id, pk);
        }

        // TODO: mana CRUD yang lain?
    }
}
