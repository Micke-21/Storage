#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Storage.Data;
using Storage.Models;

namespace Storage.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StorageContext _context;

        public ProductsController(StorageContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Orderdate,Category,Shelf,Count,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Orderdate,Category,Shelf,Count,Description")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Sumary()
        {
            var productViewModel = _context.Product.Select(e => new ProductViewModel
            {
                Name = e.Name,
                Price = e.Price,
                Count = e.Count,

            });

            return View(nameof(Sumary), await productViewModel.ToListAsync());
        }

        public async Task<IActionResult> Search(string category)
        {
            var viewModel = _context.Product
                .Where(e => e.Category == category)
                .Select(e => new ProductViewModel
                {
                    Id = e.Id,
                    Count = e.Count,
                    Name = e.Name,
                    Price = e.Price

                });

            return View(nameof(Sumary), await viewModel.ToListAsync());
        }

        public async Task<IActionResult> SearchProduct(string category)
        {
            var viewModel = _context.Product
                .Where(e => e.Category == category)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Count = p.Count,
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.Category,
                    Orderdate = p.Orderdate,
                    Shelf = p.Shelf,
                    Description = p.Description
                });

            return View(nameof(Index), await viewModel.ToListAsync());
        }


        public async Task<IActionResult> Sumary2(string product, string category)
        {
            CultureInfo culture = new CultureInfo("sv-SE");
           
            var cat = _context.Product
                .Select(c => c.Category)
                .Distinct()
                .OrderBy(c => c)
                //.OrderBy(c => c, StringComparer.Create(culture, CompareOptions.IgnoreCase))
                //.OrderBy(c => c, StringComparer.Create(culture, ignoreCase: true))
                ;
            //ToDo OrderBy sorterar åäö fel!!!


            var viewModel = _context.Product
                .Where(e => e.Category == category || category == "Kategori" || string.IsNullOrWhiteSpace(category))
                //.Where(e => true)
                .Where(e => (string.IsNullOrWhiteSpace(product)) || e.Name.StartsWith(product) )

                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Count = p.Count,
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.Category,
                    //Orderdate = p.Orderdate,
                    //Shelf = p.Shelf,
                    //Description = p.Description
                })
                .OrderBy(o => o.Category)
                .ThenBy(o => o.Name);

            var prodCatModel = new ProductCategoriViewModel
            {
                Categories = await cat.ToListAsync(),
                Products = (IEnumerable<ProductViewModel>)await viewModel.ToListAsync()
            };

            return View(nameof(Sumary2), prodCatModel);
        }
    }
}
