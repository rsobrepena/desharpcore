using System;
using DesharpCore.Core;
using DesharpCore.Producers;

namespace DesharpCore {
	/// <summary>
	/// Desharp debugging service interface for dependency injection.
	/// </summary>
	public interface IDesharp {
		/// <summary>
		/// Get or set current request FireDump to log anything into client browser console window through FirePHP extension.
		/// </summary>
		FireDump Fire { get; set; }

		/// <summary>
		/// Enable or disable variables dumping from application code environment for all threads.
		/// </summary>
		/// <param name="enabled"><c>true</c> to enable, <c>false</c> to disable, if no param defined, no changes will be made.</param>
		/// <returns>bool about enabled/disabled dumping state</returns>
		bool Enabled(bool? enabled = null);

		/// <summary>
		/// Configure Desharp assembly from running application environment and override any XML config settings or automatically detected settings.
		/// </summary>
		/// <param name="cfg">Specialized Desharp configuration collection - just create new instance with public fields of that.</param>
		void Configure(DebugConfig cfg);

		/// <summary>
		/// Return last uncaught Exception in request, mostly used in web applications by error page rendering process to know something about Exception before.
		/// </summary>
		/// <returns>Last Exception instance, including original exception call stack and possible inner exceptions.</returns>
		Exception GetLastError();

		/// <summary>
		/// Return spent request processing time for web applications or return application up time for all other platforms.
		/// </summary>
		/// <returns>Returned values is number of seconds with 3 decimal places after comma with milliseconds.</returns>
		double GetProcessingTime();

		/// <summary>
		/// Prints to output or into log file number of seconds from last timer call under called name in seconds with 3 floating point decimal spaces.
		/// </summary>
		/// <param name="name">Timer name, used as key to find last <c>DesharpCore.Debug.Timer(name);</c> call from internal dictionary to print the timespan in app output or log file.</param>
		/// <param name="returnTimerSeconds">If <c>true</c>, do not print or log timer value - only return the time span difference decimal value as result of this function.</param>
		/// <param name="logLevel">Use log level to specify log file used when dump printing into output is disabled, <c>"debug.log"</c> used by default.</param>
		/// <returns>If <c>returnTimerSeconds</c> param is <c>true</c>, return the time span difference decimal from the last DesharpCore.Debug.Timer(name); call under called <c>name</c> or return zero if <c>returnTimerSeconds</c> param is <c>false</c> (by default).</returns>
		double Timer(string name = null, bool returnTimerSeconds = false, Level logLevel = Level.DEBUG);

		/// <summary>
		/// Print to application output or log into file (if enabled) first param to be <c>true</c> or not and describe what was equal or not in first param by second param <c>message</c>.
		/// </summary>
		/// <param name="assertion">Comparison boolean to dump or log, way to compare things is up to you, you need to process it in method param space to send here boolean only.</param>
		/// <param name="description">Any text to describe previous comparison.</param>
		/// <param name="logLevel">Log level, used only when printing to output is disabled, <c>"default.log"</c> used by default.</param>
		void Assert(bool assertion, string description = "", Level logLevel = Level.DEBUG);

		/// <summary>
		/// Print current thread stack trace into application output and exit running application.
		/// </summary>
		void Stop();

		/// <summary>
		/// Dump exception instance to application output if output dumping is enabled.
		/// </summary>
		/// <param name="exception">Exception instance to dump.</param>
		/// <param name="options">Dump options collection (optional) - just create new instance with public fields of that.</param>
		/// <returns>Returns empty string if debug printing is disabled and also returns empty string if second param <c>DumpOptions.Return</c> is <c>false</c> (by default), but if true, return dumped exception as string.</returns>
		string Dump(Exception exception = null, DumpOptions? options = null);

		/// <summary>
		/// Dump any values to application output (in web applications into debug bar, in desktop applications into console).
		/// </summary>
		/// <param name="args">Infinite number of values to dump into application output.</param>
		/// <returns>All dumped values are in this function version always returned as dumped string.</returns>
		string Dump(params object[] args);

		/// <summary>
		/// Dump any type value to application output (in web applications into debug bar, in desktop applications into console).
		/// </summary>
		/// <param name="obj">Any type value to dump into application output.</param>
		/// <param name="options">Dump options collection (optional) - just create new instance with public fields of that.</param>
		/// <returns>Returns empty string by default or dumped variable string if you specify in second argument <c>DumpOptions.Return</c> to be <c>true</c>.</returns>
		string Dump(object obj, DumpOptions? options = null);

		/// <summary>
		/// Dump any type value to direct application output (not into web request debug bar in web applications!) and stop request/thread (in web applications dump into direct response body, in desktop applications into console).
		/// </summary>
		/// <param name="obj">Any type value to dump into application output.</param>
		/// <param name="options">Dump options collection (optional) - just create new instance with public fields of that.</param>
		/// <returns>Returns empty string by default or dumped variable string if you specify in second argument <c>DumpOptions.Return</c> to be <c>true</c>.</returns>
		string DumpAndDie(object obj = null, DumpOptions? options = null);

		/// <summary>
		/// Log exception instance as dumped string into <c>exceptions.log|exceptions.html</c> file.
		/// </summary>
		/// <param name="exception">Exception instance to print.</param>
		void Log(Exception exception = null);

		/// <summary>
		/// Log any type value to application <c>*.log|*.html</c> file, specified by level param.
		/// </summary>
		/// <param name="obj">Any type value to dump into application output.</param>
		/// <param name="level">Log level to specify log file name and also to allow/prevent write dumped variable into proper log file by config settings.</param>
		/// <param name="maxDepth">How many levels in complex type variables will be iterated throw to dump all it's properties, fields and other values.</param>
		/// <param name="maxLength">If any dumped string length is larger than this value, it will be cut into this max. length.</param>
		void Log(object obj = null, Level level = Level.INFO, int maxDepth = 0, int maxLength = 0);
	}
}
