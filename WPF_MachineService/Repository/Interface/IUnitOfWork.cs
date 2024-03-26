using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_MachineService.Models;

namespace WPF_MachineService.Repository.Interface
{
    public interface IUnitOfWork
    {
        IGenericRepository<Product> ProductRepository { get; }

        void Save();
    }
}
