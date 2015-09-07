using System;

namespace Gamelogic
{
	/// <summary>
	/// Use to mark classes and structs that are abstract, but cannot be implemented
	/// becuase Unity cannot handle it.
	/// </summary>
	[Version(1, 4)]
	[AttributeUsage(AttributeTargets.Class |
					AttributeTargets.Struct | 
					AttributeTargets.Method |
					AttributeTargets.Property)]
	public sealed class AbstractAttribute : Attribute
	{
	}
}
