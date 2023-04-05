// <copyright file="AppDelegate.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Foundation;
using UIKit;

namespace Drastic.TwitchDownloader;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        DrasticForbiddenControls.CatalystControls.AllowsUnsupportedMacIdiomBehavior();
        return base.FinishedLaunching(application, launchOptions);
    }
}
