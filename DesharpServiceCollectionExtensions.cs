using System;
using DesharpCore;
using DesharpCore.Core;

namespace Microsoft.Extensions.DependencyInjection {
	/// <summary>
	/// Extension methods for configuring DesharpCore dependency injection in <see cref="IServiceCollection"/>.
	/// </summary>
	public static class DesharpServiceCollectionExtensions {
		/// <summary>
		/// Adds DesharpCore debugging and logging services (<see cref="IDesharpService"/> and <see cref="IDesharp"/>) to the specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <returns>The original <see cref="IServiceCollection"/> for method chaining.</returns>
		public static IServiceCollection AddDesharp(this IServiceCollection services) {
			if (services == null) {
				throw new ArgumentNullException(nameof(services));
			}
			services.AddSingleton<IDesharpService, DesharpService>();
			services.AddSingleton<IDesharp>(sp => sp.GetRequiredService<IDesharpService>());
			return services;
		}

		/// <summary>
		/// Adds DesharpCore debugging and logging services (<see cref="IDesharpService"/> and <see cref="IDesharp"/>) to the specified <see cref="IServiceCollection"/> with runtime configuration action.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <param name="configure">An action to configure <see cref="DebugConfig"/> settings.</param>
		/// <returns>The original <see cref="IServiceCollection"/> for method chaining.</returns>
		public static IServiceCollection AddDesharp(this IServiceCollection services, Action<DebugConfig> configure) {
			if (services == null) {
				throw new ArgumentNullException(nameof(services));
			}
			var config = new DebugConfig();
			configure?.Invoke(config);
			Debug.Configure(config);

			services.AddSingleton<IDesharpService>(new DesharpService(config));
			services.AddSingleton<IDesharp>(sp => sp.GetRequiredService<IDesharpService>());
			return services;
		}

		/// <summary>
		/// Adds DesharpCore debugging and logging services (<see cref="IDesharpService"/> and <see cref="IDesharp"/>) to the specified <see cref="IServiceCollection"/> with runtime configuration instance.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <param name="config">A <see cref="DebugConfig"/> instance containing configuration settings.</param>
		/// <returns>The original <see cref="IServiceCollection"/> for method chaining.</returns>
		public static IServiceCollection AddDesharp(this IServiceCollection services, DebugConfig config) {
			if (services == null) {
				throw new ArgumentNullException(nameof(services));
			}
			Debug.Configure(config);

			services.AddSingleton<IDesharpService>(new DesharpService(config));
			services.AddSingleton<IDesharp>(sp => sp.GetRequiredService<IDesharpService>());
			return services;
		}

		/// <summary>
		/// Adds DesharpCore debugging and logging services to the specified <see cref="IServiceCollection"/> (alias for AddDesharp).
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <returns>The original <see cref="IServiceCollection"/> for method chaining.</returns>
		public static IServiceCollection AddDesharpCore(this IServiceCollection services) {
			return services.AddDesharp();
		}

		/// <summary>
		/// Adds DesharpCore debugging and logging services to the specified <see cref="IServiceCollection"/> with runtime configuration action (alias for AddDesharp).
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <param name="configure">An action to configure <see cref="DebugConfig"/> settings.</param>
		/// <returns>The original <see cref="IServiceCollection"/> for method chaining.</returns>
		public static IServiceCollection AddDesharpCore(this IServiceCollection services, Action<DebugConfig> configure) {
			return services.AddDesharp(configure);
		}

		/// <summary>
		/// Adds DesharpCore debugging and logging services to the specified <see cref="IServiceCollection"/> with runtime configuration instance (alias for AddDesharp).
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
		/// <param name="config">A <see cref="DebugConfig"/> instance containing configuration settings.</param>
		/// <returns>The original <see cref="IServiceCollection"/> for method chaining.</returns>
		public static IServiceCollection AddDesharpCore(this IServiceCollection services, DebugConfig config) {
			return services.AddDesharp(config);
		}
	}
}
