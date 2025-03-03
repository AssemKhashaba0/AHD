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
    public class DeliveryLocationRepository : Repository<DeliveryLocation>, DeliveryLocationIRepository
    {
        private readonly ApplicationDbContext dbContext;

        public DeliveryLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }


    }
}
