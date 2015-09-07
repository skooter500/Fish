using System;

namespace Gamelogic
{
	/// <summary>
	/// An attribute to mark API versions.
	/// </summary>
	[Version(1, 4)]
	[AttributeUsage(AttributeTargets.All)]
	public class VersionAttribute : Attribute
	{
		/// <summary>
		/// The main version number of this element.
		/// </summary>
		public int MainVersion { get; private set; }
		/// <summary>
		/// The sub version version number of this element.
		/// </summary>
		public int SubVersion { get; private set; }

		/// <summary>
		/// The sub sub version of this element.
		/// </summary>
		public int SubSubVersion { get; private set; }

		public VersionAttribute(int mainVersion, int subVersion  = 0, int subSubVerion = 0)
		{
			MainVersion = mainVersion;
			SubVersion = subVersion;
			SubSubVersion = subSubVerion;
		}
	}
}
