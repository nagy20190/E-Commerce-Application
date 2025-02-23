using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceApplication.DAL.Services
{
    public class OrderHelper
    {

        // Shiping Fee
        public static decimal ShipingFee { get; } = 5; // like 5$

        public static Dictionary<string, string> PayementMethods { get; } = new()
        {
            { "Cash", "Cash on Delivery" },
            { "Paypal", "Paypal" },
            { "Credit Card", "Credit Card" }
        };

        public static List<string> PaymentStatuses { get; } = new()
        {
            "Pending", "Accepted", "Canceled"
        };


        public static List<string> OrderStatuses { get; } = new()
        {
            "Created", "Accepted", "Canceled", "Shipped", "Delivered", "Returned"
        };



        /*
        * Receives a string of product identifiers, separated by '-'
        * Example: 9-9-7-9-6
        * 
        * Returns a list of pairs (dictionary):
        *     - the pair name is the product ID
        *     - the pair value is the product quantity
        * Example:
        * {
        *     9: 3,
        *     7: 1,
        *     6: 1
        * }
        */
        public static Dictionary<int, int> getProductDictionary(string prdctIdentifier)
        {
            var productDictionary = new Dictionary<int, int>();
            if (prdctIdentifier.Length > 0)
            {
                // if not empty
                string[] productIdArray = prdctIdentifier.Split('-');

                foreach (string productId in productIdArray)
                {
                    try
                    {
                        int id = int.Parse(productId);
                        if (productDictionary.ContainsKey(id)) // check if the Dictionary contains valid int
                        {
                            productDictionary[id] += 1;
                        }
                        else
                        {
                            productDictionary.Add(id, 1);
                        }
                    }
                    catch(Exception) 
                    {
                        
                    }
                }

            }
            return productDictionary;
        }


    }
}
