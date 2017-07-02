namespace NancyWebhookProducer
{
    public class StockItem
    {
        public string Sku { get; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public StockItem(string sku)
        {
            Sku = sku;
            Description = Sku;
        }

        public StockItem(string sku, int quantity)
            : this(sku)
        {
            Quantity = quantity;
        }

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj))
        //    {
        //        return false;
        //    }
        //    if (ReferenceEquals(this, obj))
        //    {
        //        return true;
        //    }
        //    if (obj.GetType() != GetType())
        //    {
        //        return false;
        //    }
        //    var other = (StockItem)obj;
        //    return string.Equals(Sku, other.Sku, StringComparison.OrdinalIgnoreCase)
        //           && Quantity == other.Quantity;
        //}

        //public override int GetHashCode() =>
        //    Sku.GetHashCode();
    }
}
