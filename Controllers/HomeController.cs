using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FormApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.IO;

namespace FormApp.Controllers;

public class HomeController : Controller
{

    public HomeController()
    {

    }
    [HttpGet]
    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;
        if (!String.IsNullOrEmpty(searchString))
        {
            ViewBag.SearchString = searchString;
            products = products.Where(p => p.Name.ToLower().Contains(searchString)).ToList();
        }

        if (!String.IsNullOrEmpty(category) && category != "0")
        {
            products = products.Where(p => p.CategoryId == int.Parse(category)).ToList();
        }
        // ViewBag.Categories = new SelectList(Repository.Categories, "CatgoryId", "Name", category);
        var model = new ProductViewModel
        {
            Products = products,
            Categories = Repository.Categories,
            SelectCategory = category

        };
        return View(model);
    }
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = new SelectList(Repository.Categories, "CatgoryId", "Name");
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Product model, IFormFile ImageFile)
    {


        var extension = "";
        if (ImageFile != null)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            extension = Path.GetExtension(ImageFile?.FileName);
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Gecerli bir resim seciniz");

            }
        }
        if (ModelState.IsValid)
        {
            if (ImageFile != null)
            {
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
                model.ProductId = Repository.Products.Count + 1;
                Repository.CreateProduct(model);

            }
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CatgoryId", "Name");
        return View(model);
    }

    public IActionResult Edit(int? Id)
    {
        if (Id == null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == Id);
        if (entity == null)
        {
            return NotFound();
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CatgoryId", "Name");
        return View(entity);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? ImageFile)
    {
        if (id != model.ProductId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (ImageFile != null)
            {
                var extension = Path.GetExtension(ImageFile.FileName);
                var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                model.Image = randomFileName;
            }
            Repository.EditProduct(model);
            return RedirectToAction("Index");

        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CatgoryId", "Name");
        return View(model);
    }
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if (entity == null)
        {
            return NotFound();
        }
        return View("DeleteConfirm", entity);
    }
    [HttpPost]
    public IActionResult Delete(int id, int ProductId)
    {
        if (id != ProductId)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == ProductId);
        if (entity == null)
        {
            return NotFound();
        }
        Repository.DeleteProduct(entity);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult EditProducts(List<Product> Products)
    {
        foreach (var product in Products)
        {
            Repository.EditIsActive(product);
        }
        return RedirectToAction("Index");
    }

}