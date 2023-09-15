using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using R54_M10_Class_01_Work_01.Models;
using R54_M10_Class_01_Work_01.ViewModels.Input;
using X.PagedList;
namespace R54_M10_Class_01_Work_01.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductDbContext db;
        private readonly IWebHostEnvironment env;
        public ProductsController(ProductDbContext db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }
        public async Task<IActionResult> Index(int pg=1)
        {
           var data = await db.Products
                .Include(x => x.ProductInventories)
                .OrderBy(x => x.ProductId)
                .ToPagedListAsync(pg, 5);
            return View(data);
        }
        public async Task<IActionResult> ProductList(int pg=1)
        {
            var data = await db.Products
                 .Include(x => x.ProductInventories)
                 .OrderBy(x => x.ProductId)
                 .ToPagedListAsync(pg, 5);
            return PartialView("_ProductList", data);
        }
        public IActionResult Create()
        {
            ProductInputModel data = new ProductInputModel();
            data.ProductInventories.Add(new ProductInventory { Date = null, Quantity = null });
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductInputModel model, string act="")
        {
            if (act == "add")
            {
                model.ProductInventories.Add(new ProductInventory { Date = null, Quantity = null });
                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }

            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.ProductInventories.RemoveAt(index);

                foreach (var v in ModelState.Values)
                {

                    v.RawValue = null;
                    v.Errors.Clear();
                }

            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var data = new Product
                    {
                        Name = model.Name,
                        UnitPrice = model.UnitPrice ?? 0,
                        SellUnit = model.SellUnit

                    };
                    //
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string fileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{ext}";
                    string savePath = Path.Combine(this.env.WebRootPath, "Images", fileName);
                    FileStream fs = new FileStream(savePath, FileMode.Create);
                    await model.Picture.CopyToAsync(fs);
                    fs.Close();
                    //
                    data.Picture = fileName;
                    foreach (var pi in model.ProductInventories)
                    {
                        data.ProductInventories.Add(pi);
                    }
                    await db.Products.AddAsync(data);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(model);

        }
        public async Task<IActionResult> Edit(int id)
        {
            var data = await db.Products.Include(x => x.ProductInventories).FirstOrDefaultAsync(x => x.ProductId == id);
            if (data == null) return NotFound();
            var modelData = new ProductEditModel
            {
                ProductId = id,
                Name = data.Name,
                UnitPrice = data.UnitPrice,
                SellUnit = data.SellUnit
            };
            foreach (var pi in data.ProductInventories)
            {
                modelData.ProductInventories.Add(pi);
            }
            ViewBag.CurrentPic = data.Picture;
            return View(modelData);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditModel model, string act = "")
        {
            var data = await db.Products.Include(x => x.ProductInventories).FirstOrDefaultAsync(x => x.ProductId == model.ProductId);
            if (data == null) return NotFound();
            if (act == "add")
            {
                model.ProductInventories.Add(new ProductInventory { Date = null, Quantity = null });
                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.ProductInventories.RemoveAt(index);

                foreach (var v in ModelState.Values)
                {

                    v.RawValue = null;
                    v.Errors.Clear();
                }

            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    data.Name = model.Name;
                    data.UnitPrice = model.UnitPrice;
                    data.SellUnit = model.SellUnit;

                };
                if (model.Picture != null)
                {
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string fileName = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}{ext}";
                    string savePath = Path.Combine(this.env.WebRootPath, "Images", fileName);
                    FileStream fs = new FileStream(savePath, FileMode.Create);
                    model.Picture?.CopyTo(fs);
                    fs.Close();
                    //
                    data.Picture = fileName;
                }
                else
                {
                    //nothing

                }
                db.ProductInventories.RemoveRange(db.ProductInventories.Where(x => x.ProductId == model.ProductId).ToList());
                foreach (var pi in model.ProductInventories)
                {
                        await db.ProductInventories.AddAsync(new ProductInventory { Date=pi.Date, Quantity=pi.Quantity, ProductId=model.ProductId});
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CurrentPic = data.Picture;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await db.Products.FirstOrDefaultAsync(x=> x.ProductId == id);
            if (data == null) return NotFound();
            db.Products.Remove(data);

            if (await db.SaveChangesAsync() > 0)
                return Json(new { success = true, id });
            else
                return Json(new { success = false, id });
        }
    }
}
