using PizzaPlace.Models.Types;

namespace PizzaPlace.Entities
{
    public class StockEntity
    {
        public long Id { get; set; }
        public StockType StockType { get; set; }
        public int Amount { get; set; }
    }
}
