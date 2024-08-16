namespace DunkOrSpam_CS.scheduler {
	
	/// <summary>
	/// Extension method for delegate type Action.
	/// Used to give an immediate follow-up action to a scheduled Action once completed.
	/// </summary>
	public static class ActionExtension {
		
		public static Action AndThen(this Action first, Action second) {
			return () => {
				first();
				second();
			};
		}
		
	}
	
}
