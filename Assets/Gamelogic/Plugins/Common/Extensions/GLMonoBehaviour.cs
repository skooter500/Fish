#if UNITY_WINRT
#undef GL_SUPPORTS_REFLECTION //Just in case
#else
#define GL_SUPPORTS_REFLECTION
#endif

using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Gamelogic
{
	/// <summary>
	/// Provides some additional functions for MonoBehaviour.
	/// </summary>
	[Version(1)]
	public class GLMonoBehaviour : MonoBehaviour
	{
		

		#region Typesafe instatiation
#if !UNITY_5
		/**
			Instantiates an object.
		*/
		public static T Instantiate<T>(T obj) where T : Component
		{
			return (T) Object.Instantiate(obj);
		}
#endif

		/**
			Instantiates an object at the 
			given position in the given orientation.
		*/
		public static T Instantiate<T>(T obj, Vector3 position, Quaternion rotation) where T : Component
		{
			var newObj = Instantiate<T>(obj);

			newObj.transform.position = position;
			newObj.transform.rotation = rotation;

			return newObj;
		}

		/**
			Instantiates an object and attaches it to the given root. 
		*/
		public static T Instantiate<T>(T obj, GameObject root) where T : Component
		{
			var newObj = (T)Object.Instantiate(obj);
			newObj.transform.parent = root.transform;
			newObj.transform.ResetLocal();

			return newObj;
		}

		/**
			Instantiates an object, attaches it to the given root, and
			sets the local position and rotation.
		*/
		public static T Instantiate<T>(T obj, GameObject root, Vector3 localPosition, Quaternion localRotation) where T : Component
		{
			var newObj = Instantiate<T>(obj);

			newObj.transform.parent = root.transform;

			newObj.transform.localPosition = localPosition;
			newObj.transform.localRotation = localRotation;
			newObj.transform.ResetScale();

			return newObj;
		}


		/**
			Instantiates a GameObject.
		*/
		public static GameObject Instantiate(GameObject obj)
		{
			return (GameObject) Object.Instantiate(obj);
		}


		/**
			Instantiates a GameObject at the 
			given position in the given orientation.
		*/
		public static GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation)
		{
			var newObj = (GameObject) Object.Instantiate(obj, position, rotation);

			return newObj;
		}

		/**
			Instantiates a GameObject and parents it to the root.
		*/
		public static GameObject Instantiate(GameObject obj, GameObject root)
		{
			var newObject = (GameObject)Object.Instantiate(obj);
			newObject.transform.parent = root.transform;
			newObject.transform.ResetLocal();

			return newObject;
		}

		/**
			Instantiates a GameObject, attaches it to the given root, and
			sets the local position and rotation.
		*/
		public static GameObject Instantiate(GameObject obj, GameObject root, Vector3 localPosition, Quaternion localRotation)
		{
			var newObj = (GameObject)Object.Instantiate(obj);

			newObj.transform.parent = newObj.transform;
			newObj.transform.localPosition = localPosition;
			newObj.transform.localRotation = localRotation;
			newObj.transform.ResetScale();

			return newObj;
		}
		#endregion

		#region Find
		/**
			Similar to FindObjectsOfType, except that it looks for components
			that implement a specific interface.
		*/
		public static List<I> FindObjectsOfInterface<I>() where I : class
		{
			var monoBehaviours = FindObjectsOfType<MonoBehaviour>();

			return monoBehaviours.Select(behaviour => behaviour.GetComponent(typeof (I))).OfType<I>().ToList();
		}
		#endregion
	}

	///<summary>
	/// Provides useful extension methods for MonoBehaviours.
	/// </summary>
	[Version(1)]
	public static class MonoBehaviourExtensions
	{
		#region Cloning
		/// <summary>
		/// Clones an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T Clone<T>(this T obj) where T:MonoBehaviour
		{
			return GLMonoBehaviour.Instantiate<T>(obj);
		}

		/// <summary>
		/// Clones an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static List<T> Clone<T>(this T obj, int count) where T : MonoBehaviour
		{
			var list = new List<T>();

			for (int i = 0; i < count; i++)
			{
				list.Add(obj.Clone<T>());
			}

			return list;
		}
		#endregion

#if GL_SUPPORTS_REFLECTION
		#region Typesafe, but potentially slow, methods for scheduling

		/**
			Invokes the given action after the given amount of time.

			The action must be a method of the calling class.
		*/
		public static void Invoke(this MonoBehaviour component, System.Action action, float time)
		{
			component.Invoke(action.Method.Name, time);
		}

		/**
			Invokes the given action after the given amount of time, and repeats the 
			action after every repeatTime seconds.

			The action must be a method of the calling class.
		*/
		public static void InvokeRepeating(this MonoBehaviour component, System.Action action, float time, float repeatTime)
		{
			component.InvokeRepeating(action.Method.Name, time, repeatTime);
		}

		/**
			Invokes an action after a random time between the minimum and 
			maximum times given.
		*/
		public static void InvokeRandom(this MonoBehaviour component, System.Action action, float minTime, float maxTime)
		{
			var time = UnityEngine.Random.value * (maxTime - minTime) + minTime;

			component.Invoke(action, time);
		}

		/**
			Cancels the action if it was scheduled.
		*/
		public static void CancelInvoke(this MonoBehaviour component, System.Action action)
		{
			component.CancelInvoke(action.Method.Name);
		}

		/**
			Returns whether an invoke is pending on an action.
		*/
		public static bool IsInvoking(this MonoBehaviour component, System.Action action)
		{
			return component.IsInvoking(action.Method.Name);
		}

		#endregion
#endif

		#region Children
		public static GameObject FindChild(this MonoBehaviour component, string childName)
		{
			return component.transform.FindChild(childName).gameObject;
		}

		public static GameObject FindChild(this MonoBehaviour component, string childName, bool recursive)
		{
			if (recursive) return component.FindChild(childName);

			return FindChildRecursively(component.transform, childName);
		}

		private static GameObject FindChildRecursively(Transform target, string childName)
		{
			if (target.name == childName) return target.gameObject;

			for (var i = 0; i < target.childCount; ++i)
			{
				var result = FindChildRecursively(target.GetChild(i), childName);

				if (result != null) return result;
			}

			return null;
		}

		/**
			Finds a component of the type s2 in on the same object, or on a child down the hierarchy. This method also works
			in the editor and when the game object is inactive.

			@version_e_1_1

		*/
		public static T GetComponentInChildrenAlways<T>(this MonoBehaviour component) where T : MonoBehaviour
		{
			foreach (var child in component.transform.SelfAndAllChildren())
			{
				var componentInChild = child.GetComponent<T>();

				if (componentInChild != null)
				{
					return componentInChild;
				}
			}

			return null;
		}

		/**
			Finds all components of the type s2 on the same object and on a children down the hierarchy. This method also works
			in the editor and when the game object is inactive.

			@version_e_1_1
		*/
		public static T[] GetComponentsInChildrenAlways<T>(this MonoBehaviour component) where T : MonoBehaviour
		{
			var components = new List<T>();

			foreach (var child in component.transform.SelfAndAllChildren())
			{
				var componentsInChild = child.GetComponents<T>();

				if (componentsInChild != null)
				{
					components.AddRange(componentsInChild);
				}
			}

			return components.ToArray();
		}
		#endregion

		#region Components
		/**
			The same as GetComponent, but logs an error if the component is not found.
		*/
		public static T GetRequiredComponent<T>(this MonoBehaviour thisComponent) where T : Component
		{
			var component = thisComponent.GetComponent<T>();

			if (component == null)
			{
				Debug.LogError("Expected to find component of type " + typeof(T) + " but found none", thisComponent.gameObject);
			}

			return component;
		}

		/**
			Gets an attached component that implements the interface of the type parameter.
		*/
		public static I GetInterfaceComponent<I>(this MonoBehaviour thisComponent) where I : class
		{
			return thisComponent.GetComponent(typeof(I)) as I;
		}

		#endregion
	}
}