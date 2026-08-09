using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Watermelon.Message
{
    public class IDCardHelper
    {
        public static GameErrorCode CheckIDCard(string id)
        {
            if (id.Length != 18)
            {
                return GameErrorCode.IdCardLengthError;
            }
            
            if (!Regex.IsMatch(id, @"^[1-9]\d{5}(18|19|20)\d{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])\d{3}[\dXx]$"))
            {
                return GameErrorCode.IdCardDigitError;
            }
            
            if (!ValidateAgeFromIDCard(id))
            {
                return GameErrorCode.IdCardAgeError;
            }
            
            if (!ValidateAreaCode(id))
            {
                return GameErrorCode.IdCardAreaError;
            }
            
            if (!ValidateCheckSum(id))
            {
                return GameErrorCode.IdCardCheckSumError;
            }
            
            return GameErrorCode.Succ;
        }
        
        
         // 简化的地区码字典，实际应用中可能需要更完整的地区码数据
        private static readonly Dictionary<string, string> AreaCodes = new Dictionary<string, string>
        {
            { "110000", "北京市" },
            { "120000", "天津市" },
            { "130000", "河北省" },
            { "140000", "山西省" },
            { "150000", "内蒙古自治区" },
            { "210000", "辽宁省" },
            { "220000", "吉林省" },
            { "230000", "黑龙江省" },
            { "310000", "上海市" },
            { "320000", "江苏省" },
            { "330000", "浙江省" },
            { "340000", "安徽省" },
            { "350000", "福建省" },
            { "360000", "江西省" },
            { "370000", "山东省" },
            { "410000", "河南省" },
            { "420000", "湖北省" },
            { "430000", "湖南省" },
            { "440000", "广东省" },
            { "450000", "广西壮族自治区" },
            { "460000", "海南省" },
            { "500000", "重庆市" },
            { "510000", "四川省" },
            { "520000", "贵州省" },
            { "530000", "云南省" },
            { "540000", "西藏自治区" },
            { "610000", "陕西省" },
            { "620000", "甘肃省" },
            { "630000", "青海省" },
            { "640000", "宁夏回族自治区" },
            { "650000", "新疆维吾尔自治区" },
            { "710000", "台湾地区" },
            { "810000", "香港特别行政区" },
            { "820000", "澳门特别行政区" }
            // 这里可以添加更详细的地区码，如省、市、区/县的完整编码
        };

        /// <summary>
        /// 验证身份证号的地区码是否有效
        /// </summary>
        /// <param name="idCardNumber">身份证号</param>
        /// <returns>如果地区码有效返回true，否则返回false</returns>
        public static bool ValidateAreaCode(string idCardNumber)
        {
            // 验证身份证号长度
            if (string.IsNullOrWhiteSpace(idCardNumber) ||
                (idCardNumber.Length != 15 && idCardNumber.Length != 18))
            {
                return false;
            }

            if (!Regex.IsMatch(idCardNumber.Substring(0, 6), @"^[1-9]\d{5}"))
            {
                return false;
            }

            // 提取前6位地区码
            string areaCode = idCardNumber.Substring(0, 6);

            // 检查地区码是否存在
            if (AreaCodes.ContainsKey(areaCode))
            {
                return true;
            }

            // 检查是否为更详细的地区（前两位匹配省级）
            if (areaCode.Length >= 2)
            {
                string provinceCode = areaCode.Substring(0, 2) + "0000";
                if (AreaCodes.ContainsKey(provinceCode))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static bool ValidateCheckSum(string idCardNumber)
        {
            int sum = 0;
            int[] weights = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            for (int i = 0; i < 17; i++)
            {
                int num = int.Parse(idCardNumber[i].ToString());
                sum += num * weights[i];
            }

            int mod = sum % 11;
            char checkDigit = "10X98765432"[mod];
            
            return checkDigit.ToString() == idCardNumber[17].ToString().ToUpper();
        }
        
        public static bool ValidateAgeFromIDCard(string idCardNumber)
        {
            int age = GetAgeFromIDCard(idCardNumber);
            return age >= 1 && age <= 120;
        }
        
        //根据身份证号获取年龄
        public static int GetAgeFromIDCard(string idCardNumber)
        {
            if (idCardNumber.Length != 18)
            {
                return -1;
            }

            string birthDateStr = idCardNumber.Substring(6, 8);
            if (!DateTime.TryParseExact(birthDateStr, "yyyyMMdd", null, DateTimeStyles.None, out DateTime birthDate))
            {
                return -1;
            }

            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
            {
                age--;
            }

            return age;
        }
    }
}