using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Colors
{
	/// <summary>
	/// The base class for all nodes in a <see cref="Graph"/>.
	/// </summary>
	[Version(1, 1)]
	[Abstract]
	public class Node: ScriptableObject
	{
		#region Fields
		// TODO: Make this private and provide a readonly property
		/// <exclude />
		[HideInInspector]
		public int id;

		/// <summary>
		/// The rectangle this node occupies when displayed visually.
		/// </summary>
		[HideInInspector]
		public Rect rect = new Rect(50, 50, 100, 0);
		#endregion

		#region Properties
		/// <summary>
		/// A list of nodes that are the inputs for this node.
		/// </summary>
		[Abstract]
		public virtual IEnumerable<Node> Inputs
		{
			get { throw new NotImplementedException("Node of type " + GetType() + "should override this property"); }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Adds a node to this nodes inputs.
		/// </summary>
		/// <param name="input"></param>
		[Abstract]
		public virtual void AddNodeInput(Node input)
		{
			throw new NotImplementedException("Node of type " + GetType() + "should override this method");
		}

		/// <summary>
		/// Removes the given node from the list of input nodes of this node.
		/// </summary>
		/// <param name="input"></param>
		[Abstract]
		public virtual void RemoveNodeInput(Node input)
		{
			throw new NotImplementedException("Node of type " + GetType() + "should override this method");
		}

		/// <summary>
		/// Updates a nodes outputs without recomputing internal (possibly random) values.
		/// </summary>
		[Abstract]
		public virtual void UpdateStatic()
		{
			throw new NotImplementedException("Node of type " + GetType() + "should override this method");
		}

		/// <summary>
		/// Recomputes a nodes internal values that are independent of the inputs.
		/// </summary>
		[Abstract]
		public virtual void Recompute()
		{
			throw new NotImplementedException("Node of type " + GetType() + "should override this method");
		}
		#endregion

		#region Message Handlers
		public void OnEnable()
		{
			hideFlags = HideFlags.HideInHierarchy;
			name = "(" + id + ") " + GetType().Name;
		}
		#endregion
	}

	/// <summary>
	/// A node with typed output.
	/// </summary>
	/// <typeparam name="TOutput">The type of the output for this node.</typeparam>
	[Abstract]
	public class Node<TOutput> : Node
	{
		[HideInInspector] [SerializeField] private List<TOutput> output;

		/// <summary>
		/// A list of outputs for this node.
		/// </summary>
		public List<TOutput> Output
		{
			get { return output; }
			protected set { output = value; }
		}
	}

	/// <summary>
	/// A Node with typed inputs and outputs.
	/// </summary>
	/// <typeparam name="TInput"></typeparam>
	/// <typeparam name="TOutput"></typeparam>
	[Abstract]
	public class Node<TInput, TOutput> : Node<TOutput>
	{
		[HideInInspector] public List<Node> inputNodes = new List<Node>();

		/// <summary>
		/// Total number of items coming into the node.
		/// </summary>
		protected int InputItemCount
		{
			get { return inputNodes.Cast<Node<TOutput>>().Sum(n => n.Output.Count); }
		}

		/// <summary>
		/// Calculates a list of output from a given list of input.
		/// </summary>
		/// <param name="input">The input values to base the computation on.</param>
		/// <returns>The list of output values.</returns>
		[Abstract]
		public virtual List<TOutput> Execute(IEnumerable<TInput> input)
		{
			throw new NotImplementedException();
		}

		public override void UpdateStatic()
		{
			foreach (var node in inputNodes)
			{
				node.UpdateStatic();
			}

			var inputs = inputNodes.Cast<Node<TInput>>().Select(node => node.Output).SelectMany(x => x);
			Output = Execute(inputs);
		}

		public sealed override IEnumerable<Node> Inputs
		{
			get { return inputNodes; }
		}

		public sealed override void AddNodeInput(Node input)
		{
			var inputNode = input as Node<TInput>;

			if (inputNode != null)
			{
				inputNodes.Add(inputNode);
			}
		}

		public sealed override void RemoveNodeInput(Node input)
		{
			inputNodes.Remove(input);
		}
	}
}