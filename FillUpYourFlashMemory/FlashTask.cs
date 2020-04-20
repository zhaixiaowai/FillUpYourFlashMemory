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
using System.Diagnostics; 
using System.Threading; 
using FillUpYourFlashMemory.Modules;
using FillUpYourFlashMemory.Utils;
using ThreadPriority = System.Threading.ThreadPriority;

namespace FillUpYourFlashMemory
{
    class FlashTask
    {
        /// <summary>
        /// 任务是否在执行
        /// </summary>
        internal bool Running
        {
            get;
            private set;
        }
        /// <summary>
        /// 是否填充0到文件,false则填充随机字节
        /// </summary>
        internal bool FillZero
        {
            get;
            private set;
        }
        /// <summary>
        /// 存储换算单位是否以1024为基数,如果为false则以1000为基数
        /// </summary>
        internal bool UseRadix1024 { get; set; } = true;
        /// <summary>
        /// 任务线程
        /// </summary>
        private Thread threadTask;
        /// <summary>
        /// 任务统计
        /// </summary>
        private TaskTotal taskTotal = new TaskTotal();
        /// <summary>
        /// 任务观察时间统计
        /// </summary>
        private Stopwatch stopWatch = new Stopwatch(); 
        /// <summary>
        /// 闪存大小变更推送
        /// </summary>
        internal Action<MemorySize> OnMemoryChanged;
        /// <summary>
        /// 文本信息推送
        /// </summary>
        internal Action<string> OnMessage;
        /// <summary>
        /// 统计信息推送
        /// </summary>
        internal Action<TaskTotalJS> OnTotalChanged;
        /// <summary>
        /// 任务停止
        /// </summary>
        internal Action OnStop;
        /// <summary>
        /// 任务开始
        /// </summary>
        /// <param name="fillZero"></param>
        internal void Start(bool fillZero)
        {
            this.FillZero = fillZero;
            this.Running = true;
            if (threadTask != null)
            {
                try
                {
                    threadTask.Abort();
                }
                catch
                {
                }
                threadTask = null;
            }
            threadTask = new Thread(TaskOnThread);
            threadTask.Priority = ThreadPriority.Highest;
            threadTask.IsBackground = true;
            threadTask.Start();
        }
        /// <summary>
        /// 标记任务停止
        /// </summary>
        internal void Stop()
        {
            if (!this.Running) return;
            this.Running = false;
        }
        /// <summary>
        /// 任务线程执行方法
        /// </summary>
        private void TaskOnThread()
        {
            bool stopself = true;
            try
            {
                taskTotal.Bytes = 0;
                taskTotal.Count = 0;
                taskTotal.AverageBytes = 0;
                taskTotal.StartTime = DateTime.Now;
                stopWatch.Restart();
                long localBytes = 0;
                while (Running)
                {
                    Thread.Sleep(20);
                    var size = IOHelper.GetMemorySize(UseRadix1024);
                    OnMemoryChanged?.Invoke(size);
                    var surplus = size.FreeBytes;
                    if (surplus <= 0)
                    {
                        OnMessage?.Invoke("可用空间不足,主动停止任务/insufficient free memory,stop task");
                        return;
                    }
                    var writeBytes = Math.Min(surplus, 819200 + Util.Rnd.Next(204800));
                    var now = DateTime.Now;
                    var filepath = $"{Android.OS.Environment.ExternalStorageDirectory.Path}/.diskpush/ZZZZZZZZZZZZ{now.DayOfYear}/{now.Hour}{now.Minute % 10}ZZZZZ/{Guid.NewGuid().ToString().Replace("-", "")}";
                    var bytes = new byte[writeBytes];
                    if (!FillZero)
                    {
                        Util.Rnd.NextBytes(bytes);
                    }
                    if (!IOHelper.SetFile(filepath, bytes))
                    {
                        OnMessage?.Invoke($"写入文件[{filepath}]失败,请检查权限/write failed, please check permission");
                        return;
                    }
                    taskTotal.Count += 1;
                    taskTotal.Bytes += writeBytes;
                    OnMessage?.Invoke($"写入文件[{filepath}][{ Util.FormatBytesLength(writeBytes, UseRadix1024) }]");
                    localBytes += writeBytes;
                    var milSecond = stopWatch.ElapsedMilliseconds;
                    if (milSecond > 3000)
                    {
                        taskTotal.AverageBytes = (long)Math.Floor((double)(localBytes / (milSecond / 1000)));
                        localBytes = 0;
                        stopWatch.Restart();
                    }
                    surplus -= writeBytes;
                    var allUseTime = taskTotal.UseTime.TotalSeconds;
                    double allAverageBytes = 0;
                    if (allUseTime > 0)
                    {
                        allAverageBytes = taskTotal.Bytes / allUseTime;
                    }

                    if (surplus > 0 && allAverageBytes > 0)
                    {
                        taskTotal.SurplusTimes = surplus / allAverageBytes;
                    }
                    else
                    {
                        taskTotal.SurplusTimes = 0;
                    }
                    OnTotalChanged?.Invoke(new TaskTotalJS(taskTotal, UseRadix1024));
                }
            }
            catch (ThreadAbortException)
            {
                stopself = false;
            }
            catch (Exception ex)
            {
                OnMessage?.Invoke(ex.ToString());
            }
            finally
            {
                stopWatch.Stop();
                Running = false;
                if (stopself)
                {
                    OnStop?.Invoke();
                }
            }
        }
    }
}