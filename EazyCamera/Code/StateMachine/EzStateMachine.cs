using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyCamera.Legacy
{
	public class EzStateMachine
	{
		public EzCameraState CurrentState { get; private set; }
		public EzCameraState PreviousState { get; private set; }

		public void UpdateState()
		{
			if (CurrentState != null)
			{
				CurrentState.UpdateState();
			}
		}

		public void UpdateStateFixed()
		{
			if (CurrentState != null)
			{
				CurrentState.UpdateStateFixed();
			}
		}

		public void LateUpdateState()
		{
			if (CurrentState != null)
			{
				CurrentState.LateUpdateState();
			}
		}

		public void SetCurrentState(EzCameraState newState)
		{
			if (CurrentState != null)
			{
				PreviousState = CurrentState;
				CurrentState.ExitState();
			}

			CurrentState = newState;

			if (CurrentState != null)
			{
				CurrentState.EnterState();
			}
		}
	}
}
