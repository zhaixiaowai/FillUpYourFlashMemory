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
using Android.OS; 
using System.IO;
using Android.Content.Res;
using FillUpYourFlashMemory.Modules;

namespace FillUpYourFlashMemory.Utils
{
	/// <summary>
	/// 磁盘IO帮助类
	/// </summary>
	static class IOHelper
	{
		/// <summary>
		/// 写入文件
		/// </summary>
		/// <param name="filepath">文件路径</param>
		/// <param name="value">写入数据信息</param>
		/// <returns>是否成功</returns>
		internal static bool SetFile(string filepath, byte[] value)
		{
			if (value == null) return false;
			if (string.IsNullOrEmpty(filepath)) return false;
			try
			{
				var di = new FileInfo(filepath).Directory;
				if (!di.Exists) di.Create();
				di = null;
				using (var fs = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Read, 8192))
				{
					fs.Write(value, 0, value.Length);
				}
				return true;
			}
			catch 
			{
				return false;
			}
		}
		/// <summary>
		/// 读取文件
		/// </summary>
		/// <param name="filepath">文件路径</param>
		/// <returns>当文件不存在时返回null</returns>
		internal static byte[] GetFile(string filepath)
		{
			if (string.IsNullOrEmpty(filepath)) return null;
			try
			{
				var fi = new FileInfo(filepath);
				if (!fi.Exists) return null;
				var len = (int)fi.Length;
				fi = null;
				using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 8192))
				{
					var value = new byte[len];
					var count = fs.Read(value, 0, len);
					if (count != len)
					{
						var copyValue = new byte[count];
						Array.Copy(value, 0, copyValue, 0, count);
						return copyValue;
					}
					return value;
				}
			}
			catch
			{
				return null;
			}
		}
		/// <summary>
		/// 使用utf-8编码读取Asset资源
		/// </summary>
		/// <param name="assetManager"></param>
		/// <param name="filepath"></param>
		/// <returns></returns>
		internal static string GetAssetWithUTF8(AssetManager assetManager, string filepath)
		{
			if (string.IsNullOrEmpty(filepath)) return null;
			try
			{
				using (var fs = new StreamReader(assetManager.Open(filepath)))
				{
					return fs.ReadToEnd();
				}

			}
			catch
			{
				return null;
			}
		}
		/// <summary>
		/// 读取Asset资源
		/// </summary>
		/// <param name="assetManager">AssetManager</param>
		/// <param name="filepath">文件路径</param>
		/// <returns></returns>
		internal static byte[] GetAsset(AssetManager assetManager, string filepath)
		{
			if (string.IsNullOrEmpty(filepath)) return null;
			try
			{
				using (var ms = new MemoryStream())
				{
					using (var fs = assetManager.Open(filepath))
					{
						fs.CopyTo(ms);
						return ms.ToArray();
					}
				}
				
			}
			catch
			{
				return null;
			}
		} 
		internal static MemorySize GetMemorySize(bool useRadix1024)
		{
			MemorySize size = new MemorySize(useRadix1024);
			try
			{
				using(var path= Android.OS.Environment.DataDirectory)
				{
					using(var stat = new StatFs(path.Path))
					{ 
						size.FreeBytes = stat.AvailableBytes;
						size.TotalBytes = stat.TotalBytes;
					}
				}
			}
			catch
			{

			}
			return size;
		}
		
	}
}