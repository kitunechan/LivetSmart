namespace LivetSmart.Messaging {
	/// <summary>
	/// <see cref="WindowCommandMessage"/>で使用する、Windowが遷移すべき状態を表します。
	/// </summary>
	public enum WindowCommand {
		/// <summary>
		/// Windowを閉じます。
		/// </summary>
		Close,
		/// <summary>
		/// Windowを最大化します。
		/// </summary>
		Maximize,
		/// <summary>
		/// Windowを最小化します。
		/// </summary>
		Minimize,
		/// <summary>
		/// Windowを通常状態にします。
		/// </summary>
		Normal,
		/// <summary>
		/// Windowをアクティブにします。
		/// </summary>
		Active,

		/// <summary>
		/// WindowのDialogResultをTrueにします。
		/// </summary>
		ResultOK,

		/// <summary>
		/// WindowのDialogResultをFalseにします。
		/// </summary>
		ResultCancel,
	}
}
