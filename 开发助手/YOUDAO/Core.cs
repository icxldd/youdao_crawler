using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using MSScriptControl;
using System.IO;
using System.Windows.Forms;
using System.Web;
using 开发助手.HTTP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace 开发助手.YOUDAO
{
    public class Core
    {
        /*
         
         初始包文：
          POST http://fanyi.youdao.com/translate_o?smartresult=dict&smartresult=rule HTTP/1.1
            Host: fanyi.youdao.com
            Connection: keep-alive
            Content-Length: 214
            Pragma: no-cache
            Cache-Control: no-cache
            Origin: http://fanyi.youdao.com
            X-Requested-With: XMLHttpRequest
            User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36
            Content-Type: application/x-www-form-urlencoded; charset=UTF-8
            Referer: http://fanyi.youdao.com/
            Accept-Encoding: gzip, deflate
            Accept-Language: zh-CN,zh;q=0.9
            Cookie: OUTFOX_SEARCH_USER_ID=-1196939335@10.169.0.83; JSESSIONID=aaaC3yN9sl10-dTJS4Mlw; OUTFOX_SEARCH_USER_ID_NCOO=843467026.9002694; ___rl__test__cookies=1524293883052
            i=%E6%88%91%E6%98%AF&from=AUTO&to=AUTO&smartresult=dict&client=fanyideskweb&salt=1524293883058&sign=00421f5dc841223c1b4c1fec014a6f45&doctype=json&version=2.1&keyfrom=fanyi.web&action=FY_BY_REALTIME&typoResult=false
         */
        //需要翻译的中文
        //1.获取cookie
        //2.获取翻译值
        public string _get(string a1)
        {
            HttpHelper vv1 = new HttpHelper();
            /****************************************************************************************************1*********************************************************************************************************/
            string v1 = "http://fanyi.youdao.com/";
            vv1.HttpVisit(v1);
            /****************************************************************************************************2*********************************************************************************************************/
            string v2_1 = HttpUtility.UrlEncode(a1).ToUpper();//i
            string v2_2 = "fanyideskweb";//client
            string v2_3 = _getArg_r();//salt
            string v2_4 = _getArg_o(a1, v2_2, v2_3);
            string v2_5 = "fanyi.web";

            string v2_6 = string.Format("i={0}&from=AUTO&to=AUTO&smartresult=dict&client={1}&salt={2}&sign={3}&doctype=json&version=2.1&keyfrom={4}&action=FY_BY_CLICKBUTTION&typoResult=false", v2_1, v2_2, v2_3, v2_4, v2_5);
            string v2 = "http://fanyi.youdao.com/translate_o?smartresult=dict&smartresult=rule";
            string v2_r = vv1.HttpVisit(v2, true, v2_6).Html;
            string v2_rr = _getStr(v2_r);



            return v2_rr;
        }
        private string _getStr(string a1)
        {
            /*{"translateResult":[[{"tgt":"acutely","src":"艹"}]],"errorCode":0,"type":"zh-CHS2en"}*/
            JObject o = JObject.Parse(a1);

            string v1 = o["translateResult"][0][0]["tgt"] != null ? o["translateResult"][0][0]["tgt"].ToString() : (o["smartResult"]["entries"][1]).ToString();

            return v1;

        }
        private string _getArg_r()
        {
            string v3 = _getJSContext();
            string v4 = ExecuteScript("getTime();", v3);
            return v4;

        }
        private string _getJSContext()
        {
            string v1 = Application.StartupPath;
            string v2 = v1 + "/md5.js";
            string v3 = File.ReadAllText(v2, Encoding.UTF8);
            return v3;
        }
        //v1 ->需要翻译的词
        private string _getArg_o(string a1, string a2, string a3)
        {
            string s = a1;
            string n = a2;
            string r = a3;
            string d = "ebSeFb%=XZ%T[KZ)c(sy!";
            string v2 = n + s + r + d;
            string v3 = MD5(v2).ToLower();
            return v3;
        }

        private string MD5(string a1)
        {
            string v3 = _getJSContext();
            string v4 = ExecuteScript(string.Format(@"md5('{0}');", a1), v3);
            return v4;
        }

        private string ExecuteScript(string sExpression, string sCode)
        {
            MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
            scriptControl.UseSafeSubset = true;
            scriptControl.Language = "JScript";
            scriptControl.AddCode(sCode);
            try
            {
                string str = scriptControl.Eval(sExpression).ToString();
                return str;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        }
        private static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
    }
}
