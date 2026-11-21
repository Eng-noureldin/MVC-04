using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Session04_MVC.BLL;
using Session04_MVC.DAL;
using Session04_MVC.Helpers;
using Session04_MVC.ViewModels;

namespace Session04_MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Products (with Search, Filter, Pagination)
        public async Task<IActionResult> Index(string? searchName, int? categoryId, int page = 1)
        {
            const int pageSize = 5;

            var query = _unitOfWork.Products.GetQueryable().Include(p => p.Category).AsNoTracking();

            // 1. Search by Name
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(p => p.Name.Contains(searchName.Trim()));
                ViewBag.SearchName = searchName;
            }

            // 2. Filter by Category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.SelectedCategory = categoryId.Value;
            }

            query = query.OrderByDescending(p => p.CreatedAt);

            // 3. Paginated list
            var paginatedProducts = await PaginatedList<Product>.CreateAsync(query, page, pageSize);

            // Pass categories for filter dropdown
            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);

            return View(paginatedProducts);
        }

        // GET: /Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Products.GetQueryable()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var vm = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                ExistingImageName = product.ImageName
            };
            return View(vm);
        }

        // GET: /Products/Create
        public async Task<IActionResult> Create()
        {
            var vm = new ProductViewModel
            {
                AvailableCategories = await GetCategorySelectListAsync()
            };
            return View(vm);
        }

        // POST: /Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                string? fileName = null;
                if (vm.Image != null)
                {
                    fileName = DocumentSettings.UploadFile(vm.Image, "images");
                }

                var product = new Product
                {
                    Name = vm.Name,
                    Price = vm.Price,
                    Description = vm.Description,
                    StockQuantity = vm.StockQuantity,
                    CategoryId = vm.CategoryId,
                    ImageName = fileName
                };

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }

            vm.AvailableCategories = await GetCategorySelectListAsync();
            return View(vm);
        }

        // GET: /Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return NotFound();

            var vm = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                ExistingImageName = product.ImageName,
                AvailableCategories = await GetCategorySelectListAsync()
            };
            return View(vm);
        }

        // POST: /Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null) return NotFound();

                // If user uploads a new image, delete the old one and upload the new one
                if (vm.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageName))
                    {
                        DocumentSettings.DeleteFile(product.ImageName, "images");
                    }
                    product.ImageName = DocumentSettings.UploadFile(vm.Image, "images");
                }

                product.Name = vm.Name;
                product.Price = vm.Price;
                product.Description = vm.Description;
                product.StockQuantity = vm.StockQuantity;
                product.CategoryId = vm.CategoryId;

                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }

            vm.AvailableCategories = await GetCategorySelectListAsync();
            return View(vm);
        }

        // GET: /Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Products.GetQueryable()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var vm = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryName = product.Category?.Name,
                ExistingImageName = product.ImageName
            };
            return View(vm);
        }

        // POST: /Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageName))
                {
                    DocumentSettings.DeleteFile(product.ImageName, "images");
                }
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }
    }
}
