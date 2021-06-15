//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/
// ReSharper disable InconsistentNaming
namespace DapperLinqToQuery.Utilities.Enums
{
    public enum JoinTypes
    {
		/// <summary>
		/// This will not generate any join
		/// </summary>
		NONE,
		InnerJoin,
		LeftJoin,
		LeftOuterJoin,
		RightJoin,
		RightOuterJoin,
		FullOuterJoin
    }
}
