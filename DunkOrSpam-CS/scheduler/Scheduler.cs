namespace DunkOrSpam_CS.scheduler {

	/// <summary>
	/// Quick and dirty scheduling utility
	/// </summary>
	public class Scheduler {
		
		private const int TickRate = 20;

		private static Scheduler? instance;
		public static Scheduler Instance => instance ?? new Scheduler(); // Wait why did I make this a singleton?

		private readonly Dictionary<int, TaskInfo> taskQueue = new();
		private Dictionary<int, TaskInfo> asyncQueue = new(); // Not gonna bother with this right now

		private long tick;
		private int nextHandle;
		private bool ticking;

		private int NextHandle => nextHandle++;

		private Scheduler() {
			instance = this;
			StartTicking();
		}

		/// <summary>
		/// Tick
		/// </summary>
		private async void StartTicking() {
			ticking = true;

			while (ticking) {
				await Task.Delay(TickRate);
				Tick();
			}
			
		}

		public void StopTicking() {
			ticking = false;
		}

		/// <summary>
		/// Ticks the scheduler
		/// Called 50x per second
		/// </summary>
		private void Tick() {
			if (taskQueue.Count > 0) {
				IEnumerable<TaskInfo> toRemove = taskQueue.ToList().Where(task => task.Value.NextRun <= tick)
					.Select(task => {
						TaskInfo info = task.Value;
						info.Action();

						if (info.Repeating) {
							info.NextRun = tick + info.Period;
							taskQueue[info.Handle] = info; // Concurrent modification?
						}

						return info;
					})
					.Where(task => !task.Repeating);

				foreach (var taskInfo in toRemove) {
					CancelTask(taskInfo.Handle); // Not concurrent modification lmao
				}
			}

			tick++;
		}

		/// <summary>
		/// Schedules an action to occur after a set amount of ticks (1/50th of a second)
		/// </summary>
		/// <param name="action">The action to perform</param>
		/// <param name="delay">The delay (in ticks) before performing the Action</param>
		/// <returns></returns>
		public int ScheduleSync(Action action, long delay) {
			TaskInfo info = new(action, tick + delay, 0L, false, NextHandle);
			taskQueue[info.Handle] = info;

			return info.Handle;
		}

		/// <summary>
		/// Don't know enough about concurrency to implement this yet lmao
		/// </summary>
		/// <param name="action"></param>
		/// <param name="delay"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public int ScheduleAsync(Action action, long delay) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Schedules an action to begin repeating every Peroid ticks, starting after an optional given delay
		/// </summary>
		/// <param name="action">The action to perform</param>
		/// <param name="period">The amount of ticks between each execution of the given Action</param>
		/// <param name="delay">(Optional) The delay (in ticks) before performing the Action for the first time</param>
		/// <returns></returns>
		public int RepeatSync(Action action, long period, long delay = 0) {
			TaskInfo info = new(action, tick + delay, period <= 0 ? 1 : period, true, NextHandle);
			taskQueue[info.Handle] = info;

			return info.Handle;
		}

		/// <summary>
		/// Ditto ^
		/// </summary>
		/// <param name="action"></param>
		/// <param name="period"></param>
		/// <param name="delay"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public int RepeatAsync(Action action, long period, long delay = 0) {
			throw new NotImplementedException();
		}

		public void CancelTask(int handle) {
			taskQueue.Remove(handle); // TODO: Update logic to be thread safe whenever async is implemented
		}

		private struct TaskInfo {

			public readonly Action Action;
			public long NextRun;
			public readonly long Period;
			public readonly bool Repeating;
			public readonly int Handle;

			public TaskInfo(Action action, long nextRun, long period, bool repeating, int handle) {
				Action = action;
				NextRun = nextRun;
				Period = period;
				Repeating = repeating;
				Handle = handle;
			}

		}
		
	}

}
