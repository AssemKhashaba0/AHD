using AHD.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class MosqueRepository : Repository<Mosque>, MosqueIRepository
    {
        private readonly ApplicationDbContext dbContext;

        public MosqueRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }


    }
}

