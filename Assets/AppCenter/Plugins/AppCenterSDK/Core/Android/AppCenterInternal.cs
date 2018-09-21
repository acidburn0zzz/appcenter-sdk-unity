﻿// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using UnityEngine;

namespace Microsoft.AppCenter.Unity.Internal
{
#if UNITY_IOS || UNITY_ANDROID
    using ServiceType = System.IntPtr;
#else
    using ServiceType = System.Type;
#endif

    class AppCenterInternal
    {
        private static AndroidJavaClass _appCenter = new AndroidJavaClass("com.microsoft.appcenter.AppCenter");

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

        public static void StartFromLibrary(ServiceType[] services) {
            AndroidJavaClass context = new AndroidJavaClass("android.content.Context");
            AndroidJavaClass arrayClass = new AndroidJavaClass( "java.lang.reflect.Array" );
            AndroidJavaClass classClass = new AndroidJavaClass( "java.lang.Class" );
            AndroidJavaObject classObject = classClass.CallStatic<AndroidJavaObject>("forName", "com.microsoft.appcenter.analytics.Analytics");
            AndroidJavaObject arrayObject = arrayClass.CallStatic< AndroidJavaObject >( "newInstance", new AndroidJavaClass( "java.lang.Class" ), 1 );
            //AndroidJavaObject [] ooo = new AndroidJavaObject[] { classObject };
            arrayClass.CallStatic( "set", arrayObject, 0, classObject );


            _appCenter.CallStatic("startFromLibrary", context, classObject);
            //_appCenter.CallStatic("startFromLibrary", context, ooo);
        }
    }
}
#endif
