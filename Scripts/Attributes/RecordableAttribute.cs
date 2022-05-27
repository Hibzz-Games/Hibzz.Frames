using System;

namespace Hibzz.Frames
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class RecordableAttribute : Attribute
	{
		public RecordableAttribute() { }
	}
}
