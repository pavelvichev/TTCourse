using GFL.NET.Interfaces;
using GFL.NET.Models;
using Microsoft.EntityFrameworkCore;

namespace GFL.NET.Mocks
{
    public class CatalogMock : ICatalogRepository
    {
        private readonly AppDbContext _context;

        public CatalogMock(AppDbContext context)
        {
            _context = context;
        }
        public int FindByName(string name)
        {
            return GetAll(null).Find(c => c.Name == name).Id;

        }
        public CatalogModel Add(CatalogModel newModel)
        {
            _context.Database.ExecuteSqlRaw("AddNewCatalog {0},{1}", newModel.Name, newModel.ParentId);
            return newModel;
        }
      
        public CatalogModel GetCatalog(int id)
        {
          return  _context.Catalogs.FromSqlRaw("SELECT * FROM Catalogs Where Id = {0}", id).ToList().FirstOrDefault();
            
        }

        public List<CatalogModel> GetAll(int? id)
        {
            if (id.HasValue)
            {
                return _context.Catalogs.FromSqlRaw("SELECT * FROM Catalogs Where Id >= {0}",id).ToList();
                
            }
            return _context.Catalogs.FromSqlRaw("SELECT * FROM Catalogs").ToList();
        }

        public List<CatalogModel> GetChild(int id)
        {
            return _context.Catalogs.FromSqlRaw("SELECT * FROM Catalogs where ParentId = {0}", id).ToList();
        }

        
    }
}
