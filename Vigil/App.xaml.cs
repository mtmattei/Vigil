using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace Vigil;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Uno.Extensions APIs are used in a way that is safe for trimming in this template context.")]
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                .ConfigureServices((context, services) =>
                {
                    var fault = FaultInjection.FromEnvironment();
                    services.AddSingleton<IFaultInjection>(fault);
                    services.AddSingleton<IClock, SystemClock>();
                    services.AddSingleton<IPreferences, Preferences>();
                    services.AddSingleton<IFormulary, EmbeddedFormulary>();
                    services.AddSingleton<ICaseStore>(_ => new JsonCaseStore(
                        Path.Combine(ApplicationData.Current.LocalFolder.Path, "cases"), fault));
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        // UseStudio() carries the App MCP connection on desktop; headless capture runs opt out.
        if (Environment.GetEnvironmentVariable("APP_NO_HOTDESIGN") != "1")
        {
            MainWindow.UseStudio();
        }
#endif
        MainWindow.SetWindowIcon();

        Host = await MainWindow.InitializeNavigationAsync(
            async () =>
            {
                var host = builder.Build();
#if DEBUG
                await SeedForVerificationAsync(host.Services);
#endif
                return host;
            },
            initialRoute: "Board"
        );
    }

#if DEBUG
    /// <summary>Verification hook: <c>--vigil-seed=sample</c> loads the sample day, <c>--vigil-seed=empty</c> clears the store.</summary>
    private static async Task SeedForVerificationAsync(IServiceProvider services)
    {
        var seed = Environment.GetCommandLineArgs()
            .FirstOrDefault(a => a.StartsWith("--vigil-seed=", StringComparison.OrdinalIgnoreCase))?.Split('=', 2)[1];
        if (seed is null)
        {
            return;
        }
        var store = services.GetRequiredService<ICaseStore>();
        var clock = services.GetRequiredService<IClock>();
        await store.ClearAsync(CancellationToken.None);
        if (seed == "sample")
        {
            foreach (var c in SampleDay.Create(clock.Now))
            {
                await store.SaveAsync(c, CancellationToken.None);
            }
        }
    }
#endif

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap<BoardPage, BoardModel>(),
            new ViewMap<NewCasePage, NewCaseModel>(ResultData: typeof(CaseRef)),
            new DataViewMap<RecordPage, RecordModel, CaseRef>(),
            new DataViewMap<DosePage, DoseModel, CaseRef>()
        );

        routes.Register(
            new RouteMap("Board", View: views.FindByViewModel<BoardModel>(), IsDefault: true),
            new RouteMap("NewCase", View: views.FindByViewModel<NewCaseModel>()),
            new RouteMap("Record", View: views.FindByViewModel<RecordModel>()),
            new RouteMap("Dose", View: views.FindByViewModel<DoseModel>())
        );
    }
}
