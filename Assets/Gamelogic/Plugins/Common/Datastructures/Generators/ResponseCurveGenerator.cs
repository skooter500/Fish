namespace Gamelogic
{
	/// <summary>
	/// A generator that uses a response curve to generate elements.
	/// </summary>
	/// <typeparam name="T">The type of element to generate. The response 
	/// curve must be of the same type.</typeparam>
	[Version(1, 4)]
	public class ResponseCurveGenerator<T> : IGenerator<T>
	{
		private readonly ResponseCurveBase<T> responseCurve;
		private readonly IGenerator<float> floatGenerator;

		/// <summary>
		/// Creates a new ResponseCurveGenerator with the given 
		/// response curve.
		/// </summary>
		/// <param name="responseCurve"></param>
		public ResponseCurveGenerator(ResponseCurveBase<T> responseCurve):
			this(responseCurve, new UniformFloatGenerator())
		{}

		/// <summary>
		/// Creates a new ResponseCurveGenerator with the given 
		/// response curve.
		/// </summary>
		/// <param name="responseCurve"></param>
		/// <param name="floatGenerator"></param>
		public ResponseCurveGenerator(ResponseCurveBase<T> responseCurve, IGenerator<float> floatGenerator)
		{
			this.responseCurve = responseCurve;
			this.floatGenerator = floatGenerator;
		}

		public T Next()
		{
			return responseCurve[floatGenerator.Next()];
		}

		object IGenerator.Next()
		{
			return Next();
		}
	}
}