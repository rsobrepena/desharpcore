using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DesharpCore.Core {
	/// <summary>
	/// ASP.NET Core middleware to dispatch DesharpCore separately from the original application.
	/// It renders the debug bar into HTML responses, stores debug panels over redirects
	/// and handles unhandled exceptions of the request.
	/// </summary>
	internal class DesharpMiddleware {
		private readonly RequestDelegate _next;
		public DesharpMiddleware (RequestDelegate next) {
			this._next = next;
		}
		public async Task InvokeAsync (HttpContext context) {
			WebContext.Current = context;
			WebContext.RequestStarted = DateTime.Now;
			Dispatcher dispatcher = null;
			try {
				dispatcher = Dispatcher.GetCurrent(true);
				dispatcher.WebRequestBegin();
				context.Response.OnStarting(() => {
					WebContext.Current = context;
					try {
						dispatcher.WebRequestSessionBegin();
						dispatcher.WebRequestSessionEnd();
						dispatcher.WebRequestPreSendHeaders();
					} catch (Exception ex) {
						Debug.Log(ex.Message + "\r\n" + ex.StackTrace);
					}
					return Task.CompletedTask;
				});
			} catch (Exception ex) {
				Debug.Log(ex.Message + "\r\n" + ex.StackTrace);
			}
			try {
				try {
					await this._next(context);
				} catch (WebContext.StopRequestException) {
					// request has been stopped by Debug.Stop() or Debug.DumpAndDie()
				} catch (Exception ex) {
					if (dispatcher == null) throw;
					dispatcher.WebRequestError(ex);
				}
				if (dispatcher != null) {
					// run the response start callbacks to complete headers and then append the debug bar to the body
					if (!context.Response.HasStarted) await context.Response.StartAsync();
					dispatcher.WebRequestPreSendBody();
				}
			} catch (Exception ex) when (!(ex is OperationCanceledException)) {
				Debug.Log(ex.Message + "\r\n" + ex.StackTrace);
			} finally {
				Dispatcher.Remove();
				WebContext.Current = null;
			}
		}
	}
}
