// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
#if WINDOWS10_0_17763_0
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
#endif

namespace Microsoft.AppCenter.Utils
{
    public class ApplicationLifecycleHelperWinUI: ApplicationLifecycleHelper
    {
        public ApplicationLifecycleHelperWinUI()
        {

#if WINDOWS10_0_17763_0

            // Subscribe to Resuming and Suspending events.
            CoreApplication.Suspending += delegate { InvokeSuspended(); };

            // If the "LeavingBackground" event is present, use that for Resuming. Else, use CoreApplication.Resuming.
            if (ApiInformation.IsEventPresent(typeof(CoreApplication).FullName, "LeavingBackground"))
            {
                CoreApplication.LeavingBackground += delegate { InvokeResuming(); };

                // If the application has anything visible, then it has already started,
                // so invoke the resuming event immediately.
                HasStartedAndNeedsResume().ContinueWith(completedTask =>
                {
                    if (completedTask.Result)
                    {
                        InvokeResuming();
                    }
                });
            }
            else
            {

                // In versions of Windows 10 where the LeavingBackground event is unavailable, we consider this point to be
                // the start so invoke resuming (and subscribe to future resume events). If InvokeResuming was not called here,
                // the resuming event wouldn't be invoked until the *next* time the application is resumed, which is a problem
                // if the application is not currently suspended. The side effect is that regardless of whether UI is available
                // ever in the process, InvokeResuming will be called at least once (in the case where LeavingBackground isn't
                // available).
                CoreApplication.Resuming += delegate { InvokeResuming(); };
                InvokeResuming();
            }

            // Subscribe to unhandled errors events.
            CoreApplication.UnhandledErrorDetected += (sender, eventArgs) =>
            {
                try
                {

                    // Intentionally propagate exception to get the exception object that crashed the app.
                    eventArgs.UnhandledError.Propagate();
                }
                catch (Exception exception)
                {
                    InvokeUnhandledExceptionOccurred(sender, exception);

                    // Since UnhandledError.Propagate marks the error as Handled, rethrow in order to only Log and not Handle.
                    // Use ExceptionDispatchInfo to avoid changing the stack-trace.
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
            };
#endif
        }

        // Determines whether the application has started already and is not suspended, 
        // but ApplicationLifecycleHelper has not yet fired an initial "resume" event.
        private static async Task<bool> HasStartedAndNeedsResume()
        {
            var needsResume = false;
            try
            {
#if WINDOWS10_0_17763_0

                // Don't use CurrentSynchronizationContext as that seems to cause an error in Unity applications.
                var asyncAction = CoreApplication.MainView?.CoreWindow?.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal, () =>
                    {

                        // If started already, a resume has already occurred.
                        if (_started)
                        {
                            return;
                        }
                        if (CoreApplication.Views.Any(view => view.CoreWindow != null && view.CoreWindow.Visible))
                        {
                            needsResume = true;
                        }
                    });
                if (asyncAction != null)
                {
                    await asyncAction;
                }
#endif
            }
            catch (Exception e) when (e is COMException || e is InvalidOperationException)
            {

                // If MainView can't be accessed, a COMException or InvalidOperationException is thrown. It means that the
                // MainView hasn't been created, and thus the UI hasn't appeared yet.
                AppCenterLog.Debug(AppCenterLog.LogTag,
                    "Not invoking resume immediately because UI is not ready.");
            }
            return needsResume;
        }

        internal void InvokeUnhandledExceptionOccurred(object sender, Exception exception)
        {
            base.InvokeUnhandledExceptionOccurred(sender, new UnhandledExceptionOccurredEventArgs(exception));
        }
    }
}
