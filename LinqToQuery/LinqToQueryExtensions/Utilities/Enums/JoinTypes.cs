// ReSharper disable InconsistentNaming
namespace LinqToQueryExtensions.Utilities.Enums
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
