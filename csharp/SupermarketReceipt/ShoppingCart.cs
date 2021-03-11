using System;
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

                var offer = offers[p];
                var unitPrice = catalog.GetUnitPrice(p);
                
                var discount = DefineDiscount(offer, quantity, unitPrice, p);

                if (discount != null)
                    receipt.AddDiscount(discount);
            }
        }

        private static Discount DefineDiscount(Offer offer, double quantity, double unitPrice, Product product)
        {
            switch (offer.OfferType)
            {
                case SpecialOfferType.ThreeForTwo:
                {
                    return DefineThreeForTwoDiscount(offer, quantity, unitPrice, product);
                }
                case SpecialOfferType.TwoForAmount:
                {
                    return DefineTwoForAmountDiscount(offer, quantity, unitPrice, product);
                }
                case SpecialOfferType.FiveForAmount:
                {
                    return DefineFiveForAmountDiscount(offer, quantity, unitPrice, product);
                }
                case SpecialOfferType.TenPercentDiscount:
                {
                    return DefineTenPercentDiscount(offer, quantity, unitPrice, product);
                }
                default:
                {
                    return null;
                }
            }
        }

        private static Discount DefineTenPercentDiscount(Offer offer, double quantity, double unitPrice, Product product)
        {
            return new Discount(product, offer.Argument + "% off", -quantity * unitPrice * offer.Argument / 100.0);
        }

        private static Discount DefineFiveForAmountDiscount(Offer offer, double quantity, double unitPrice, Product product)
        {
            const int quantityPerOfferFiveForAmount = 5;

            var numberOfXsFiveForAmount = (int) quantity / quantityPerOfferFiveForAmount;


            if (offer.OfferType == SpecialOfferType.FiveForAmount && (int) quantity >= 5)
            {
                var discountTotal = unitPrice * quantity -
                                    (offer.Argument * numberOfXsFiveForAmount + (int) quantity % 5 * unitPrice);
                return new Discount(product, quantityPerOfferFiveForAmount + " for " + offer.Argument, -discountTotal);
            }

            return null;
        }

        private static Discount DefineTwoForAmountDiscount(Offer offer, double quantity, double unitPrice, Product product)
        {
            const int quantityPerOfferTwoForAmount = 2;
            if ((int) quantity >= 2)
            {
                var total = offer.Argument * ((int) quantity / quantityPerOfferTwoForAmount) +
                            (int) quantity % 2 * unitPrice;
                var discountN = unitPrice * quantity - total;
                return new Discount(product, "2 for " + offer.Argument, -discountN);
            }

            return null;
        }

        private static Discount DefineThreeForTwoDiscount(Offer offer, double quantity, double unitPrice, Product product)
        {
            const int quantityPerOfferThreeForTwo = 3;

            var numberOfXsThreeForTwo = (int) quantity / quantityPerOfferThreeForTwo;
            if (offer.OfferType == SpecialOfferType.ThreeForTwo && (int) quantity > 2)
            {
                var discountAmount = quantity * unitPrice -
                                     (numberOfXsThreeForTwo * 2 * unitPrice + (int) quantity % 3 * unitPrice);
                return new Discount(product, "3 for 2", -discountAmount);
            }

            return null;
        }
    }
}