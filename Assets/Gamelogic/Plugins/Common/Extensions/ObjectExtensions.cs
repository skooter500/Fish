using System;

namespace Gamelogic
{
	/// <summary>
	/// Provides extensions for objects.
	/// </summary>
	[Version(1, 4)]
	public static class ObjectExtensions
	{
		/// <summary>
		/// Throws a NullReferenceException if the object is null.
		/// </summary>
		/// <param name="o">An object to check.</param>
		/// <param name="name">The name of the variable this
		/// methods is called on.</param>
		/// <exception cref="NullReferenceException"></exception>
		public static void ThrowIfNull(this object o, string name)
		{
			if(o == null) throw new NullReferenceException(name);
		}

		/// <summary>
		/// Throws a ArgumentOutOfRange exception if the integer is negative.
		/// </summary>
		/// <param name="n">The integer to check.</param>
		/// <param name="name">The name of the variable.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[Version(1,4, 1)]
		public static void TrowIfNegative(this int n, string name)
		{
			if(n < 0) throw new ArgumentOutOfRangeException(name, n, "argument cannot be negative");
		}

		/// <summary>
		/// Throws a ArgumentOutOfRange exception if the float is negative.
		/// </summary>
		/// <param name="x">The float to check.</param>
		/// <param name="name">The name of the variable.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[Version(1, 4, 1)]
		public static void TrowIfNegative(float x, string name)
		{
			if (x < 0) throw new ArgumentOutOfRangeException(name, x, "argument cannot be negative");
		}
	}
}
