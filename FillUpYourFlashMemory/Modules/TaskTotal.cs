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

namespace FillUpYourFlashMemory.Modules
{
	internal class TaskTotalJS: TaskTotal
	{
		internal bool UseRadix1024;
		/// <summary>
		/// 预估剩余时长
		/// </summary>
		public string SurplusTimesStr
		{
			get
			{
				if (SurplusTimes == 0)
				{
					return "";
				}
				return TimeSpan.FromSeconds(SurplusTimes).ToString(@"hh\:mm\:ss");
			}
		}
		/// <summary>
		/// 平均速率
		/// </summary>
		public string AverageBytesStr
		{
			get
			{
				return Utils.Util.FormatBytesLength(AverageBytes, UseRadix1024);
			}
		}
		/// <summary>
		/// 累计写入字节数
		/// </summary>
		public string BytesStr
		{
			get
			{
				return Utils.Util.FormatBytesLength(Bytes, UseRadix1024);
			}
		}
		/// <summary>
		/// 截至目前的累计耗时
		/// </summary>
		public string UseTimeStr
		{
			get
			{
				return UseTime.ToString(@"hh\:mm\:ss");
			}
		}
		internal TaskTotalJS( TaskTotal taskTotal,bool useRadix1024)
		{
			this.UseRadix1024 = useRadix1024;
			this.Count = taskTotal.Count;
			this.SurplusTimes = taskTotal.SurplusTimes;
			this.AverageBytes = taskTotal.AverageBytes;
			this.Bytes = taskTotal.Bytes;
			this.StartTime = taskTotal.StartTime;
		}
	}
	/// <summary>
	/// 任务统计
	/// </summary>
	internal class TaskTotal
	{
		/// <summary>
		/// 累计写入文件数
		/// </summary>
		public int Count;
		/// <summary>
		/// 预估剩余时长
		/// </summary>
		internal double SurplusTimes;		
		/// <summary>
		/// 平均速率
		/// </summary>
		internal long AverageBytes;		
		/// <summary>
		/// 累计写入字节数
		/// </summary>
		internal long Bytes;
		/// <summary>
		/// 起始时间
		/// </summary>
		internal DateTime StartTime;
		/// <summary>
		/// 截至目前的累计耗时
		/// </summary>
		internal TimeSpan UseTime
		{
			get
			{
				return (DateTime.Now - StartTime);
			}
		}
		
	}
}