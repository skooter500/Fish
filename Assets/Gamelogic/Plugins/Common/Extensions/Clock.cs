using System.Collections.Generic;

namespace Gamelogic
{
	/// <summary>
	/// To use this clock, instantiate it, call Reset with the right time value, and call Update it each frame.
	/// 
	/// Any class that wants to be notified of events need to implement the IClockListener interface,
	/// and subscribe to events using the AddListener method. A listener can be removed with the RemoveListener
	/// event.
	/// 
	/// Clocks can be paused independently of Time.timeScale using the Pause method (and started again using Unpause).
	/// </summary>
	[Version(1, 2)]
	public class Clock
	{
		private float time;
		private int timeInSeconds;
		private readonly List<IClockListener> listeners;

		public bool IsPaused
		{
			get; private set;
		}

		public bool IsDone
		{
			get; private set;
		}

		public float Time
		{
			get
			{
				return time;
			}
		}

		public int TimeInSeconds
		{
			get
			{
				return timeInSeconds;
			}
		}

		public Clock()
		{
			listeners = new List<IClockListener>();
			IsPaused = true;
			Reset(0);
		}

		public void AddClockListener(IClockListener listener)
		{
			listeners.Add(listener);
		}

		public void RemoveClockListener(IClockListener listener)
		{
			listeners.Remove(listener);
		}

		public void Reset(float startTime)
		{
			time = startTime;
			IsDone = false;
			CheckIfTimeInSecondsChanged();
		}

		public void Unpause()
		{
			IsPaused = false;
		}

		public void Pause()
		{
			IsPaused = true;
		}

		public void Update()
		{
			if (IsPaused) return;

			if (IsDone) return;
		
			time -= UnityEngine.Time.deltaTime;

			CheckIfTimeInSecondsChanged();

			if (time <= 0)
			{
				time = 0;
				IsDone = true;

				foreach (var listener in listeners)
				{
					listener.OnTimeOut();
				}
			}
		}

		private void CheckIfTimeInSecondsChanged()
		{
			var newTimeInSeonds = (int)time;

			if (newTimeInSeonds == timeInSeconds) return;
		
			timeInSeconds = newTimeInSeonds;

			foreach (var listener in listeners)
			{
				listener.OnSecondsChanged(timeInSeconds);
			}
		}
	}

	/// <summary>
	/// A class that can listen to clock events.
	/// </summary>
	public interface IClockListener
	{
		/// <summary>
		/// Called whenever the seconds of the clock changed.
		/// </summary>
		/// <remarks>This is useful when the time is displayed and only needs to be updated
		/// not more freuqnelty than once a second.
		/// </remarks>
		/// <param name="seconds"></param>
		void OnSecondsChanged(int seconds);

		/// <summary>
		/// Called when a clock's time runs out.
		/// </summary>
		void OnTimeOut();
	}
}