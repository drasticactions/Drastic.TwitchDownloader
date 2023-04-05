// <copyright file="AppShell.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.TwitchDownloader;

public partial class AppShell : Shell
{
    public AppShell()
    {
        this.InitializeComponent();

#if WINDOWS
        this.FlyoutBehavior = FlyoutBehavior.Locked;
#endif
    }
}
