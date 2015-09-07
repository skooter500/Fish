namespace Gamelogic
{
	/// <summary>
	/// Generates elements with frequencies that are different for each element, 
	/// and also depends on the previsouly generated elements. 
	/// </summary>
	[Version(1, 4)]
	public class MarkovChain2IntGenerator : IIntGenerator
	{
		private int lastSymbol;
		private readonly FrequencyIntGenerator[] generators;

		/// <summary>Constructs a new MarkovChain2IntGenerator
		/// </summary>
		/// <param name="frequencies">The conditional frequencies for the elements to generate,
		/// where frequencies[m][n] is the relative prob of generating n given m was generated 
		/// the last time </param>
		public MarkovChain2IntGenerator(float[][] frequencies)
		{
			int symbolCount = frequencies.Length;

			var initialFrequencies = new float[symbolCount];
			generators = new FrequencyIntGenerator[symbolCount];

			for (int i = 0; i < symbolCount; i++)
			{
				for (int j = 0; j < symbolCount; j++)
				{
					initialFrequencies[j] += frequencies[i][j];
				}

				generators[i] = new FrequencyIntGenerator(frequencies[i]);
			}

			var initialGenerator = new FrequencyIntGenerator(initialFrequencies);
			lastSymbol = initialGenerator.Next();
		}

		public int Next()
		{
			var nextSymbol = generators[lastSymbol].Next();
			lastSymbol = nextSymbol;
			return nextSymbol;
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}