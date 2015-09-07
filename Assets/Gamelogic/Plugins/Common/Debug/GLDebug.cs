using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Gamelogic.Diagnostics
{
	/// <summary>
	/// Class that contains methods useful for debugging.
	/// All methods are only compiled if the DEBUG symbol is defined.
	/// </summary>
	public static class GLDebug
	{
		/// <summary>
		/// Check whether the condition is true, and print an error message if it is not.
		/// </summary>
		[Version(1, 2)]
		[Conditional("DEBUG")]
		public static void Assert(bool condition, string message, Object context=null)
		{
			if (!condition)
			{
				LogError("Assert failed", message, context);
			}
		}

		[Conditional("DEBUG")]
		public static void Log(object message, Object context = null)
		{
			Debug.Log(message, context);
		}

		[Conditional("DEBUG")]
		public static void LogWarning(object message, Object context = null)
		{
			Debug.LogWarning(message, context);
		}

		[Conditional("DEBUG")]
		public static void LogError(object message, Object context = null)
		{
			Debug.LogError(message, context);
		}

		[Conditional("DEBUG")]
		public static void Log(string type, object message, Object context = null)
		{
			Debug.Log(type + ": " + message, context);
		}

		[Conditional("DEBUG")]
		public static void LogWarning(string type, object message, Object context = null)
		{
			Debug.LogWarning(type + ": " + message, context);
		}

		[Conditional("DEBUG")]
		public static void LogError(string type, object message, Object context = null)
		{
			Debug.LogError(type + ": " + message, context);
		}
	}
}