using DesharpCore.Core;
using DesharpCore.Renderers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DesharpCore.Panels {
	/// <summary>
	/// Panel displaying ASP.NET Core endpoint routing: all registered route endpoints,
	/// the matched endpoint with its route values and controller action or Razor Page.
	/// </summary>
	[ComVisible(true)]
	public class Routing: IPanel, ISessionPanel {
		public static string PanelName = "routing";
		public int[] DefaultWindowSizes => new int[] { 350, 250 };
		public string IconValue => Routing.PanelName;
		public string Name => Routing.PanelName;
		public PanelIconType PanelIconType => PanelIconType.Class;
		public bool AddIfEmpty => false;
		public PanelType PanelType => PanelType.BarBtnAndWindow;

		private bool _dataCompleted = false;
		private string _barText = String.Empty;
		private string _panelContent = String.Empty;

		public void SessionBegin () {
			this._completeData();
		}
		public void SessionEnd () { }
		public string[] RenderBarTitle () {
			this._completeData();
			return new string[] { this._barText };
		}
		public string RenderWindowContent () {
			this._completeData();
			return this._panelContent;
		}

		private void _completeData () {
			if (this._dataCompleted) return;
			this._dataCompleted = true;
			try {
				HttpContext context = WebContext.Current;
				Endpoint matched = context.GetEndpoint();
				List<RouteEndpoint> routeEndpoints = Routing._getRouteEndpoints(context);
				this._barText = matched != null
					? Routing._getEndpointTarget(matched) + " (" + routeEndpoints.Count + ")"
					: "No matched route";
				this._panelContent = @"<div class=""content" + (matched != null ? "" : " nomatch") + @""">"
					+ Routing._renderAllEndpoints(routeEndpoints, matched)
					+ Routing._renderCurrentRequest(context)
					+ Routing._renderRouteValues(context)
				+ "</div>";
			} catch (Exception e) {
				Debug.Log(e);
			}
		}
		private static List<RouteEndpoint> _getRouteEndpoints (HttpContext context) {
			EndpointDataSource dataSource = context.RequestServices.GetService<EndpointDataSource>();
			if (dataSource == null) return new List<RouteEndpoint>();
			return dataSource.Endpoints.OfType<RouteEndpoint>().OrderBy(e => e.Order).ToList();
		}
		private static string _getEndpointTarget (Endpoint endpoint) {
			ActionDescriptor action = endpoint.Metadata.GetMetadata<ActionDescriptor>();
			if (action is ControllerActionDescriptor controllerAction) {
				return controllerAction.ControllerTypeInfo.FullName + ":" + controllerAction.ActionName;
			}
			if (action is PageActionDescriptor page) {
				return "Page:" + page.RelativePath;
			}
			return endpoint.DisplayName ?? "";
		}
		private static string _renderAllEndpoints (List<RouteEndpoint> endpoints, Endpoint matched) {
			StringBuilder rows = new StringBuilder();
			foreach (RouteEndpoint endpoint in endpoints) {
				bool isMatched = ReferenceEquals(endpoint, matched);
				string httpMethods = String.Join(", ",
					endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? new List<string>()
				);
				rows.Append(String.Format(
					@"<tr{0}><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>",
					isMatched ? @" class=""matched""" : "",
					isMatched ? "&#10003;" : "",
					Routing._renderPattern(endpoint.RoutePattern.RawText),
					@"<div class=""target"">" + Tools.HtmlEntities(Routing._getEndpointTarget(endpoint)) + "</div>",
					Tools.HtmlEntities(httpMethods)
				));
			}
			return @"<b class=""heading"">All Routes:</b><table class=""routes"">"
				+ @"<thead><tr class=""header""><th></th><th>Pattern</th><th>Target</th><th>HTTP methods</th></tr></thead>"
				+ "<tbody>" + rows.ToString() + "</tbody></table>";
		}
		private static string _renderPattern (string rawText) {
			string pattern = "~/" + (rawText ?? "").TrimStart('/');
			pattern = System.Text.RegularExpressions.Regex.Replace(
				Tools.HtmlEntities(pattern), @"\{([^{}]*)\}", "<b>{$1}</b>"
			);
			return @"<div class=""pattern"">""" + pattern + @"""</div>";
		}
		private static string _renderCurrentRequest (HttpContext context) {
			HttpRequest request = context.Request;
			return @"<p class=""source-url"">" + request.Method.ToUpper() + "&nbsp;"
				+ request.Scheme + "://" + request.Host + request.PathBase
				+ "<span>" + request.Path + request.QueryString + "</span></p>";
		}
		private static string _renderRouteValues (HttpContext context) {
			RouteValueDictionary values = context.Request.RouteValues;
			if (values.Count == 0) return "<p>Matched route has no route values.</p>";
			StringBuilder rows = new StringBuilder();
			foreach (var item in values) {
				rows.Append("<tr><td>" + Tools.HtmlEntities(item.Key) + "</td><td>" + Dumper.Dump(
					item.Value, true, Dispatcher.DumpDepth, Dispatcher.DumpMaxLength
				) + "</td></tr>");
			}
			return @"<b class=""heading"">Matched route values:</b><table>"
				+ "<thead><tr><th>Key</th><th>Value</th></tr></thead><tbody>" + rows.ToString() + "</tbody></table>";
		}
	}
}
