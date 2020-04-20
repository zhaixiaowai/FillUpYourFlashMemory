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

namespace FillUpYourFlashMemory.Modules
{
	/// <summary>
	/// 闪存信息
	/// </summary>
	internal class MemorySize
	{		
		internal MemorySize(bool useRadix1024)
		{
			this.UseRadix1024 = useRadix1024;
		}
		internal bool UseRadix1024;
		/// <summary>
		/// 总量
		/// </summary>
		internal long TotalBytes;
		/// <summary>
		/// 可用量
		/// </summary>
		internal long FreeBytes;
		/// <summary>
		/// 总量格式化字符串描述
		/// </summary>
		public string TotalBytesStr
		{
			get
			{
				return Utils.Util.FormatBytesLength(TotalBytes, UseRadix1024);
			} 
		}
		/// <summary>
		/// 可用量格式化字符串描述
		/// </summary>
		public string FreeBytesStr
		{
			get
			{
				return Utils.Util.FormatBytesLength(FreeBytes, UseRadix1024);
			} 
		}
	}
}