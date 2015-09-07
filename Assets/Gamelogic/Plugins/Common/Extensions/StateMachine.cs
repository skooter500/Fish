using System;
using System.Collections.Generic;

namespace Gamelogic
{
	/// <summary>
	/// A lightweight state machine.
	/// </summary>
	/// <remarks>
	/// 	<para>To use it: </para>
	/// 	<list type="bullet">
	/// 		<item>
	/// 			<description>Define your own label. Enums are probably the best
	/// choice.</description>
	/// 		</item>
	/// 		<item>
	/// 			<description>Construct a new state machine, typically in a
	/// MonoBehaviour's Start method.</description>
	/// 		</item>
	/// 		<item>
	/// 			<description>Add the various states with the appropriate delegates.
	/// </description>
	/// 		</item>
	/// 		<item>
	/// 			<description>Call the state machine's Update method from the
	/// MonoBehaviour's Update method.</description>
	/// 		</item>
	/// 		<item>
	/// 			<description>Set the CurrentState property on the state machine to transition.
	/// (You can eitther set it from one of the state delegates, or from anywhere else.
	/// </description>
	/// 		</item>
	/// 	</list>
	/// 	<para>When a state is changed, the OnStop on exisiting state is called, then the
	/// OnStart of the new state, and from there on OnUpdate of the new state each time
	/// the update is called.</para>
	/// </remarks>
	/// <typeparam name="TLabel">The label type of this state machine. Enums are common,
	/// but strings or int are other posibilities.</typeparam>
	[Version(1)]
	public class StateMachine<TLabel>
	{
		private class State
		{
			public readonly Action onStart;
			public readonly Action onUpdate;
			public readonly Action onStop;
			public readonly TLabel label;

			public State(TLabel label, Action onStart, Action onUpdate, Action onStop)
			{
				this.onStart = onStart;
				this.onUpdate = onUpdate;
				this.onStop = onStop;
				this.label = label;
			}
		}

		private readonly Dictionary<TLabel, State> stateDictionary;
		private State currentState;

		/// <summary>
		/// Returns the label of the current state.
		/// </summary>
		public TLabel CurrentState
		{
			get { return currentState.label; }

			[Version(1, 2)]
			set { ChangeState(value); }
		}

		/// <summary>
		/// Constructs a new StateMachine.
		/// </summary>
		public StateMachine()
		{
			stateDictionary = new Dictionary<TLabel, State>();
		}

		/// <summary>
		/// 	Adds a state, and the delegates that should run 
		/// when the state starts, stops, 
		/// and when the state machine is updated.
		/// 
		/// Any delegate can be null, and wont be executed.
		/// </summary>
		/// <param name="label"></param>
		/// <param name="onStart"></param>
		/// <param name="onUpdate"></param>
		/// <param name="onStop"></param>
		public void AddState(TLabel label, Action onStart, Action onUpdate, Action onStop)
		{
			stateDictionary[label] = new State(label, onStart, onUpdate, onStop);
		}

		public void AddState(TLabel label, Action onStart, Action onUpdate)
		{
			AddState(label, onStart, onUpdate, null);
		}

		public void AddState(TLabel label, Action onStart)
		{
			AddState(label, onStart, null);
		}

		public void AddState(TLabel label)
		{
			AddState(label, null);
		}
		
		/// <summary>
		/// Adds a sub state machine for the given state.
		///
		/// The sub state machine need not be updated, as long as this state machine
		/// is being updated.
		/// </summary>
		/// <typeparam name="TSubstateLabel"></typeparam>
		/// <param name="label"></param>
		/// <param name="substateMachine"></param>
		/// <param name="substate"></param>
		[Version(1, 4)]
		public void AddState<TSubstateLabel>(TLabel label, StateMachine<TSubstateLabel> substateMachine,
			TSubstateLabel substate)
		{
			AddState(
				label,
				() => substateMachine.ChangeState(substate),
				substateMachine.Update);
		}

		/// <summary>
		/// Changes the state from the existing one to the state with the given label.
		/// 
		/// It is legal (and useful) to transition to the same state, in which case the 
		/// current state's onStop action is called, the onstart ation is called, and the
		/// state keeps on updating as before. The behviour is exactly the same as switching to
		/// a new state.
		/// </summary>
		/// <param name="newState"></param>
		private void ChangeState(TLabel newState)
		{
			if (currentState != null && currentState.onStop != null)
			{
				currentState.onStop();
			}

			currentState = stateDictionary[newState];

			if (currentState.onStart != null)
			{
				currentState.onStart();
			}
		}

		/// <summary>
		/// This method should be called every frame. 
		/// </summary>
		public void Update()
		{
			if (currentState != null && currentState.onUpdate != null)
			{
				currentState.onUpdate();
			}
		}

		/// <summary>
		/// This method should be called every frame. 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return CurrentState.ToString();
		}
	}
}