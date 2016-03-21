using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Number.Model
{
    public class NumberInformation
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 号段
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 区号
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string PostCode { get; set; }

        public NumberInformation()
        {
            
        }

        public NumberInformation(string number, string section, string city, string type, string cityCode, string postCode)
        {
            Number = number;
            Section = section;
            City = city;
            Type = type;
            CityCode = cityCode;
            PostCode = postCode;
        }
    }
}
