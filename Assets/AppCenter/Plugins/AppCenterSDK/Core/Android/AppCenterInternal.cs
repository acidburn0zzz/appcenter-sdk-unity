﻿// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Internal
{
    class AppCenterInternal
    {
        private static AndroidJavaClass _appCenter = new AndroidJavaClass("com.microsoft.appcenter.AppCenter");
        private static AndroidJavaObject _context;

        public static void SetLogLevel(int logLevel)
        {
            _appCenter.CallStatic("setLogLevel", logLevel);
        }

        public static int GetLogLevel()
        {
            return _appCenter.CallStatic<int>("getLogLevel");
        }

        public static bool IsConfigured()
        {
            return _appCenter.CallStatic<bool>("isConfigured");
        }

        public static void SetLogUrl(string logUrl)
        {
            _appCenter.CallStatic("setLogUrl", logUrl);
        }

        public static string GetSdkVersion()
        {
            return _appCenter.CallStatic<string>("getSdkVersion");
        }

        public static AppCenterTask SetEnabledAsync(bool enabled)
        {
            var future = _appCenter.CallStatic<AndroidJavaObject>("setEnabled", enabled);
            return new AppCenterTask(future);
        }

        public static AppCenterTask<bool> IsEnabledAsync()
        {
            var future = _appCenter.CallStatic<AndroidJavaObject>("isEnabled");
            return new AppCenterTask<bool>(future);
        }

        public static AppCenterTask<string> GetInstallIdAsync()
        {
            AndroidJavaObject future = _appCenter.CallStatic<AndroidJavaObject>("getInstallId");
            var javaUUIDtask = new AppCenterTask<AndroidJavaObject>(future);
            var stringTask = new AppCenterTask<string>();
            javaUUIDtask.ContinueWith(t => {
                var installId = t.Result.Call<string>("toString");
                stringTask.SetResult(installId);
            });
            return stringTask;
        }

        public static void SetCustomProperties(AndroidJavaObject properties)
        {
            _appCenter.CallStatic("setCustomProperties", properties);
        }

        private static AndroidJavaObject GetAndroidApplication()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return activity.Call<AndroidJavaObject>("getApplication");
        }

        public static void SetWrapperSdk(string wrapperSdkVersion,
                                         string wrapperSdkName,
                                         string wrapperRuntimeVersion,
                                         string liveUpdateReleaseLabel,
                                         string liveUpdateDeploymentKey,
                                         string liveUpdatePackageHash)
        {
            var wrapperSdkObject = new AndroidJavaObject("com.microsoft.appcenter.ingestion.models.WrapperSdk");
            wrapperSdkObject.Call("setWrapperSdkVersion", wrapperSdkVersion);
            wrapperSdkObject.Call("setWrapperSdkName", wrapperSdkName);
            wrapperSdkObject.Call("setWrapperRuntimeVersion", wrapperRuntimeVersion);
            wrapperSdkObject.Call("setLiveUpdateReleaseLabel", liveUpdateReleaseLabel);
            wrapperSdkObject.Call("setLiveUpdateDeploymentKey", liveUpdateDeploymentKey);
            wrapperSdkObject.Call("setLiveUpdatePackageHash", liveUpdatePackageHash);
            _appCenter.CallStatic("setWrapperSdk", wrapperSdkObject);
        }
        
        private static AndroidJavaObject GetAndroidContext()
        {
            if (_context != null) 
            {
                return _context;
            }
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _context = activity.Call<AndroidJavaObject>("getApplicationContext");
            return _context;
        }

        public static void StartFromLibrary(IntPtr[] servicesArray)
        {
            var startMethod = AndroidJNI.GetStaticMethodID(_appCenter.GetRawClass(), "startFromLibrary", "(Landroid/content/Context;[Ljava/lang/Class;)V");
            AndroidJNI.CallStaticVoidMethod(_appCenter.GetRawClass(), startMethod, new jvalue[]
            {
                new jvalue { l = GetAndroidContext().GetRawObject() }, 
                new jvalue { l = servicesArray[0] } 
            });
        }
    }
}
#endif
