namespace Gamelogic
{
	/// <summary>
	/// A type less Generator that is the base of all generators.
	/// </summary>
	[Version(1, 2)]
	public interface IGenerator
	{
		/// <summary>
		/// Generates an element of typ object.
		/// </summary>
		/// <returns></returns>
		object Next();
	}

	/// <summary>
	/// Classes that implement this interface can produce a new element of the 
	/// given type. Generators differ from enumerables in that they generally
	/// are infinite, and don't have a "start" position.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Version(1, 2)]
	public interface IGenerator<out T> :IGenerator
	{
		/// <summary>
		/// Generates the next element.
		/// </summary>
		/// <returns></returns>
		new T Next();
	}
}