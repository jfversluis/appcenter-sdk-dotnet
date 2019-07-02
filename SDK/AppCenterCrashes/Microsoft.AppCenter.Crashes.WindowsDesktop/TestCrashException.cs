// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.Serialization;

namespace Microsoft.AppCenter.Crashes
{
    /// <summary>
    /// Exception type thrown for testing purposes. See <see cref="Crashes.GenerateTestCrash"/>. 
    /// </summary>
    [Serializable]
    public class TestCrashException : Exception
    {
        const string CrashMessage = "Test crash exception generated by SDK";

        /// <summary>
        /// Initializes a new instance of the TestCrashException class with a predefined error message.
        /// </summary>
        public TestCrashException() : base(CrashMessage)
        {
        }

        /// <summary>
        /// Deserialization constructor. Not intended for public use.
        /// </summary>
        protected TestCrashException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
