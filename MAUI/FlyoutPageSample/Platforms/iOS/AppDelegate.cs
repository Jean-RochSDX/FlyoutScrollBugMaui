using FlyoutPageSample.Platforms.iOS.Renderer;
using Foundation;

namespace FlyoutPageSample;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<AppFlyout, IOSFlyoutPageRenderer>();
            });

        var app = builder.Build();

        return app;
    }
}
