using AutoMapper;
using System.Text;

namespace Lycoris.AutoMapper.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// 文件大小单位 字节转Mb
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected double ConvertBytesToMegabytes(long? bytes)
        {
            bytes ??= 0;
            // 将字节转换为兆字节  
            return (double)bytes / (1024 * 1024);
        }

        /// <summary>
        /// 文件大小单位 字节转适当单位
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected string ConvertBytesToReadableSize(long? bytes)
        {
            bytes ??= 0;

            if (bytes < 1024)
            {
                return $"{bytes} B"; // 小于 1 KB，返回字节
            }
            else if (bytes < 1024 * 1024)
            {
                return $"{(double)bytes / 1024:F2} KB"; // 小于 1 MB，返回 KB
            }
            else if (bytes < 1024 * 1024 * 1024)
            {
                return $"{(double)bytes / (1024 * 1024):F2} MB"; // 小于 1 GB，返回 MB
            }
            else if (bytes < 1024L * 1024 * 1024 * 1024)
            {
                return $"{(double)bytes / (1024 * 1024 * 1024):F2} GB"; // 小于 1 TB，返回 GB
            }
            else
            {
                return $"{(double)bytes / (1024L * 1024 * 1024 * 1024):F2} TB"; // 大于等于 1 TB，返回 TB
            }
        }

        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumStr"></param>
        /// <returns></returns>
        protected T? TryMapperEnum<T>(string? enumStr) where T : struct
        {
            if (string.IsNullOrEmpty(enumStr))
                return null; ;

            if (Enum.TryParse<T>(enumStr, ignoreCase: true, out var result) && Enum.IsDefined(typeof(T), result))
                return result;

            return null;
        }

        /// <summary>
        /// 隐藏邮件详情
        /// </summary>
        /// <param name="email">邮件地址</param>
        /// <param name="left">邮件头保留字符个数，默认值设置为3</param>
        /// <returns></returns>
        protected string HideEmailDetails(string email, int left = 3)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "";
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(email, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))//如果是邮件地址
            {
                int suffixLen = email.Length - email.LastIndexOf('@');
                return HideSensitiveInfo(email, left, suffixLen, false);
            }
            else
            {
                return HideSensitiveInfo(email);
            }
        }

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="sublen">信息总长与左子串（或右子串）的比例</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边，默认true，默认显示左边</param>
        /// <code>true</code>显示左边，<code>false</code>显示右边
        /// <returns></returns>
        protected string HideSensitiveInfo(string? info, int sublen = 3, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
                return "";

            if (sublen <= 1)
                sublen = 3;

            var subLength = info.Length / sublen;

            if (subLength > 0 && info.Length > (subLength * 2))
            {
                string prefix = info[..subLength], suffix = info[^subLength..];
                return prefix + "****" + suffix;
            }

            if (basedOnLeft)
            {
                var prefix = subLength > 0 ? info[..subLength] : info[..1];
                return prefix + "****";
            }
            else
            {
                var suffix = subLength > 0 ? info[^subLength..] : info[^1..];
                return "****" + suffix;
            }
        }

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息实体</param>
        /// <param name="left">左边保留的字符数</param>
        /// <param name="right">右边保留的字符数</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边 </param>
        /// <returns></returns>
        protected string HideSensitiveInfo(string? info, int left, int right, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
                return "";

            var sbText = new StringBuilder();
            int hiddenCharCount = info.Length - left - right;
            if (hiddenCharCount > 0)
            {
                string prefix = info[..left], suffix = info[^right..];
                sbText.Append(prefix);
                for (int i = 0; i < hiddenCharCount; i++)
                {
                    sbText.Append('*');
                }
                sbText.Append(suffix);
            }
            else
            {
                if (basedOnLeft)
                {
                    if (info.Length > left && left > 0)
                    {
                        sbText.Append(info[..left] + "****");
                    }
                    else
                    {
                        sbText.Append(info[..1] + "****");
                    }
                }
                else
                {
                    if (info.Length > right && right > 0)
                    {
                        sbText.Append(string.Concat("****", info.AsSpan(info.Length - right)));
                    }
                    else
                    {
                        sbText.Append(string.Concat("****", info.AsSpan(info.Length - 1)));
                    }
                }
            }
            return sbText.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected string? ChangeEmptyStringToNull(string? str) => string.IsNullOrEmpty(str) ? null : str;
    }
}
