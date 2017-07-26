//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/
using System;
using System.Text.RegularExpressions;
using LinqToQueryExtensions.Utilities.Enums;

namespace LinqToQueryExtensions.Columns
{
	internal static class ColumnExtension
	{

		internal static string GetSelect(this Column type)
		{
			if (string.IsNullOrWhiteSpace(type.ColumnAlias))
			{
				return $" {type.TableAlias}.{type.ColumnName}";
			}
			return $" {type.TableAlias}.{type.ColumnName} AS {type.ColumnAlias}";
		}

		internal static string GetJoin(this Column type)
		{
			if (type.JoinType == JoinTypes.NONE) return "";

			if (type.JoinWithColumn == null) throw new Exception("Join is not correct");
			string joinSyntax = Regex.Replace(type.JoinType.ToString(), "(\\B[A-Z])", " $1").ToUpper();

			return $" {joinSyntax} {type.TableName} AS {type.TableAlias}" +
					$" ON  {type.JoinWithColumn.TableAlias}.{type.JoinWithColumn.ColumnName} = {type.TableAlias}.{type.ColumnName}";
		}

		internal static string GetWhere(this Column type)
		{
			return $" {type.GetOperator()} {type.Operator}";
		}
		internal static string GetOrderBy(this Column type)
		{
			return $" {type.TableName}.{type.ColumnName} {type.OrderBy.ToString()}";
		}
		internal static string GetSetValue(this Column type)
		{
			return $" {type.TableName}.{type.ColumnName} = @UP{type.ColumnName}";
		}
		private static string GetOperator(this Column type)
		{
			switch (type.Condition)
			{
				case ConditionTypes.CONTAINS:
					return $"{type.TableName}.{type.ColumnName} LIKE '%'+@{type.ColumnName}+'%'";
				case ConditionTypes.NOTCONTAINS:
					return $"{type.TableName}.{type.ColumnName} NOT LIKE '%'+@{type.ColumnName}+'%'";
				case ConditionTypes.NOTEQUALSTO:
					return $"{type.TableName}.{type.ColumnName} != @{type.ColumnName}";
				case ConditionTypes.GREATERTHEN:
					return $"{type.TableName}.{type.ColumnName} > @{type.ColumnName}";
				case ConditionTypes.GREATERTHENEQUALSTO:
					return $"{type.TableName}.{type.ColumnName} >= @{type.ColumnName}";
				case ConditionTypes.LESSTHEN:
					return $"{type.TableName}.{type.ColumnName} < @{type.ColumnName}";
				case ConditionTypes.LESSTHENEQUALSTO:
					return $"{type.TableName}.{type.ColumnName} <= @{type.ColumnName}";
				case ConditionTypes.IN:
					string[] inValues = type.Value as string[];
					var inVariableName = "";
					if (inValues != null)
					{
						for (int i = 0; i < inValues.Length; i++)
						{
							if (string.IsNullOrWhiteSpace(inVariableName) == false)
							{
								inVariableName += ",";
							}
							inVariableName += $"@{type.ColumnName}{i}";
						}
						return $"{type.TableName}.{type.ColumnName} IN ({inVariableName})";
					}
					return "";
				case ConditionTypes.NOTIN:
					string[] notInValues = type.Value as string[];
					var variableName = "";
					if (notInValues != null)
					{
						for (int i = 0; i < notInValues.Length; i++)
						{
							if (string.IsNullOrWhiteSpace(variableName) == false)
							{
								variableName += ",";
							}
							variableName += $"@{type.ColumnName}{i}";
						}
						return $"{type.TableName}.{type.ColumnName} NOT IN ({variableName})";
					}
					return "";
				default:
					return $"{type.TableName}.{type.ColumnName} = @{type.ColumnName}";
			}
		}
	}
}