using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using Newtonsoft.Json;
using unidyna_web.Models;

namespace unidyna_web.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LineLoginDirect()
        {
            string response_type = "code";
            string client_id = Properties.Settings.Default.Line_Channel_ID ;
            string redirect_uri = "http://localhost:61177" + Url.Action("LineCallback","Login");
            string state = Properties.Settings.Default.Line_State_code;
            string LineLoginUrl = string.Format("https://access.line.me/oauth2/v2.1/authorize?response_type={0}&client_id={1}&redirect_uri={2}&state={3}&scope=openid%20profile&nonce=09876xyz",
                response_type,
                client_id,
                redirect_uri,
                state
                );
            return Redirect(LineLoginUrl);

        }


        public ActionResult LineCallback(string code, string state)
        {
            if (state == Properties.Settings.Default.Line_State_code)
            {
                #region Api變數宣告
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string result = string.Empty;
                NameValueCollection nvc = new NameValueCollection();
                #endregion
                try
                {
                    //取回Token
                    string ApiUrl_Token = "https://api.line.me/oauth2/v2.1/token";
                    nvc.Add("grant_type", "authorization_code");
                    nvc.Add("code", code);
                    nvc.Add("redirect_uri", "http://localhost:61177" + Url.Action("LineCallback", "Login"));
                    nvc.Add("client_id", Properties.Settings.Default.Line_Channel_ID);
                    nvc.Add("client_secret", Properties.Settings.Default.Line_Channel_Secret);
                    string JsonStr = Encoding.UTF8.GetString(wc.UploadValues(ApiUrl_Token, "POST", nvc));
                    LineLoginToken ToKenObj = JsonConvert.DeserializeObject<LineLoginToken>(JsonStr);
                    wc.Headers.Clear();

                    //取回User Profile
                    string ApiUrl_Profile = "https://api.line.me/v2/profile";
                    wc.Headers.Add("Authorization", "Bearer " + ToKenObj.access_token);
                    string UserProfile = wc.DownloadString(ApiUrl_Profile);
                    LineProfile ProfileObj = JsonConvert.DeserializeObject<LineProfile>(UserProfile);

                    //return RedirectToAction("UserProfile", "Login", new { displayName = ProfileObj.displayName, pictureUrl = ProfileObj.pictureUrl });
                    ViewBag.DisplayName = ProfileObj.displayName;
                    ViewBag.PictureUrl =  ProfileObj.pictureUrl ;
                    return View();

                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    throw;
                }
            }
            return View();
        }
    }
}