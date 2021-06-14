//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToQueryCore.Utilities
{
	public static class ExpressionExtensions
	{
		public static string GetMemberInfo<TModel,TType>(this Expression<Func<TModel, TType>> expression)
		{
			var exp = GetMember(expression);
			return exp;
		}

		private static string GetMember(this Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentException(
					"The expression cannot be null.");
			}

			if (expression is MemberExpression)
			{
				// Reference type property or field
				var memberExpression =
					(MemberExpression)expression;
				return memberExpression.Member.Name;
			}

			if (expression is MethodCallExpression)
			{
				// Reference type method
				var methodCallExpression =
					(MethodCallExpression)expression;
				return methodCallExpression.Method.Name;
			}
			if (expression is LambdaExpression)
			{
				// Reference type method
				var methodCallExpression =
					(LambdaExpression)expression;

				if (methodCallExpression.Body.NodeType == ExpressionType.Convert ||
					methodCallExpression.Body.NodeType == ExpressionType.ConvertChecked)
				{
					var unaryExpression = methodCallExpression.Body as UnaryExpression;
					return GetMemberName(unaryExpression);
				}
				if (methodCallExpression.Body.NodeType == ExpressionType.Constant)
				{
					var constExp = methodCallExpression.Body as ConstantExpression;
					if (constExp != null)
						return constExp.Value.ToString();
				}
				var exp = methodCallExpression.Body as MemberExpression;
				if (exp != null)
					return exp.Member.Name;
			}

			if (expression is UnaryExpression)
			{
				// Property, field of method returning value type
				var unaryExpression = (UnaryExpression)expression;
				return GetMemberName(unaryExpression);
			}

			throw new ArgumentException("Invalid expression");
		}
		private static string GetMemberName(
			UnaryExpression unaryExpression)
		{
			if (unaryExpression.Operand is MethodCallExpression)
			{
				var methodExpression =
					(MethodCallExpression)unaryExpression.Operand;
				return methodExpression.Method.Name;
			}

			return ((MemberExpression)unaryExpression.Operand)
				.Member.Name;
		}
		public static object GetAttributeValue<TModel, TType>(string colName)
		{
			var model = typeof(TModel);
			var property = model.GetProperties().FirstOrDefault(x => x.Name == colName);
			if (property == null) return null;

			return property.GetCustomAttribute(typeof(TType));
		}
		public static object GetMemberType<TModel>(this Expression<Func<TModel, object>> expression,string colName)
		{
			var model = typeof(TModel);

			var property = model.GetProperty(colName);
			if (property == null) return null;

			return property.PropertyType.Name;
		}
	}
}