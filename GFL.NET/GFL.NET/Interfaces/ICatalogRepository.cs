using GFL.NET.Models;

namespace GFL.NET.Interfaces
{
    public interface ICatalogRepository
    {
        public int FindByName(string name);
        public CatalogModel Add(CatalogModel newModel);
        public CatalogModel GetCatalog(int id);

        public List<CatalogModel> GetChild(int id);
        public List<CatalogModel> GetAll(int? id);
    }
}
