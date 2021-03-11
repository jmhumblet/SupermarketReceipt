using System;

namespace SupermarketReceipt
{
    public enum SpecialOfferType
    {
        ThreeForTwo,
        TenPercentDiscount,
        TwoForAmount,
        FiveForAmount
    }

    public abstract class Offer
    {
        private Product _product;

        protected Offer(SpecialOfferType offerType, Product product, double argument)
        {
            OfferType = offerType;
            Argument = argument;
            _product = product;
        }

        public SpecialOfferType OfferType { get; }
        public double Argument { get; }

        public abstract Discount DefineDiscount(double quantity, double unitPrice, Product product);

        public static Offer For(SpecialOfferType offerType, Product product, double argument)
        {
            switch (offerType)
            {
                case SpecialOfferType.ThreeForTwo:
                    return new ThreeForTwoOffer(product, argument);
                case SpecialOfferType.TenPercentDiscount:
                    return new TenPercentDiscountOffer(product, argument);
                case SpecialOfferType.TwoForAmount:
                    return new TwoForAmountOffer(product, argument);
                case SpecialOfferType.FiveForAmount:
                    return new FiveForAmountOffer(product, argument);
                default:
                    return null;
            }
        }
    }

    class FiveForAmountOffer : Offer
    {
        public FiveForAmountOffer(Product product, double argument) 
            : base(SpecialOfferType.FiveForAmount, product, argument)
        {
        }

        public override Discount DefineDiscount(double quantity, double unitPrice, Product product)
        {
            const int quantityPerOfferFiveForAmount = 5;

            var numberOfXsFiveForAmount = (int) quantity / quantityPerOfferFiveForAmount;


            if (this.OfferType == SpecialOfferType.FiveForAmount && (int) quantity >= 5)
            {
                var discountTotal = unitPrice * quantity -
                                    (this.Argument * numberOfXsFiveForAmount + (int) quantity % 5 * unitPrice);
                return new Discount(product, quantityPerOfferFiveForAmount + " for " + this.Argument, -discountTotal);
            }

            return null;
        }
    }

    class TwoForAmountOffer : Offer
    {
        public TwoForAmountOffer(Product product, double argument) 
            : base(SpecialOfferType.TwoForAmount, product, argument)
        {
        }

        public override Discount DefineDiscount(double quantity, double unitPrice, Product product)
        {
            const int quantityPerOfferTwoForAmount = 2;
            if ((int) quantity >= 2)
            {
                var total = this.Argument * ((int) quantity / quantityPerOfferTwoForAmount) +
                            (int) quantity % 2 * unitPrice;
                var discountN = unitPrice * quantity - total;
                return new Discount(product, "2 for " + this.Argument, -discountN);
            }

            return null;
        }
    }

    class TenPercentDiscountOffer : Offer
    {
        public TenPercentDiscountOffer(Product product, double argument) 
            : base(SpecialOfferType.TenPercentDiscount, product, argument)
        {
        }

        public override Discount DefineDiscount(double quantity, double unitPrice, Product product)
        {
            return new Discount(product, Argument + "% off", -quantity * unitPrice * Argument / 100.0);
        }
    }

    class ThreeForTwoOffer : Offer
    {
        public ThreeForTwoOffer(Product product, double argument) 
            : base(SpecialOfferType.ThreeForTwo, product, argument)
        {
        }

        public override Discount DefineDiscount(double quantity, double unitPrice, Product product)
        {
            const int quantityPerOfferThreeForTwo = 3;

            var numberOfXsThreeForTwo = (int) quantity / quantityPerOfferThreeForTwo;
            if (this.OfferType == SpecialOfferType.ThreeForTwo && (int) quantity > 2)
            {
                var discountAmount = quantity * unitPrice -
                                     (numberOfXsThreeForTwo * 2 * unitPrice + (int) quantity % 3 * unitPrice);
                return new Discount(product, "3 for 2", -discountAmount);
            }

            return null;
        }
    }
}