// <copyright file="MauiApplicationDispatcherService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Services;

namespace Drastic.TwitchDownloader.Services
{
    /// <summary>
    ///  Maui Application Dispatcher Service.
    /// </summary>
    internal class MauiApplicationDispatcherService : IAppDispatcher
    {
        /// <inheritdoc/>
        public bool Dispatch(Action action)
        {
            return Microsoft.Maui.Controls.Application.Current!.Dispatcher.Dispatch(action);
        }
    }
}
