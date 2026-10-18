using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Threading;

namespace DesharpCore.Core {
	/// <summary>
	/// Holds the current ASP.NET Core <see cref="HttpContext"/> for the executing request
	/// and the hosting information captured by the DesharpCore middleware.
	/// </summary>
	internal static class WebContext {
		private static readonly AsyncLocal<HttpContext> _current = new AsyncLocal<HttpContext>();
		/// <summary>
		/// True after <c>UseDesharp()</c> has been called in the ASP.NET Core request pipeline.
		/// </summary>
		internal static bool Hosted { get; set; } = false;
		/// <summary>
		/// True when the hosting environment is Development.
		/// </summary>
		internal static bool IsDevelopment { get; set; } = false;
		/// <summary>
		/// Application content root directory.
		/// </summary>
		internal static string ContentRoot { get; set; } = null;
		/// <summary>
		/// Current request HTTP context or <c>null</c> outside of a web request.
		/// </summary>
		internal static HttpContext Current {
			get { return WebContext._current.Value; }
			set { WebContext._current.Value = value; }
		}
		private const string REQUEST_ID_KEY = "DesharpCore.RequestId";
		private static long _requestCounter = 0;
		/// <summary>
		/// Unique id of the current web request or 0 outside of a web request.
		/// </summary>
		internal static long RequestId {
			get {
				HttpContext context = WebContext.Current;
				if (context == null) return 0;
				if (context.Items.TryGetValue(REQUEST_ID_KEY, out object id)) return (long)id;
				long newId = Interlocked.Increment(ref WebContext._requestCounter);
				context.Items[REQUEST_ID_KEY] = newId;
				return newId;
			}
		}
		private const string REQUEST_STARTED_KEY = "DesharpCore.RequestStarted";
		/// <summary>
		/// Local time when the current web request began.
		/// </summary>
		internal static DateTime RequestStarted {
			get {
				HttpContext context = WebContext.Current;
				if (context != null && context.Items.TryGetValue(REQUEST_STARTED_KEY, out object started)) return (DateTime)started;
				return DateTime.Now;
			}
			set {
				HttpContext context = WebContext.Current;
				if (context != null) context.Items[REQUEST_STARTED_KEY] = value;
			}
		}
		/// <summary>
		/// Session of the current request or <c>null</c> if session middleware is not configured.
		/// </summary>
		internal static ISession Session {
			get {
				HttpContext context = WebContext.Current;
				if (context == null || context.Features.Get<ISessionFeature>() == null) return null;
				try {
					return context.Session;
				} catch (InvalidOperationException) {
					return null;
				}
			}
		}
		/// <summary>
		/// Write text directly into the current response body.
		/// </summary>
		internal static void Write (string text) {
			HttpContext context = WebContext.Current;
			if (context == null) return;
			context.Response.WriteAsync(text).GetAwaiter().GetResult();
		}
		/// <summary>
		/// Thrown to stop the current web request from <c>Debug.Stop()</c> / <c>Debug.DumpAndDie()</c>.
		/// It is caught by the DesharpCore middleware.
		/// </summary>
		internal class StopRequestException : Exception {
		}
	}
}
