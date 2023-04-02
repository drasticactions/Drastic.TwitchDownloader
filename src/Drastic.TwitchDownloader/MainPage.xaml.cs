// <copyright file="MainPage.xaml.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.TwitchDownloader.ViewModels;

namespace Drastic.TwitchDownloader;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        this.InitializeComponent();
        var serviceProvider = Microsoft.Maui.Controls.Application.Current!.Handler.MauiContext!.Services;
        this.BindingContext = this.ViewModel = serviceProvider.GetRequiredService<VODDownloaderViewModel>();
    }

    public VODDownloaderViewModel ViewModel { get; }
}
