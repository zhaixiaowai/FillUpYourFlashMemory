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


namespace FillUpYourFlashMemory.Utils
{
	/// <summary>
	/// 通用方法类
	/// </summary>
	static class Util
	{
		/// <summary>
		/// 随机数生成器
		/// </summary>
		internal static Random Rnd = new Random();
		/// <summary>
		/// 格式化字节大小为字符串描述
		/// </summary>
		/// <param name="size">字节大小</param>
		/// <param name="useRadix1024">以1024为基数换算,如果为false则以1000换算</param>
		/// <param name="delimiter">与单位的间隔字符</param>
		/// <returns></returns>
		internal static string FormatBytesLength(long size,bool useRadix1024,  string delimiter = "")
		{
			var arr = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
			double result =size;
			int i;
			int radix = useRadix1024 ? 1024 : 1000;
			for (i = 0; result >= radix && i < arr.Length-1; i++)
			{
				result /= radix;
			}
			int digits = 3;
			if (i == 2)
			{
				digits = 2;
			}
			else if (i < 3)
			{
				digits = 0;
			}
			return $"{result.ToString($"N{digits}")}{delimiter}{arr[i]}";
		}
	}
}