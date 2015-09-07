using UnityEngine;

namespace Gamelogic
{
	/// <summary>
	/// Contains useful extensions for vectors.
	/// </summary>
	[Version(1)]
	public static class VectorExtensions
	{
		public static Vector3 To3DXZ(this Vector2 vector, float y)
		{
			return new Vector3(vector.x, y, vector.y);
		}

		public static Vector3 To3DXZ(this Vector2 vector)
		{
			return vector.To3DXZ(0);
		}

		public static Vector3 To3DXY(this Vector2 vector, float z)
		{
			return new Vector3(vector.x, vector.y, z);
		}

		public static Vector3 To3DXY(this Vector2 vector)
		{
			return vector.To3DXY(0);
		}

		public static Vector3 To3DYZ(this Vector2 vector, float x)
		{
			return new Vector3(x, vector.x, vector.y);
		}

		public static Vector3 To3DYZ(this Vector2 vector)
		{
			return vector.To3DYZ(0);
		}

		public static Vector2 To2DXZ(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.z);
		}

		public static Vector2 To2DXY(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		public static Vector2 To2DYZ(this Vector3 vector)
		{
			return new Vector2(vector.y, vector.z);
		}

		/// <summary>
		/// Returns the vector rotated 90 degrees counter-clockwise.
		/// </summary>
		/// <remarks>
		/// 	<para>The returned vector is always perpendicular to the given vector. </para>
		/// 	<para>The perp dot product can be caluclted using this: <c>var perpDotPorpduct = Vector2.Dot(v1.Perp(), v2);</c></para>
		/// </remarks>
		/// <param name="vector"></param>
		public static Vector2 Perp(this Vector2 vector)
		{
			return new Vector2(-vector.y, vector.x);
		}

		/// <summary>
		/// Returns the projection of this vector onto the given base.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		/// <returns></returns>
		public static Vector2 Proj(this Vector2 vector, Vector2 baseVector)
		{
			var direction = baseVector.normalized;
			var magnitude = Vector2.Dot(vector, direction);

			return direction * magnitude;
		}

		/// <summary>
		/// Returns the rejection of this vector onto the given base.
		/// </summary>
		/// <remarks>
		/// 	<para>The sum of a vector's projection and rejection on a base is equal to
		/// the original vector.</para>
		/// </remarks>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		public static Vector2 Rej(this Vector2 vector, Vector2 baseVector)
		{
			return vector - vector.Proj(baseVector);
		}

		/// <summary>
		/// Returns the projection of this vector onto the given base.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		/// <returns></returns>

		public static Vector3 Proj(this Vector3 vector, Vector3 baseVector)
		{
			var direction = baseVector.normalized;
			var magnitude = Vector2.Dot(vector, direction);

			return direction * magnitude;
		}

		/// <summary>
		/// Returns the rejection of this vector onto the given base.
		/// </summary>
		/// <remarks>
		/// 	<para>The sum of a vector's projection and rejection on a base is equal to
		/// the original vector.</para>
		/// </remarks>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		public static Vector3 Rej(this Vector3 vector, Vector3 baseVector)
		{
			return vector - vector.Proj(baseVector);
		}

		/// <summary>
		/// Returns the projection of this vector onto the given base.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		/// <returns></returns>
		public static Vector4 Proj(this Vector4 vector, Vector4 baseVector)
		{
			var direction = baseVector.normalized;
			var magnitude = Vector2.Dot(vector, direction);

			return direction * magnitude;
		}

		/// <summary>
		/// Returns the rejection of this vector onto the given base.
		/// The sum of a vector's projection and rejection on a base is
		/// equal to the original vector.
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="baseVector"></param>
		/// <returns></returns>
		public static Vector4 Rej(this Vector4 vector, Vector4 baseVector)
		{
			return vector - vector.Proj(baseVector);
		}

		public static Vector3 PerpXZ(this Vector3 v)
		{
			return new Vector3(-v.z, v.y, v.x);
		}

		public static Vector3 PerpXY(this Vector3 v)
		{
			return new Vector3(-v.y, v.x, v.z);
		}
	}
}