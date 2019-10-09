﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Utils;
using System;

namespace Microsoft.AppCenter.Windows.Shared.Utils
{
    /**
     * Utility to store and retrieve values for user identifiers.
    */
    public class UserIdContext
    {
        private static readonly object UserIdLock = new object();
        private static UserIdContext _instanceField;

        internal UserIdContext()
        {

        }

        /// <summary>
        /// Maximum allowed length for user identifier for App Center server.
        /// </summary>
        public static int USER_ID_APP_CENTER_MAX_LENGTH = 256;

        /// <summary>
        /// Unique instance.
        /// </summary>
        public static UserIdContext Instance
        {
            get
            {
                lock (UserIdLock)
                {
                    return _instanceField ?? (_instanceField = new UserIdContext());
                }
            }
            set
            {
                lock (UserIdLock)
                {
                    _instanceField = value;
                }
            }
        }

        /// <summary>
        /// Current user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Check if userId is valid for App Center.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true if valid, false otherwise.</returns>
        public static bool CheckUserIdValidForAppCenter(String userId)
        {
            if (userId != null && userId.Length > USER_ID_APP_CENTER_MAX_LENGTH)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "userId is limited to " + USER_ID_APP_CENTER_MAX_LENGTH + " characters.");
                return false;
            }
            return true;
        }
    }
}