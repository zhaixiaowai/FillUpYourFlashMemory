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
using Newtonsoft.Json;
namespace FillUpYourFlashMemory.Utils
{
	/// <summary>
	/// json格式化帮助类
	/// </summary>
	static class JSON
	{
		/// <summary>
		/// 格式化对象为JSON字符串
		/// </summary>
		/// <param name="obj">待格式化对象</param>
		/// <returns></returns>
		internal static string Stringify(object obj)
		{
			try
			{
				return JsonConvert.SerializeObject(obj);
			}
			catch 
			{

				return null;
			}
		}
		/// <summary>
		/// 转换JSON为指定类型对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="jsonStr"></param>
		/// <returns>转换失败返回T的默认值</returns>
		internal static T Parse<T>(string jsonStr)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(jsonStr);
			}
			catch 
			{
				return default; 
			}
		}
	}
}
