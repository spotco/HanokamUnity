using UnityEngine;
using System.Collections.Generic;

public struct DelayedAction {
	public float _time_left;
	public System.Action<GameEngineScene> _callback;
	public void run_and_cleanup(GameEngineScene g) {
		_callback(g);
		_callback = null;
	}
}

public class DelayActionQueue : SPGameUpdateable {
	private List<DelayedAction> _queue = new List<DelayedAction>();
	public static DelayActionQueue cons() {
		return (new DelayActionQueue());
	}
	public void i_update(GameEngineScene g) {
		for (int i = _queue.Count-1; i >= 0; i--) {
			DelayedAction itr = _queue[i];
			itr._time_left -= SPUtil.dt_scale_get();
			if (itr._time_left <= 0) {
				itr.run_and_cleanup(g);
				_queue.RemoveAt(i);
			}
		}
	}
	public void enqueue_action(DelayedAction action) {
		_queue.Add(action);
	}
}
