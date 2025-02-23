using E_CommerceApplication.BLL.DTO;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.DAL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IBaseRepository<Product> _productRepository;
        public CartController(IBaseRepository<Product> productRepository)
        {
            _productRepository = productRepository;
            
        }

        [HttpGet("GetPaymentMethods")]
        public IActionResult GetPaymentMethods()
        {
            return Ok(OrderHelper.PayementMethods);
        }


        [HttpGet]
        public async Task<IActionResult> GetCart(string productIdentifiers)
        {
            CartDTO cartDTO = new CartDTO();
            cartDTO.items = new List<CartItemDTO>();
            cartDTO.SubTotal = 0;
            cartDTO.ShipingFee = OrderHelper.ShipingFee;
            cartDTO.TotalPrice = 0;

            var productDictionary = OrderHelper.getProductDictionary(productIdentifiers); // return list products and quantity of thim in the cart
            // for example :
            /*
             user input --> 9-9-5-6
            output -> 
            "5" : 1,
            "9" : 2,
            "6" : 1
             */

            foreach (var pair in productDictionary) // every pair has the poduct id and quantity
            {
                int proudctId = pair.Key; 
                var product = await _productRepository.Find(p => p.Id == proudctId);

                if (product == null)
                {
                    continue; // if not product with this id , go to next id
                }
                // making a list of catsItems to add it in the cartDTO
                CartItemDTO cartItemDTO = new CartItemDTO();
                cartItemDTO.product = product;
                cartItemDTO.Quantity = pair.Value;

                cartDTO.items.Add(cartItemDTO);  
                cartDTO.SubTotal += product.Price * pair.Value; // subTotal of the one product
                cartDTO.TotalPrice = cartDTO.SubTotal + cartDTO.ShipingFee;
            }

            return Ok(cartDTO);
        }
    }
}
