using AHD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.Repository.IRepository;

namespace DataAccess.Repository
{
    public class paymentRepository : Repository<Payment>, paymentIRepository
    {
        private readonly ApplicationDbContext dbContext;

        public paymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }


    }
}
