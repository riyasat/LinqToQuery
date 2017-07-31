using System.ComponentModel.DataAnnotations.Schema;

namespace LinqToQueryExample.Entities
{
	[Table("OrderItem")]
	public class OrderItem
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public float UnitPrice { get; set; }
		public int Quantity { get; set; }
	}
}
