using System.Linq;
using LinqToQueryExtensions.Utilities.Enums;
using LinqToQueryExtensions.Where;

namespace LinqToQueryExtensions.Base
{
	public class BaseWhere<TModel>
	{
		private readonly WhereBuilder<TModel> _whereBuilder;

		public BaseWhere(WhereBuilder<TModel> whereBuilder)
		{
			_whereBuilder = whereBuilder;
		}

		public OperatorBuilder<TModel> EqualsTo(object value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.EQUALSTO;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> NotEqualsTo(object value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.NOTEQUALSTO;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> GreaterThan(object value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition =ConditionTypes.GREATERTHEN;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> GreaterThanEqualsTo(object value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.GREATERTHENEQUALSTO;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> LessThan(object value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.LESSTHEN;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> LessThanEqualsTo(object value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.LESSTHENEQUALSTO;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> Contains(object value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.CONTAINS;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> NotContains(object value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.NOTCONTAINS;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> In(string[] value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.IN;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}
		public OperatorBuilder<TModel> NotIn(string[] value)
		{
			var column = _whereBuilder.Conditions.Last();
			column.Value = value;
			column.Condition = ConditionTypes.NOTIN;
			return new OperatorBuilder<TModel>(_whereBuilder);
		}

	}
}