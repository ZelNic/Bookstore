using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public interface IUnitOfWork
        {           
            IProductRepository Product { get; }         
            void SaveAsync();
        }
    }
}
