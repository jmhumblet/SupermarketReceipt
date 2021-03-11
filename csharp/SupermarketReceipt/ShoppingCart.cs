using System.Collections.Generic;
using System.Linq;

namespace SupermarketReceipt
{
    public class ShoppingCart
    {
        private readonly List<ProductQuantity> _items = new List<ProductQuantity>();
        private readonly Dictionary<Product, double> _productQuantities = new Dictionary<Product, double>();


        public List<ProductQuantity> GetItems()
        {
            return new List<ProductQuantity>(_items);
        }

        public void AddItem(Product product)
        {
            AddItemQuantity(product, 1.0);
        }


        public void AddItemQuantity(Product product, double quantity)
        {
            _items.Add(new ProductQuantity(product, quantity));
            if (_productQuantities.ContainsKey(product))
            {
                var newAmount = _productQuantities[product] + quantity;
                _productQuantities[product] = newAmount;
            }
            else
            {
                _productQuantities.Add(product, quantity);
            }
        }

        public void HandleOffers(Receipt receipt, Dictionary<Product, Offer> offers, SupermarketCatalog catalog)
        {
            foreach (var p in _productQuantities.Keys.Where(offers.ContainsKey))
            {
                var quantity = _productQuantities[p];
                var quantityAsInt = (int) quantity;

                var offer = offers[p];
                var unitPrice = catalog.GetUnitPrice(p);
                Discount discount = null;

                switch (offer.OfferType)
                {
                    case SpecialOfferType.ThreeForTwo:
                        const int quantityPerOfferThreeForTwo = 3;
                        
                        var numberOfXsThreeForTwo = quantityAsInt / quantityPerOfferThreeForTwo;
                        if (offer.OfferType == SpecialOfferType.ThreeForTwo && quantityAsInt > 2)
                        {
                            var discountAmount = quantity * unitPrice - (numberOfXsThreeForTwo * 2 * unitPrice + quantityAsInt % 3 * unitPrice);
                            discount = new Discount(p, "3 for 2", -discountAmount);
                        }
                        break;
                    
                    case SpecialOfferType.TwoForAmount:
                    {
                        const int quantityPerOfferTwoForAmount = 2;
                        if (quantityAsInt >= 2)
                        {
                            var total = offer.Argument * (quantityAsInt / quantityPerOfferTwoForAmount) + quantityAsInt % 2 * unitPrice;
                            var discountN = unitPrice * quantity - total;
                            discount = new Discount(p, "2 for " + offer.Argument, -discountN);
                        }

                        break;
                    }
                    case SpecialOfferType.FiveForAmount:
                        const int quantityPerOfferFiveForAmount = 5;

                        var numberOfXsFiveForAmount = quantityAsInt / quantityPerOfferFiveForAmount;


                        if (offer.OfferType == SpecialOfferType.FiveForAmount && quantityAsInt >= 5)
                        {
                            var discountTotal = unitPrice * quantity - (offer.Argument * numberOfXsFiveForAmount + quantityAsInt % 5 * unitPrice);
                            discount = new Discount(p, quantityPerOfferFiveForAmount + " for " + offer.Argument, -discountTotal);
                        }
                        break;

                    case SpecialOfferType.TenPercentDiscount:
                        discount = new Discount(p, offer.Argument + "% off", -quantity * unitPrice * offer.Argument / 100.0);
                        break;
                }
                
                if (discount != null)
                    receipt.AddDiscount(discount);
            }
        }
    }
}