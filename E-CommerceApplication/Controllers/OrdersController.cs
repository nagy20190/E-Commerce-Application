using System.Threading.Tasks;
using E_CommerceApplication.BLL.DTO;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<User> _userRepository;
        
        public OrdersController(IBaseRepository<Order> orderRepository, 
            IBaseRepository<Product> productRepository,
            IBaseRepository<User> userRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDTO orderDTO)
        {
            if (orderDTO == null)
            {
                return BadRequest("Data Cannot be empty");
            }
            // check if Payment Method belong to our payment methods
            if (!OrderHelper.PayementMethods.ContainsKey(orderDTO.PaymentMethod))
            {
                ModelState.AddModelError("Payment Method", "Please select a valid payment mehtod");
                return BadRequest(ModelState);
            }
            int userId = JWTReader.GetUserId(User);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("User", "Enable to create the order");
                return BadRequest(ModelState);
            }
            var productDitctionary = OrderHelper.getProductDictionary(orderDTO.ProductIdentifiers);

            var order = new Order();
            order.UserId = userId;
            order.CreatedAt = DateTime.Now;
            order.ShippingFee = OrderHelper.ShipingFee;
            order.DeliveryAddress = orderDTO.DeliveryAddress;
            order.PaymentMethod = orderDTO.PaymentMethod;
            order.PaymentStatus = OrderHelper.PaymentStatuses[0]; // Pending
            order.OrderStatus = OrderHelper.OrderStatuses[0]; // Created

            foreach (var pair in productDitctionary)
            {
                int productId = pair.Key;
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    ModelState.AddModelError("Product", $"Product With id ${productId} is not avaliable");
                    return BadRequest(ModelState);
                }
                var orderItem = new OrderItem();
                orderItem.ProductId = productId;
                orderItem.Quantity = pair.Value;
                orderItem.UnitPrice = product.Price;

                order.OrderItems.Add(orderItem);
            }

            if (order.OrderItems.Count < 1)
            {
                ModelState.AddModelError("Order", $"Enable to create the order");
                return BadRequest(ModelState);
            }

            // save a new order in DB
            await _orderRepository.AddAsync(order);

            // for every item we set item to null to avoid Cycle exception
            foreach (var item in order.OrderItems)
            {
                item.Order = null!;
            }

            // hide user password
            order.User.Password = "";
            return Ok(order);
        }

        // Read Orders from DB
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders(int? page)
        {
            int userId = JWTReader.GetUserId(User);
            string userRole = _userRepository.Find(u => u.Id == userId)?.Result.Role ?? "";

            IQueryable<Order> query = _orderRepository.query().Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product);

            if (userRole != "admin")
            {
                query = query.Where(o => o.UserId == userId);
            }

            query = query.OrderByDescending(o => o.Id);

            // Pagination
            if (page == null || page < 1) page = 1;

            int pageSize = 5;
            int totalPages = 0;
            decimal count = await query.CountAsync();
            totalPages = (int)Math.Ceiling(count / pageSize); // total pages of orders

            query = query.Skip((int)(page - 1) * pageSize).Take(pageSize);

            var orders = await query.ToListAsync();

            foreach (var order in orders)
            {
                // get rid of the object cycle
                foreach (var item in order.OrderItems)
                {
                    item.Order = null!;
                }
                order.User.Password = ""; 
            }
            var response = new
            {
                Total = totalPages,
                PageSize = pageSize,
                Page = page,
                Orders = orders,
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            int userId = JWTReader.GetUserId(User);
            string userRole = _userRepository.Find(u => u.Id == userId)?.Result.Role??"";
            Order? order = null;
            if (userRole == "admin")
            {
                order = await _orderRepository.query().Include(o => o.User)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).
                    FirstOrDefaultAsync(o => o.Id == id);
            }
            else
            {
                order = await _orderRepository.query().Include(o => o.User)
                    .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            if (order == null) return NotFound();

            foreach (var item in order.OrderItems)
            {
                item.Order = null!;
            }
            order.User.Password = ""??"";

            return Ok(order);
        }

        [Authorize(Roles ="admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, string? paymentStatus, string? orderStatus)
        {
            if (paymentStatus == null && orderStatus == null)
            {
                ModelState.AddModelError("Update Error", "There is no thing tp update");
                return BadRequest(ModelState);
            }

            if (paymentStatus != null && !OrderHelper.PaymentStatuses.Contains(paymentStatus))
            {
                // the payment status is not valid
                ModelState.AddModelError("Payment Status", "The Payment Status is not valid");
                return BadRequest(ModelState);
            }

            if (orderStatus != null && !OrderHelper.OrderStatuses.Contains(orderStatus))
            {
                // the payment status is not valid
                ModelState.AddModelError("Payment Status", "The Payment Status is not valid");
                return BadRequest(ModelState);
            }

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (orderStatus != null)
            {
                order.OrderStatus = orderStatus;
            }

            if (paymentStatus != null)
            {
                order.PaymentStatus = paymentStatus;
            }

            await _orderRepository.UpdateAsync(order);

            return Ok(order);
        }

        [Authorize(Roles ="admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            await _orderRepository.DeleteAsync(order);
            return Ok();
        }


    
    }
}
