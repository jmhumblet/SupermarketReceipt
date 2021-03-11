namespace SupermarketReceipt
{
    public enum SpecialOfferType
    {
        ThreeForTwo,
        TenPercentDiscount,
        TwoForAmount,
        FiveForAmount
    }

    public class Offer
    {
        private Product _product;

        public Offer(SpecialOfferType offerType, Product product, double argument)
        {
            OfferType = offerType;
            Argument = argument;
            _product = product;
        }

        public SpecialOfferType OfferType { get; }
        public double Argument { get; }

        public Discount DefineDiscount(double quantity, double unitPrice, Product product)
        {
            switch (OfferType)
            {
                case SpecialOfferType.ThreeForTwo:
                {
                    return DefineThreeForTwoDiscount(this, quantity, unitPrice, product);
                }
                case SpecialOfferType.TwoForAmount:
                {
                    return DefineTwoForAmountDiscount(this, quantity, unitPrice, product);
                }
                case SpecialOfferType.FiveForAmount:
                {
                    return DefineFiveForAmountDiscount(this, quantity, unitPrice, product);
                }
                case SpecialOfferType.TenPercentDiscount:
                {
                    return DefineTenPercentDiscount(this, quantity, unitPrice, product);
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

        public static Offer For(SpecialOfferType offerType, Product product, double argument)
        {
            return new Offer(offerType, product, argument);
        }
    }
}