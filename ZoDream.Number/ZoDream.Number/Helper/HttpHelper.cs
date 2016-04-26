using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZoDream.Number.Model;
using ZoDream.Helper.Http;


namespace ZoDream.Number.Helper
{
    public class HttpHelper
    {
        public static MobileItem Get(string number)
        {
            var request = new Request("http://zodream.localhost/admin.php/mobile?phone=" + number);
            string html = request.Get();
            JObject jo = (JObject)JsonConvert.DeserializeObject(html);
            if (jo["status"].ToString() == "success")
            {
                return new MobileItem(jo["data"]["number"].ToString(),
                    jo["data"]["city"].ToString(),
                    jo["data"]["type"].ToString(),
                    jo["data"]["city_code"].ToString(),
                    jo["data"]["postcode"].ToString());
            }
            return new MobileItem();
        }
    }
}
