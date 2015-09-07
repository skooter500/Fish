using Gamelogic.Diagnostics;
using UnityEngine;

namespace Gamelogic
{
	/// <summary>
	/// Methods for additional math functions.
	/// </summary>
	[Version(1, 4)]
	public static class GLMathf 
	{
		public static float Wlerp01(float v1, float v2, float t)
		{
			GLDebug.Assert(InRange(v1, 0, 1), "v1 is not in [0, 1)");
			GLDebug.Assert(InRange(v2, 0, 1), "v2 is not in [0, 1)");

			if (Mathf.Abs(v1 - v2) <= 0.5f)
			{
				return Mathf.Lerp(v1, v2, t);
			}
			else if (v1 <= v2)
			{
				return Wrap01(Mathf.Lerp(v1 + 1, v2, t));
			}
			else
			{
				return Wrap01(Mathf.Lerp(v1, v2 + 1, t));
			}
		}

		public static bool InRange01(float value)
		{
			return InRange(value, 0, 1);
		}

		public static bool InRange(float value, float closedLeft, float openRight)
		{
			return value >= closedLeft && value < openRight;
		}

		public static float Wrap01(float value)
		{
			int n = Mathf.FloorToInt(value);
			float result = value - n;

			GLDebug.Assert(InRange01(result), "result is not in [0, 1)");
			return result;
		}
	}
}