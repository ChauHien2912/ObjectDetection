using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_MachineService.Models;
using WPF_MachineService.Repository.Interface;

namespace WPF_MachineService.Repository.Implement
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DetectionObjectContext context;
        private IGenericRepository<Product> productRepository;
        


        public UnitOfWork(DetectionObjectContext context)
        {
            this.context = context;
        }

        public IGenericRepository<Product> ProductRepository
        {
            get
            {
                return productRepository ??= new GenericRepository<Product>(context);
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
