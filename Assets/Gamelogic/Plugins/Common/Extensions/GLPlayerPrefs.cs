using UnityEngine;

namespace Gamelogic
{

	/// <summary>
	/// An alternative to PlayerPrefs that provides methods 
	/// for setting bool and array preferences.
	/// </summary>
	[Version(1)]
	public class GLPlayerPrefs
	{
		private const string ScopeOperator = "::";
		private const string ArrayCountKey = "Count";
		private const string Array = "Array";

		public static void SetInt(string scope, string key, int val)
		{
			PlayerPrefs.SetInt(GetKey(scope, key), val);
		}

		public static int GetInt(string scope, string key)
		{
			return PlayerPrefs.GetInt(GetKey(scope, key));
		}

		public static void SetBool(string scope, string key, bool val)
		{
			PlayerPrefs.SetInt(GetKey(scope, key), val ? 1 : 0);
		}

		public static bool GetBool(string scope, string key)
		{
			return PlayerPrefs.GetInt(GetKey(scope, key)) == 1;
		}

		public static void SetFloat(string scope, string key, float val)
		{
			PlayerPrefs.SetFloat(GetKey(scope, key), val);
		}

		public static float GetFloat(string scope, string key)
		{
			return PlayerPrefs.GetFloat(GetKey(scope, key));
		}

		public static string GetString(string scope, string key)
		{
			return PlayerPrefs.GetString(GetKey(scope, key));
		}

		public static void SetString(string scope, string key, string value)
		{
			PlayerPrefs.SetString(GetKey(scope, key), value);
		}

		public static bool HasKey(string scope, string key)
		{
			return PlayerPrefs.HasKey(GetKey(scope, key));
		}

		private static string GetKey(string scope, string key)
		{
			return scope + ScopeOperator + key;
		}

		public static void SetIntArray(string scope, string key, int[] values)
		{
			//Add a value so that HasKey also works for arrays
			PlayerPrefs.SetString(GetKey(scope, key), Array);

			PlayerPrefs.SetInt(GetArrayCountKey(scope, key), values.Length);

			for (var i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetInt(GetArrayIndexKey(scope, key, i), values[i]);
			}
		}

		public static int[] GetIntArray(string scope, string key)
		{
			var count = PlayerPrefs.GetInt(GetArrayCountKey(scope, key));
			var values = new int[count];

			for (var i = 0; i < count; i++)
			{
				values[i] = PlayerPrefs.GetInt(GetArrayIndexKey(scope, key, i));
			}

			return values;
		}

		public static void SetFloatArray(string scope, string key, float[] values)
		{
			//Add a value so that HasKey also works for arrays
			PlayerPrefs.SetString(GetKey(scope, key), Array);

			PlayerPrefs.SetInt(GetArrayCountKey(scope, key), values.Length);

			for (var i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetFloat(GetArrayIndexKey(scope, key, i), values[i]);
			}
		}

		public static float[] GetFloatArray(string scope, string key)
		{
			var count = PlayerPrefs.GetInt(GetArrayCountKey(scope, key));

			var values = new float[count];

			for (var i = 0; i < count; i++)
			{
				values[i] = PlayerPrefs.GetFloat(GetArrayIndexKey(scope, key, i));
			}

			return values;
		}

		public static void SetBoolArray(string scope, string key, bool[] values)
		{
			//Add a value so that HasKey also works for arrays
			PlayerPrefs.SetString(GetKey(scope, key), Array);

			PlayerPrefs.SetInt(GetArrayCountKey(scope, key), values.Length);

			for (var i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetInt(GetArrayIndexKey(scope, key, i), values[i] ? 1 : 0);
			}
		}

		public static bool[] GetBoolArray(string scope, string key)
		{
			var count = PlayerPrefs.GetInt(GetArrayCountKey(scope, key));

			var values = new bool[count];

			for (var i = 0; i < count; i++)
			{
				values[i] = PlayerPrefs.GetInt(GetArrayIndexKey(scope, key, i)) != 0;
			}

			return values;
		}

		public static void SetStringArray(string scope, string key, string[] values)
		{
			//Add a value so that HasKey also works for arrays
			PlayerPrefs.SetString(GetKey(scope, key), Array);

			PlayerPrefs.SetInt(GetArrayCountKey(scope, key), values.Length);

			for (var i = 0; i < values.Length; i++)
			{
				PlayerPrefs.SetString(GetArrayIndexKey(scope, key, i), values[i]);
			}
		}

		public static string[] GetStringArray(string scope, string key)
		{
			var count = PlayerPrefs.GetInt(GetArrayCountKey(scope, key));

			var values = new string[count];

			for (var i = 0; i < count; i++)
			{
				values[i] = PlayerPrefs.GetString(GetArrayIndexKey(scope, key, i));
			}

			return values;
		}

		private static string GetArrayIndexKey(string scope, string key, int index)
		{
			return scope + ScopeOperator + key + ScopeOperator + index;
		}

		private static string GetArrayCountKey(string scope, string key)
		{
			return scope + ScopeOperator + key + ScopeOperator + ArrayCountKey;
		}

		public static void DeleteArray(string scope, string key)
		{
			var count = PlayerPrefs.GetInt(GetArrayCountKey(scope, key));

			for (int i = 0; i < count; i++)
			{
				PlayerPrefs.DeleteKey(GetArrayIndexKey(scope, key, i));
			}
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}
	}

}