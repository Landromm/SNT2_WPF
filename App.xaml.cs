using Microsoft.Extensions.DependencyInjection;
using SNT2_WPF.Services;
using SNT2_WPF.Services.Implementations;
using SNT2_WPF.View.Graphs;
using SNT2_WPF.ViewModel.Graph;
using SNT2_WPF.ViewModel.MainViewModel;
using System;
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

			return services;
        }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
            Services.GetRequiredService<IUserDialog>().OpenMainWindow();
		}

	}
}
