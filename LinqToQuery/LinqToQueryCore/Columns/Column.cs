//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using LinqToQueryCore.Utilities.Enums;

namespace LinqToQueryCore.Columns
{
	internal class Column
	{

		public Column(string columnName)
		{
			ColumnName = columnName;
		}
		public Column(string columnName, string columnAlias)
		{
			ColumnName = columnName;
			ColumnAlias = columnAlias;
		}

		public string ColumnName { get; set; }
		public string ColumnAlias { get; set; }
		public string TableName { get; internal set; }
		public string TableAlias { get; set; }
		public string Operator { get; set; }
		public AggregationMethods AggregationMethod { get; set; }
		public ConditionTypes Condition { get; set; }
		public JoinTypes JoinType { get; set; }
		public OrderByDirection OrderBy { get; set; }
		public object Value { get; set; }
		public Column JoinWithColumn { get; set; }
		public string DistinctBy { get; set; }
		public bool HasDistinctBy => string.IsNullOrWhiteSpace(DistinctBy) == false;
	}
}