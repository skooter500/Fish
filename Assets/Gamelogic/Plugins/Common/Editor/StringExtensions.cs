using System.Text.RegularExpressions;

namespace Gamelogic.Editor
{
	/// <summary>
	/// Class for string extensions.
	/// </summary>
	//  From: http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/791963c8-9e20-4e9e-b184-f0e592b943b0/
	public static class StringExtensions
	{
		/// <summary>
		/// Takes a string in camel case, splis it into separate words, and 
		/// captilizes each word.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string SplitCamelCase(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}

			string camelCase = Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
			string firstLetter = camelCase.Substring(0, 1).ToUpper();

			if (str.Length > 1)
			{
				string rest = camelCase.Substring(1);

				return firstLetter + rest;
			}
			else
			{
				return firstLetter;
			}
		}
	}
}