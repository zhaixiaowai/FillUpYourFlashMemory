/**
 The MIT License (MIT)

Copyright (c) 2020 ZhaiXiaoWai(https://www.zhaixiaowai.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
using System; 
using System.Threading;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime; 
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FillUpYourFlashMemory.Modules;
using FillUpYourFlashMemory.Utils;
using FillUpYourFlashMemory.WebViewExt; 

namespace FillUpYourFlashMemory
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize,AlwaysRetainTaskState = true, ClearTaskOnLaunch = false)]
    public class MainActivity : AppCompatActivity
    {
        /// <summary>
        /// 主界面webview
        /// </summary>
        WebView webview;
        /// <summary>
        /// 任务实例
        /// </summary>
        private FlashTask flashTask;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if ((Intent.Flags & Android.Content.ActivityFlags.BroughtToFront) != 0)
            {
                Finish();
                return;
            }
            if (!IsTaskRoot)
            {
                Finish();
                return;
            }
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            CheckWriteExternalStoragePermission();
            InitTask();
            InitWebView();
        }
        private void InitTask()
        {
            flashTask = new FlashTask();
            flashTask.OnMemoryChanged = CallJSRenderMemory;
            flashTask.OnMessage = CallJSTextTip;
            flashTask.OnTotalChanged = CallJSRenderTotal;
            flashTask.OnStop = CallJSSetPlayState;
        }
        /// <summary>
        /// 初始化浏览器
        /// </summary>
        private void InitWebView()
        {
            webview = (WebView)FindViewById(Resource.Id.mainWebView);            
            webview.SetWebChromeClient(new WebChromeClientExt(OnJSCall));
            var settings = webview.Settings;
            settings.CacheMode = CacheModes.CacheElseNetwork;
            settings.DomStorageEnabled = true;
            settings.SetAppCacheMaxSize(1024 * 1024 * 8);
            String appCachePath = ApplicationContext.CacheDir.AbsolutePath;
            settings.SetAppCachePath(appCachePath);
            settings.AllowFileAccess = true;
            settings.SetAppCacheEnabled(true);   
            settings.UseWideViewPort = true;
            settings.SetSupportZoom(false);
            settings.DisplayZoomControls = false;
            settings.BuiltInZoomControls = false;
            settings.LoadWithOverviewMode = false;
            settings.SetPluginState( WebSettings.PluginState.Off);
            settings.JavaScriptEnabled = true;
            settings.DefaultTextEncodingName = "utf-8";
            var homeHtml = IOHelper.GetAssetWithUTF8(Assets, "home.html");
            const string VISUAL_HOME_URL = "http://127.0.0.1:5566/home.html";
            webview.LoadDataWithBaseURL(VISUAL_HOME_URL, homeHtml, "text/html","utf-8", VISUAL_HOME_URL);   
        }
        /// <summary>
        /// 禁用回退键
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if ((keyCode == Keycode.Back) )
            {
                if (flashTask.Running)
                {
                    var toast = Toast.MakeText(this, "任务运行中,回退已禁用\ntask running,back key disabled", ToastLength.Short);
                    toast.Show();
                    return true;
                }
            }
            return base.OnKeyDown(keyCode, e);
        }

        #region 存储权限相关

        private const int WRITE_STORAGE_REQUEST_CODE = 10086;
        public bool CheckWriteExternalStoragePermission()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == Permission.Denied)
            {
                Toast.MakeText(this, "请授权APP读写存储(please authorize app to read and write storage)", ToastLength.Long).Show();
                ActivityCompat.RequestPermissions(this,
                     new string[] { Manifest.Permission.WriteExternalStorage }, WRITE_STORAGE_REQUEST_CODE);
                return false;
            }
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case WRITE_STORAGE_REQUEST_CODE:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Denied)
                        {
                            ThreadPool.QueueUserWorkItem(s => {
                                Thread.Sleep(300);
                                this.RunOnUiThread(() => {
                                    CheckWriteExternalStoragePermission();
                                });
                            });
                        }
                        break;
                    }
            }
        }
        #endregion
        #region JS交互

        /// <summary>
        /// 推送统计数据到前端界面
        /// </summary>
        private void CallJSRenderTotal(TaskTotalJS taskTotalJS)
        {
            CallJS("renderTotal", JSON.Stringify(taskTotalJS));
        }
        /// <summary>
        /// 推送当前闪存信息到前端界面
        /// </summary>
        /// <param name="size"></param>
        private void CallJSRenderMemory(MemorySize size)
        {
            CallJS("renderMemory", JSON.Stringify(size));
        }
        /// <summary>
        /// 推送文本提示到前端界面
        /// </summary>
        /// <param name="text"></param>
        private void CallJSTextTip(string text)
        {
            CallJS("textTip", $"[{DateTime.Now.ToString("HH:mm:ss.fff")}]{text}");
        }
        /// <summary>
        /// 推送任务状态到前端界面
        /// </summary>
        private void CallJSSetPlayState()
        {
            CallJS("setPlayState", flashTask.Running ? "1" : "0");
        }
        /// <summary>
        /// 推送命令到前端界面
        /// </summary>
        /// <param name="command">命令类型</param>
        /// <param name="data">数据</param>
        private void CallJS(string command, string data)
        {
            data = JSON.Stringify(new CallJSParam() { Command = command, Data = data });
            this.RunOnUiThread(() => {
                webview.EvaluateJavascript($"try{{onNativeCall({data})}}catch(ex){{}}", null);
            });
        }
        /// <summary>
        /// 前端界面调用触发回调
        /// </summary>
        /// <param name="url"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string OnJSCall(string url, string command, string data)
        {
            switch (command)
            {
                case "getMemory":
                    {
                        var size = IOHelper.GetMemorySize(flashTask.UseRadix1024);
                        return JSON.Stringify(size);
                    }
                case "start":
                    {
                        var result = false;
                        if (!flashTask.Running)
                        {
                            flashTask.Start(data == "1");
                        }
                        return result ? "true" : "false";
                    }
                case "stop":
                    {
                        flashTask.Stop();
                        return "true";
                    }
                case "setDisplay":
                    {
                        var value = data != "0";
                        if (flashTask.UseRadix1024 != value)
                        {
                            flashTask.UseRadix1024 = value;
                        }
                        return "true";
                    }
            }
            return null;
        }
        #endregion
    }
}

