using Microsoft.Extensions.DependencyInjection;
using SNT2_WPF.Services;
using SNT2_WPF.Services.Implementations;
using SNT2_WPF.View.Graphs;
using SNT2_WPF.View.MainViews;
using SNT2_WPF.View.Settings.UserControlView;
using SNT2_WPF.ViewModel.Graph;
using SNT2_WPF.ViewModel.MainViewModel;
using SNT2_WPF.ViewModel.Settings;
using SNT2_WPF.ViewModel.Settings.UserControlViewModel;
using System;
using System.Threading;
using System.Windows;

namespace SNT2_WPF
{
    public partial class App
    {
        private static IServiceProvider? _Services;

		public static IServiceProvider Services => _Services ??= InitializeServices().BuildServiceProvider();

		private static IServiceCollection InitializeServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<MainViewModel>();
            services.AddScoped<GraphCurrentViewModel>();
            services.AddSingleton<GraphArchiveViewModel>();
            services.AddScoped<SettingsWindowViewModel>();

			services.AddSingleton<IUserDialog, UserDialogServices>();
			services.AddSingleton<IMessageBus, MessageBusService>();

			services.AddTransient(
                s =>
                {
                    var model = s.GetRequiredService<MainViewModel>();
                    var window = new MainWindow { DataContext = model };
					    model.DialogComplete += (_, _) => window.Close();

				    return window;
                });
            services.AddTransient(
                s =>
                {
                    var scope = s.CreateScope();
                    var model = scope.ServiceProvider.GetRequiredService<GraphCurrentViewModel>();
                    var window = new GraphCurrentDataView { DataContext = model };
					    model.DialogComplete += (_, _) => window.Close();
					    window.Closed += (_, _) => scope.Dispose();

				    return window;
                });
            services.AddTransient(
                s =>
                {
                    var scope = s.CreateScope();
                    var model = scope.ServiceProvider.GetRequiredService<GraphArchiveViewModel>();
                    var window = new GraphArchiveDataView { DataContext = model };
                        model.DialogComplete += (_, _) => window.Close();
                        window.Closed -= (_, _) => scope.Dispose();

                    return window;
                });
            services.AddTransient(
                s =>
                {
                    var scope = s.CreateScope();
                    var model = scope.ServiceProvider.GetRequiredService<SettingsWindowViewModel>();
                    var window = new SettingsWindowView { DataContext = model };
                        model.DialogComplete += (_, _) => window.Close();
					    window.Closed += (_, _) => scope.Dispose();

					return window;
                });

			return services;
        }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
            Services.GetRequiredService<IUserDialog>().OpenMainWindow();
		}

	}
}
