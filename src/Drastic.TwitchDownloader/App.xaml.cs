﻿// <copyright file="App.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.TwitchDownloader;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();

        this.MainPage = new AppShell();
    }
}
