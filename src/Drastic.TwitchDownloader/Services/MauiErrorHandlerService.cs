// <copyright file="MauiErrorHandlerService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Diagnostics;
using Drastic.Services;

namespace Drastic.TwitchDownloader.Services
{
    /// <summary>
    /// Maui Error Handler Service.
    /// </summary>
    public class MauiErrorHandlerService : IErrorHandlerService
    {
        /// <inheritdoc/>
        public void HandleError(Exception ex)
        {
            Debugger.Break();
        }
    }
}
