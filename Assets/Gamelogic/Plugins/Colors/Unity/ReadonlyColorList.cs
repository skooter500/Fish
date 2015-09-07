using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// Used to mark color lists that are displayed in the 
	/// inspector but cannot be edited.
	/// </summary>
	[Version(1, 1)]
	public class ReadonlyColorList:PropertyAttribute 
	{
	}
}