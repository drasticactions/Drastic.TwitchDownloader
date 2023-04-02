// <copyright file="VODDownloaderViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tools;
using Drastic.ViewModels;
using System.Text.RegularExpressions;
using TwitchDownloaderCore;

namespace Drastic.TwitchDownloader.ViewModels
{
    /// <summary>
    /// VOD Downloader View Model.
    /// </summary>
    public class VODDownloaderViewModel : BaseViewModel
    {
        private string? twitchUri = string.Empty;

        public VODDownloaderViewModel(IServiceProvider services)
            : base(services)
        {
            this.GetInfoCommand = new AsyncCommand<string>(this.RunGetInfoCommandAsync, (uri) => this.ValidateUrl(uri) > 0, this.ErrorHandler);
        }

        public string? TwitchUri
        {
            get
            {
                return this.twitchUri;
            }

            set
            {
                this.SetProperty(ref this.twitchUri, value);
                this.RaiseCanExecuteChanged();
            }
        }

        public AsyncCommand<string> GetInfoCommand { get; }

        /// <inheritdoc/>
        public override void RaiseCanExecuteChanged()
        {
            base.RaiseCanExecuteChanged();
            this.GetInfoCommand.RaiseCanExecuteChanged();
        }

        private Task RunGetInfoCommandAsync(string twitchUri)
        {
            var videoId = this.ValidateUrl(twitchUri);
            if (videoId < 0)
            {
                return Task.CompletedTask;
            }

            return this.PerformBusyAsyncTask(
               async () =>
            {
                await this.GetInfoAsync(videoId);
                await this.GetVideoTokenAsync(videoId);
            },
               "Getting Info");
        }

        private async Task GetInfoAsync(int videoId)
        {
            var info = await TwitchHelper.GetVideoInfo(videoId);
        }

        private async Task GetVideoTokenAsync(int videoId)
        {
            var info = await TwitchHelper.GetVideoInfo(videoId);
        }

        private int ValidateUrl(string text)
        {
            var vodIdRegex = new Regex(@"(?<=^|twitch\.tv\/videos\/)\d+(?=$|\?)");
            var vodIdMatch = vodIdRegex.Match(text);
            if (vodIdMatch.Success)
            {
                return int.Parse(vodIdMatch.ValueSpan);
            }

            return -1;
        }
    }
}
