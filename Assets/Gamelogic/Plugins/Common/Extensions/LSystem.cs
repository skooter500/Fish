using System.Collections.Generic;
using System.Linq;

namespace Gamelogic
{
	/// <summary>
	/// A lightweight implementation of an L-system.
	/// </summary>
	/// <typeparam name="TSymbol">This type must be 
	/// comparable using ==, or you should feed an IEqualityComparer.</typeparam>
	[Version(1, 4, 1)]
	public class LSystem <TSymbol>
	{
		private readonly Dictionary<TSymbol, List<TSymbol>> rewriteRules;

		/// <summary>
		/// Creates a new empty LSystem.
		/// </summary>
		public LSystem()
		{
			rewriteRules = new Dictionary<TSymbol, List<TSymbol>>();
		}

		/// <summary>
		/// Constrcuts a new empty L-System that will use the given comparer to compare symbols.
		/// </summary>
		/// <param name="comparer">The comprarer to use to compare symbols.</param>
		public LSystem(IEqualityComparer<TSymbol> comparer)
		{
			rewriteRules = new Dictionary<TSymbol, List<TSymbol>>(comparer);
		}

		/// <summary>
		/// Adds a new rewrite rule to the system.
		/// </summary>
		/// <param name="symbol"></param>
		/// <param name="replacement"></param>
		public void AddRewriteRule(TSymbol symbol, IEnumerable<TSymbol> replacement)
		{
			replacement.ThrowIfNull("replacement");

			rewriteRules[symbol] = replacement.ToList();
		}

		/// <summary>
		/// Rewrites a string using the rewrite rules.
		/// </summary>
		/// <param name="str">The string to rewrite.</param>
		/// <returns>The rewritten string.</returns>
		public IEnumerable<TSymbol> Rewrite(IEnumerable<TSymbol> str)
		{
			str.ThrowIfNull("str");

			return str.SelectMany<TSymbol, TSymbol>(Rewrite);
		}

		/// <summary>
		/// Performs a rewrite on a string using the rewrite rules n times.
		/// </summary>
		/// <param name="str">The string to rewrite.</param>
		/// <param name="n">The number of times to rewrite it.</param>
		/// <returns>The rewritten string.</returns>
		public IEnumerable<TSymbol> Rewrite(IEnumerable<TSymbol> str, int n)
		{
			str.ThrowIfNull("str");
			n.TrowIfNegative("n");

			return n == 0 ? str.ToList() : Rewrite(str, n - 1).SelectMany<TSymbol, TSymbol>(Rewrite);
		}
	
		private IEnumerable<TSymbol> Rewrite(TSymbol symbol)
		{
			if (rewriteRules.ContainsKey(symbol))
			{
				foreach (var newSymbol in rewriteRules[symbol])
				{
					yield return newSymbol;
				}
			}
			else yield return symbol;
		}
	}
}