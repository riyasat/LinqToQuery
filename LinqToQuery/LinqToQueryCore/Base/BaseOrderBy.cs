//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;
using System.Linq.Expressions;
using LinqToQueryCore.Order;
using LinqToQueryCore.Utilities;
using LinqToQueryCore.Utilities.Enums;

namespace LinqToQueryCore.Base
{
	public class BaseOrderBy<TModel>
	{
		private readonly OrderByBuilder<TModel> _orderByBuilder;

		public BaseOrderBy(OrderByBuilder<TModel> orderByBuilder)
		{
			_orderByBuilder = orderByBuilder;
		}

		public BaseOrderBy<TModel> ThenByColumn(Expression<Func<TModel, object>> expression, OrderByDirection orderByDirection)
		{
			var colName = expression.GetMemberInfo();
			var column = _orderByBuilder.GenerateOrderBy<TModel>(colName, orderByDirection);
			_orderByBuilder.OrderAs.Add(column);
			return this;
		}
		public BaseOrderBy<TModel> ThenByColumn<TJoin>(Expression<Func<TJoin, object>> expression, OrderByDirection orderByDirection)
		{
			var colName = expression.GetMemberInfo();
			var column = _orderByBuilder.GenerateOrderBy<TJoin>(colName, orderByDirection);
			_orderByBuilder.OrderAs.Add(column);
			return this;
		}

		
	}
}