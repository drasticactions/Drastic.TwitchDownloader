// <copyright file="MauiProgram.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CommunityToolkit.Maui;
using Drastic.Services;
using Drastic.TwitchDownloader.Services;
using Drastic.TwitchDownloader.ViewModels;
using Microsoft.Extensions.Logging;
#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
using WinUIEx;
#endif

namespace Drastic.TwitchDownloader;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.Services!
       .AddSingleton<IErrorHandlerService, MauiErrorHandlerService>()
       .AddSingleton<IAppDispatcher, MauiApplicationDispatcherService>()
       .AddSingleton<VODDownloaderViewModel>();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(wndLifeCycleBuilder =>
            {
                wndLifeCycleBuilder.OnWindowCreated(window =>
                {
                    window.CenterOnScreen(1024, 768);

                    var manager = WinUIEx.WindowManager.Get(window);
                    manager.PersistenceId = "DrasticTwitchDownloaderId";
                    manager.MinWidth = 640;
                    manager.MinHeight = 480;
                    manager.Backdrop = new WinUIEx.MicaSystemBackdrop();
                });
            });
        });
#endif

        return builder.Build();
    }
}
