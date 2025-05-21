namespace Chapeau.Models
{
    public class PaymentItemModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public bool IsAlcoholic { get; set; }
        public int TableNr { get; set; }

        public PaymentItemModel(string name, decimal price, int count, bool isAlcoholic, int tableNr)
        {
            Name = name;
            Price = price;
            Count = count;
            IsAlcoholic = isAlcoholic;
            TableNr = tableNr;
        }
        //calculates the amount for a menu item
        public decimal Amount
        {
            get
            {
                return Price * Count;
            }
        }
        //VAT percentage based on the bool value
        public int VATPercent
        {
            get
            {
                return IsAlcoholic ? 21 : 9;
            }
        }

    }
}
