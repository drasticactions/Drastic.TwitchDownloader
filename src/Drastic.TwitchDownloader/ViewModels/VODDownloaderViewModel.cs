// <copyright file="VODDownloaderViewModel.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tools;
using Drastic.ViewModels;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using TwitchDownloaderCore;
using TwitchDownloaderCore.TwitchObjects.Gql;

namespace Drastic.TwitchDownloader.ViewModels
{
    public class VideoQuality
    {
        public string Name { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public int Bandwidth { get; set; }
    }

    /// <summary>
    /// VOD Downloader View Model.
    /// </summary>
    public class VODDownloaderViewModel : BaseViewModel
    {
        private List<string>? thumbnails;

        private string? twitchUri = "https://www.twitch.tv/videos/1784911379";
        private VideoInfo? videoInfo;
        private VideoQuality? videoQuality;

        public VODDownloaderViewModel(IServiceProvider services)
            : base(services)
        {
            this.GetInfoCommand = new AsyncCommand(this.RunGetInfoCommandAsync, () => this.ValidateUrl(this.twitchUri) > 0, this.Dispatcher, this.ErrorHandler);
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

        public VideoInfo? VideoInfo {
            get {
                return this.videoInfo;
            }

            set {
                this.SetProperty(ref this.videoInfo, value);
                this.RaiseCanExecuteChanged();
            }
        }

        public List<string>? Thumbnails {
            get {
                return this.thumbnails;
            }

            set {
                this.SetProperty(ref this.thumbnails, value);
                this.RaiseCanExecuteChanged();
                this.OnPropertyChanged(nameof(this.Thumbnail));
            }
        }

        public ObservableCollection<VideoQuality> VideoQualties { get; } = new ObservableCollection<VideoQuality>();

        public VideoQuality? SelectedVideoQuality {
            get {
                return this.videoQuality;
            }

            set {
                this.SetProperty(ref this.videoQuality, value);
                this.RaiseCanExecuteChanged();
            }
        }

        public string Thumbnail => this.Thumbnails?[0] ?? string.Empty;

        public AsyncCommand GetInfoCommand { get; }

        /// <inheritdoc/>
        public override void RaiseCanExecuteChanged()
        {
            base.RaiseCanExecuteChanged();
            this.GetInfoCommand.RaiseCanExecuteChanged();
        }

        private Task RunGetInfoCommandAsync()
        {
            var videoId = this.ValidateUrl(this.twitchUri!);
            if (videoId < 0)
            {
                return Task.CompletedTask;
            }

            return this.PerformBusyAsyncTask(
               async () =>
            {
                await this.GetInfoAsync(videoId);
            },
               "Getting Info");
        }

        private async Task GetInfoAsync(int videoId)
        {
            var info = await TwitchHelper.GetVideoInfo(videoId);
            var token = await TwitchHelper.GetVideoToken(videoId, null);
            var playlist = await TwitchHelper.GetVideoPlaylist(videoId, token.data.videoPlaybackAccessToken.value, token.data.videoPlaybackAccessToken.signature);
            this.VideoQualties.Clear();
            if (playlist[0].Contains("vod_manifest_restricted"))
            {
                // throw new NullReferenceException(Translations.Strings.InsufficientAccessMayNeedOauth);
                return;
            }

            for (int i = 0; i < playlist.Length; i++)
            {
                if (playlist[i].Contains("#EXT-X-MEDIA"))
                {
                    string lastPart = playlist[i].Substring(playlist[i].IndexOf("NAME=\"") + 6);
                    string stringQuality = lastPart.Substring(0, lastPart.IndexOf('"'));

                    var bandwidthStartIndex = playlist[i + 1].IndexOf("BANDWIDTH=") + 10;
                    var bandwidthEndIndex = playlist[i + 1].IndexOf(',') - bandwidthStartIndex;
                    int bandwidth = 0; // Cannot be inlined if we want default value of 0
                    int.TryParse(playlist[i + 1].Substring(bandwidthStartIndex, bandwidthEndIndex), out bandwidth);

                    var item = new VideoQuality() { Name = stringQuality, Url = playlist[i + 2], Bandwidth = bandwidth };

                    if (!this.VideoQualties.Contains(item))
                    {
                        this.VideoQualties.Add(item);
                    }
                }
            }

            this.VideoInfo = info?.data?.video;
            this.Thumbnails = info?.data?.video?.thumbnailURLs;
        }

        private int ValidateUrl(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return -1;

            var vodIdRegex = new Regex(@"(?<=^|twitch\.tv\/videos\/)\d+(?=$|\?)");
            var vodIdMatch = vodIdRegex.Match(text);
            if (vodIdMatch.Success)
            {
                return int.Parse(vodIdMatch.ValueSpan);
            }

            return -1;
        }

        private static string EstimateVideoSize(int bandwidth, TimeSpan startTime, TimeSpan endTime)
        {
            var sizeInBytes = EstimateVideoSizeBytes(bandwidth, startTime, endTime);

            const long ONE_KILOBYTE = 1024;
            const long ONE_MEGABYTE = 1_048_576;
            const long ONE_GIGABYTE = 1_073_741_824;

            return sizeInBytes switch
            {
                < 1 => "",
                < ONE_KILOBYTE => $" - {sizeInBytes}B",
                < ONE_MEGABYTE => $" - {(float)sizeInBytes / ONE_KILOBYTE:F1}KB",
                < ONE_GIGABYTE => $" - {(float)sizeInBytes / ONE_MEGABYTE:F1}MB",
                _ => $" - {(float)sizeInBytes / ONE_GIGABYTE:F1}GB",
            };
        }

        private static long EstimateVideoSizeBytes(int bandwidth, TimeSpan startTime, TimeSpan endTime)
        {
            if (bandwidth == 0)
            {
                return 0;
            }

            var totalTime = endTime - startTime;
            return (long)(bandwidth / 8d * totalTime.TotalSeconds);
        }

        private ReadOnlySpan<char> GetQualityWithoutSize(string qualityWithSize)
        {
            int qualityIndex = qualityWithSize.LastIndexOf(" - ");
            return qualityIndex == -1
                ? qualityWithSize.AsSpan()
                : qualityWithSize.AsSpan(0, qualityIndex);
        }
    }
}
