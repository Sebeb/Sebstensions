using System;
using Sirenix.OdinInspector;


[Serializable, InlineProperty]
public class StatusUpdater
{
	[Serializable]
	public struct Status
	{
		[HideLabel]
		public State state;
		[HideLabel]
		public string message;

		public Status(State state, string message)
		{
			this.state = state;
			this.message = message;
		}
	}

	public enum State
	{
		Initial, InProgress, Success,
		Failure
	}

	public State currentState => currentStatus.state;
	public Status currentStatus
	{
		get => _currentStatus;
		set
		{
			_currentStatus = value;
			onUpdate?.Invoke(value);
		}
	}
	[ShowInInspector, InlineProperty, HideLabel, ReadOnly]
	private Status _currentStatus;

	public Action<Status> onUpdate;
}