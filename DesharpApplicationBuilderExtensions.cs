using System;
using DesharpCore.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder {
	/// <summary>
	/// Extension methods for adding the DesharpCore middleware into an ASP.NET Core (MVC or Razor Pages) request pipeline.
	/// </summary>
	public static class DesharpApplicationBuilderExtensions {
		/// <summary>
		/// Adds the DesharpCore middleware into the request pipeline. It renders the debug bar into HTML responses.
		/// Call it as the first middleware, before <c>UseSession()</c>, <c>UseRouting()</c> and endpoint mapping,
		/// to catch also exceptions thrown by any following middleware.
		/// </summary>
		/// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
		/// <returns>The original <see cref="IApplicationBuilder"/> for method chaining.</returns>
		public static IApplicationBuilder UseDesharp(this IApplicationBuilder app) {
			if (app == null) {
				throw new ArgumentNullException(nameof(app));
			}
			IWebHostEnvironment environment = app.ApplicationServices.GetService<IWebHostEnvironment>();
			WebContext.Hosted = true;
			WebContext.IsDevelopment = environment != null && environment.IsDevelopment();
			WebContext.ContentRoot = environment?.ContentRootPath;
			Dispatcher.EnsureWebEnvironment();
			return app.UseMiddleware<DesharpMiddleware>();
		}
	}
}
