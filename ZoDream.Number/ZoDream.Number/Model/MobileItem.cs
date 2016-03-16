using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Number.Model
{
    public class MobileItem
    {
        public string Number { get; set; }

        public string City { get; set; }

        public string Type { get; set; }

        public string CityCode { get; set; }

        public string PostCode { get; set; }

        public MobileItem()
        {
            
        }

        public MobileItem(string city)
        {
            City = city;
        }

        public MobileItem(string number, string city, string type, string cityCode, string postCode)
        {
            Number = number;
            City = city;
            Type = type;
            CityCode = cityCode;
            PostCode = postCode;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Number))
            {
                return "您找的号码不在数据库中！";
            }
            return $"手机号码段：{Number}\n卡号归属地：{City}\n卡类型：{Type}\n区号：{CityCode}\n邮编：{PostCode}";
        }
    }
}
