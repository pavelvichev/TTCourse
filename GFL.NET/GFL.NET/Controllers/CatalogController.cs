using GFL.NET.Interfaces;
using GFL.NET.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GFL.NET.Controllers
{
	public class CatalogController : Controller
	{
		private readonly AppDbContext _context;
		private readonly ICatalogRepository _catalogRepository;



		public CatalogModel model { get; set; }
		public List<CatalogModel> Childs { get; set; }
		List<CatalogModel> all;

		public string error;
		public CatalogController(AppDbContext context, ICatalogRepository catalogRepository)
		{
			_context = context;
			_catalogRepository = catalogRepository;
		}

		public IActionResult Catalog(int id)
		{
			all = _catalogRepository.GetAll(null);
			if(id ==0)
			{
				id = 1; 
			}
			//model = _context.Catalogs.FirstOrDefault();
			
			model = _catalogRepository.GetCatalog(id);
			Childs = _catalogRepository.GetChild(id);
			ViewBag.Childs = Childs;

			ViewBag.ErrorMessage = TempData["errorMessage"] as string;
			ViewBag.Export = TempData["export"] as string;

			return View(model);
		}
		[HttpPost]
		public IActionResult Import(string path, int id)
		{
			all = _catalogRepository.GetAll(null);

			
			try
			{
				

				string Last = path.Split(@"\").Last();

				CatalogModel newCatalog = new CatalogModel();
				newCatalog.Name = Last;
				newCatalog.ParentId = id;

				_catalogRepository.Add(newCatalog);

				ImportSubdirectories(path, newCatalog);
			}
			catch (Exception e)
			{
				TempData["errorMessage"] = e.Message;
				return RedirectToAction("Catalog");
			}

			return Redirect("Catalog");
		}

		private void ImportSubdirectories(string path, CatalogModel parent)
		{
			string[] subdirectories = Directory.GetDirectories(path);

			foreach (string subdirectory in subdirectories)
			{
				string name = subdirectory.Split(@"\").Last();


				CatalogModel catalog = new CatalogModel
				{
					Name = name,
					ParentId = _catalogRepository.FindByName(parent.Name)
				};

				_catalogRepository.Add(catalog);

				ImportSubdirectories(subdirectory, catalog);
			}
		}
		[HttpPost]
		public IActionResult ExportFile(int id)
		{
			all = _catalogRepository.GetAll(id);

			var fileName = "BD.txt";
			var path = Path.Combine(@"C:\Users\Steklyashka\Downloads", fileName);


			using (var streamWriter = new StreamWriter(path))
			{
				foreach (var catalog in all)
				{
					// Записываем в файл каждую запись в формате "Id	Name	ParentId"
					streamWriter.WriteLine($"{catalog.Id},{catalog.Name},{(catalog.ParentId.HasValue ? catalog.ParentId.Value.ToString() : "NULL")}");
				}
			}
			TempData["export"] = $"Файл {fileName} успешно экспортирован в папку Downloads";



			return RedirectToAction("Catalog");
		}

		public IActionResult ImportFile(IFormFile file, int id)
		{
			
			AddtoDB(ReturnList(file,id),id);
			return RedirectToAction("Catalog");
		}

		List<CatalogModel> ReturnList(IFormFile file, int id)
		{

			List<CatalogModel> list = new List<CatalogModel>();
			try
			{
				var path = Path.Combine(@"C:\Users\Steklyashka\Downloads", file.FileName);

				using (StreamReader stream = new StreamReader(path))
				{
					string line = null;
					while ((line = stream.ReadLine()) != null)
					{
						var strings = line.Split(',').ToList();
						var catalog = new CatalogModel
						{
							Id = int.Parse(strings[0]),
							Name = strings[1],
							ParentId = (strings[2] != "NULL" ? int.Parse(strings[2]) : id),
						};
						list.Add(catalog);

					}
					return list;
				}
				
			}
			catch(Exception ex)
			{
				TempData["errorMessage"] = ex.Message;
				return null;
			}

		}

		void AddtoDB(List<CatalogModel> catalogs, int ParId)
		{
			if (catalogs == null)
				return;
			var first = catalogs.FirstOrDefault();
			first.ParentId = ParId;
			_catalogRepository.Add(first);
			var catalogs2 = catalogs; 
			var childs = new List<CatalogModel>();
			
			int id = 0;
			int i = 0;
			foreach (var catalog in catalogs) 
			{
				
				int tempid = catalog.Id;
				if (i == 0)
				{
					id = _context.Catalogs.Max(c => c.Id);
					i++;
				}
				
				
				foreach (var catalog2 in catalogs2.Skip(1))
				{
					
					if (catalog2.ParentId == tempid)
					{
						childs.Add(catalog2);
						id = _context.Catalogs.Where(c => c.Name == catalog.Name).OrderBy(c => c.Id).Last().Id;
					}
				}
				foreach (var child in childs)
				{
					
					child.ParentId = id;
					_catalogRepository.Add(child);
				}
				if(childs.Count == 0) 
				{
					continue;
				}
				childs.Clear();
				
			}
			
		}
	}



}



