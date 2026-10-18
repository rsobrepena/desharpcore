using System;
using DesharpCore.Core;
using DesharpCore.Producers;

namespace DesharpCore {
	/// <summary>
	/// Desharp service implementation for dependency injection in .NET Core / .NET 10 applications.
	/// </summary>
	public class DesharpService : IDesharpService, IDesharp {
		/// <summary>
		/// Initializes a new instance of <see cref="DesharpService"/>.
		/// </summary>
		public DesharpService() {
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DesharpService"/> with custom configuration.
		/// </summary>
		/// <param name="config">Debug configuration to apply.</param>
		public DesharpService(DebugConfig config) {
			Debug.Configure(config);
		}

		/// <inheritdoc />
		public FireDump Fire {
			get { return Debug.Fire; }
			set { Debug.Fire = value; }
		}

		/// <inheritdoc />
		public bool Enabled(bool? enabled = null) {
			return Debug.Enabled(enabled);
		}

		/// <inheritdoc />
		public void Configure(DebugConfig cfg) {
			Debug.Configure(cfg);
		}

		/// <inheritdoc />
		public Exception GetLastError() {
			return Debug.GetLastError();
		}

		/// <inheritdoc />
		public double GetProcessingTime() {
			return Debug.GetProcessingTime();
		}

		/// <inheritdoc />
		public double Timer(string name = null, bool returnTimerSeconds = false, Level logLevel = Level.DEBUG) {
			return Debug.Timer(name, returnTimerSeconds, logLevel);
		}

		/// <inheritdoc />
		public void Assert(bool assertion, string description = "", Level logLevel = Level.DEBUG) {
			Debug.Assert(assertion, description, logLevel);
		}

		/// <inheritdoc />
		public void Stop() {
			Debug.Stop();
		}

		/// <inheritdoc />
		public string Dump(Exception exception = null, DumpOptions? options = null) {
			return Debug.Dump(exception, options);
		}

		/// <inheritdoc />
		public string Dump(params object[] args) {
			return Debug.Dump(args);
		}

		/// <inheritdoc />
		public string Dump(object obj, DumpOptions? options = null) {
			return Debug.Dump(obj, options);
		}

		/// <inheritdoc />
		public string DumpAndDie(object obj = null, DumpOptions? options = null) {
			return Debug.DumpAndDie(obj, options);
		}

		/// <inheritdoc />
		public void Log(Exception exception = null) {
			Debug.Log(exception);
		}

		/// <inheritdoc />
		public void Log(object obj = null, Level level = Level.INFO, int maxDepth = 0, int maxLength = 0) {
			Debug.Log(obj, level, maxDepth, maxLength);
		}
	}
}
