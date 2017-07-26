//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/
using System.Linq;

namespace LinqToQueryExtensions.Where
{
	public class OperatorBuilder<TModel>
	{
		private readonly WhereBuilder<TModel> _baseWhere;

		public OperatorBuilder(WhereBuilder<TModel> baseWhere)
		{
			_baseWhere = baseWhere;
		}

		public WhereBuilder<TModel> And()
		{
			var column = _baseWhere.Conditions.Last();
			column.Operator = " AND ";
			return _baseWhere;
		}
		public WhereBuilder<TModel> Or()
		{
			var column = _baseWhere.Conditions.Last();
			column.Operator = " OR ";
			return _baseWhere;
		}
	}
}