/// <summary>
///  Contains classes useful across all Gamelogic Unity libraries.
/// </summary>
namespace Gamelogic
{
	/**
		@mainpage_Extensions Gamelogic Extensions for Unity API Documentation

		This extension library contains a few light-weight utility classes and
		convenient extension methods for some Unity classes.	

		@version 1.2

		(Note, this library also comes with other %Gamelogic Unity plugins).

		The library contains:
			- A base class for creating Singletons from any MonoBehaviour. See @ref Singleton<T>.
			- A light-weight state machine class. See @ref StateMachine<TLabel>.
			- A monobehaviour class that defines generic methods for Instantiation. 
				See @ref GLMonoBehaviour.
			- A PlayerPrefs alternative that, in addition to the methods of the standard 
				PlayerPrefs, also defines methods for dealing with boolean types, and rays 
				of int, float, string and bool. See @ref GLPlayerPrefs.
			- Extensions for transform that allows easy setting of components of position, 
				scale and rotation separately. See @ref TransformExtensions.
			- Extensions for vectors to calculate the projection and rejection, and the 
				vector perpendicular (2D). See @ref VectorExtensions.
			- Extensions for Colors to get lighter or darker colors, get a color with 
				a given value, and get an opaque version of a given color. See @ref ColorExtensions.

		<h3>Changes</h3>
		Version 1.2
			- Added response curves for float, Vector2, Vector3, Vector4 and Color. See @ref ResponseCurveBase.
			- Added a @ref Clock class.
			- Added generators. See @ref IGenerator.
			- Added a @ref MinMaxFloat class with an appropriate property drawer.
			- Added NonNegative and Positive attributes, with appropriate property drawers.
			- Removed the ChangeSet method from StateMachine, and added writing capability to CurrentState.
			
			
		Version 1.1
			- Added GetCompoenentInChildrenAlways and GetCompoenentsInChildrenAlways methods.
			- Added a method SelfAndAllChildren that can be used to iterate over a sub tree 
				in the hierarchy.
	*/

	/// <summary>
	/// Contains classes useful for debugging.
	/// </summary>
	namespace Diagnostics { }

	/// <summary>
	/// Contains classes for Colors that run in the Unity editor.
	/// </summary>
	namespace Editor { }

	/// <summary>
	/// Internal API.
	/// </summary>
	namespace Editor.Internal { }

	/// <summary>
	/// Internal API.
	/// </summary>
	namespace Internal.BinaryHeap { }

	/// <summary>
	/// Internal API.
	/// </summary>
	namespace Internal.KDTree { }

}