using UnityEngine;


public abstract class SingletonMonoBehaviour<T> : CustomMono where T : SingletonMonoBehaviour<T>
{
	protected override void Assign() => SetInstance();

	private void SetInstance()
	{
		if (instance != null)
		{
			if (instance == this)
			{
				return;
			}

			Debug.LogError($"Multiple instances of {GetType()} found here...", this);
			Debug.LogError($"...and here", instance);
			return;
		}
		// Debug.Log($"Set instance {GetType()}", this);

		instance = this as T;
	}

	private static T FindInstance()
	{
		if (instance != null)
		{
			return instance;
		}

		instance = FindObjectOfType<T>(includeInactive: true);
		if (instance == null)
		{
			Debug.LogError($"No instance of {typeof(T)} found");
		}

		// else { Debug.Log($"Set instance {typeof(T)}", instance); }
		return instance;
	}

	// ReSharper disable once InconsistentNaming
	private static T instance;
	public static T _i => instance ? instance : FindInstance();
}