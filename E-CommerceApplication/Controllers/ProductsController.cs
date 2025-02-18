using E_CommerceApplication.BLL.DTO;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.BLL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
         private readonly IBaseRepository<Product> _baseRepository;
         private readonly IWebHostEnvironment _webEnvironment;
         private readonly IProductRepository _productRepository;

         private readonly List<string> listCategories = new List<string>()
         {
            "Phones", "Computers", "Accessories", "Printers", "Cameras", "Other"
         };

         public ProductsController(IBaseRepository<Product> baseRepository,
             IWebHostEnvironment webEnvironment, IProductRepository productRepository)
         {
            _baseRepository = baseRepository;
            _webEnvironment = webEnvironment;
            _productRepository = productRepository;
             
         }

        [HttpGet(nameof(GetProducts))]
        public IActionResult GetProducts
            (string? search, string? category,
            int? minPrice, int? maxPrice, string? sort, string? order, int? page)
        {
            IQueryable<Product> query = _productRepository.query();
            
            // search Logic
            if (search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }
            // updating the query to make it match the search

            if (category != null)
            {
                query = query.Where(p => p.Category == category); 
            }
            // updating the query to make it match the category

            if (minPrice != null)
            {
                query = query.Where(p => p.Price >= minPrice);
            }
            // update the query to return the products which more than minPrice

            if (maxPrice != null)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }
            // update the query to return the products which more than minPrice


            // sort Logic 
            if (sort == null) sort = "Id";
            if (order == null || order != "asc")
            {
                order = "desc";
            }

            // 
            if (sort.ToLower() == "name")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }
            else if (sort.ToLower() == "brand")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
           else if (sort.ToLower() == "category")
           {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
           }
            else if (sort.ToLower() == "price")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if(sort.ToLower() == "date")
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else
            {
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            // pagination Logic
            if (page == null || page < 1) page = 1;

            int pageSize = 5;
            int totalPages = 0;

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((int)(page - 1) * pageSize ).Take(pageSize); // update the query to read only the products of the requested page

            var products = query.ToList();
            // return the query to list of products

            // object of type anonymous to return the number of pages with requested page products
            var response = new
            {
                totalPages = totalPages,
                products = products,
                pageSize = pageSize,
                page = page,
            };

            return Ok(response);
        }

        [HttpGet(nameof(GetCategories))]
        public IActionResult GetCategories()
        {
            return Ok(listCategories);
        }

        [HttpPost(nameof(CreateProduct))]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDTO productDTO)
        {
            if (!listCategories.Contains(productDTO.Category))
            {
                ModelState.AddModelError("category", "please select a valid category");
                return BadRequest(ModelState);
            }
            if (productDTO == null)
            {
                return BadRequest("Product Data is null");
            }
            if (productDTO.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image file is required !");
                return BadRequest(ModelState);
            }

            // save image in server

            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssff");
            //This generates a unique string based on the current date and time:
           // This ensures that the file name is unique, reducing the chances of overwriting an existing file.

            imageFileName += Path.GetExtension(productDTO.ImageFile.FileName);
           // Path.GetExtension extracts the file extension from the uploaded file name (e.g., .jpg, .png, .pdf).
            
            string imagesFolder = _webEnvironment.WebRootPath + "/images/products/";
            // _webEnvironment.WebRootPath gives the root path of the wwwroot folder,
            // where static files (e.g., images, CSS, JS) are typically stored.


            using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
            {
                productDTO.ImageFile.CopyTo(stream);
            }
            // Creates a new file at the specified path (imagesFolder + imageFileName).


            // save product to database
            Product product = new Product()
            {
                Name = productDTO.Name,
                Brand = productDTO.Brand,
                Category = productDTO.Category,
                Price = productDTO.Price,
                Description = productDTO.Description??"",
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now,
            };

            try
            {
                await _baseRepository.AddAsync(product);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occured while adding a new product.");
            }

            return Ok(productDTO);   
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _baseRepository.GetByIdAsync(id);
            if (product == null)
            {
                return BadRequest($"there is no product with id {id}");
            }
            ProductDTO productDTO = new ProductDTO()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            return Ok(productDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDTO productDTO)
        {
            var product = await _baseRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            /*
            //string imageFileName = product.ImageFileName;
            //if (imageFileName != null)
            //{
            //    // save the image in the server 
            //    imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //    imageFileName += Path.GetExtension(productDTO.ImageFile.FileName);

            //    string imagesFolder = _webEnvironment.WebRootPath + "/images/products/";
            //    using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
            //    {
            //        productDTO.ImageFile.CopyTo(stream);
            //    }

            //    // delete the old image
            //    System.IO.File.Delete(imagesFolder + product.ImageFileName);
            //}
            */

            string imageFileName = product.ImageFileName;
            if (productDTO.ImageFile != null)
            {
                imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                imageFileName += Path.GetExtension(productDTO.ImageFile.FileName);

                string imagesFolder = Path.Combine(_webEnvironment.WebRootPath, "images", "products");
                Directory.CreateDirectory(imagesFolder); // Ensure directory exists

                using (var stream = System.IO.File.Create(Path.Combine(imagesFolder, imageFileName)))
                {
                    productDTO.ImageFile.CopyTo(stream);
                }

                // Delete the old image if it exists
                string oldFilePath = Path.Combine(imagesFolder, product.ImageFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // update product on DB
            product.Name = productDTO.Name;
            product.Brand = productDTO.Brand;
            product.Category = productDTO.Category;
            product.Price = productDTO.Price;
            product.Description = productDTO.Description??"";
            product.ImageFileName = imageFileName;

            try
            {
                await _baseRepository.UpdateAsync(product);
            }
            catch (Exception )
            {
                return StatusCode(500);
            }
            return Ok(productDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _baseRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            string imageFolder = _webEnvironment.WebRootPath + "/images/products/";
            System.IO.File.Delete(imageFolder + product.ImageFileName);

            // delete the product from DB
            try
            {
                await _baseRepository.DeleteAsync(product);
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
            return NoContent();
        }




    }
}
