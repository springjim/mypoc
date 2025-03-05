using POCControlCenter.Service.Model;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCControlCenter.Service
{
    public class PocAuthenticator : AuthenticatorBase
    {
        readonly string _baseUrl;
        readonly string _userName;
        readonly string _password;

        string _accessToken;
        string _refreshToken;
        DateTime _tokenExpTime;

        public PocAuthenticator(string baseUrl, string userName, string password) : base("")
        {
            _baseUrl = baseUrl;
            _userName = userName;
            _password = password;
        }

        protected override Parameter GetAuthenticationParameter(string accessToken)
        {
            //这里判断accessToken过期时间，和 refreshToken的过期时间
            string token = "";
            if (string.IsNullOrEmpty(_accessToken))
            {
                token = GetToken();
            }
            else
            {
                if (_tokenExpTime!=null && _tokenExpTime.Subtract(DateTime.Now).TotalMinutes < 2)
                {
                    token = GetRefreshToken();
                } else
                {
                    //没有过期
                    token = _accessToken;
                }

            }
            Log.I(String.Format("GetAuthenticationParameter 返回token：{0}", token));
            return new Parameter("token", token, ParameterType.HttpHeader);

        }

        private string GetToken()
        {

            var client = new RestClient(_baseUrl)
            {

            };

            var request = new RestRequest("app/login", Method.POST)
            { RequestFormat = DataFormat.Json };

            request.AddHeader("Content-Type", "application/json"); //设置HTTP头
            PttUserLoginForm obj = new PttUserLoginForm();
            obj.account = _userName;
            obj.password = _password;  //调用方要负责进行md5加密处理
            request.AddBody(obj);

            var response = client.Execute<LoginResponse>(request);
            if (response == null)
            {
                Log.E("GetToken return null");
                return null;
            }
            else
            {
                LoginResponse resp = response.Data;
                if (resp.code == 0)
                {
                    _tokenExpTime = DateTime.Now.AddSeconds(Convert.ToDouble(resp.data.expiresIn));

                    _accessToken = resp.data.accessToken;
                    _refreshToken = resp.data.refreshToken;
                    //
                    PocConstant.GetInstance().cmpId = resp.data.cmpId;
                    PocConstant.GetInstance().sdkappId = resp.data.sdkappId;
                    PocConstant.GetInstance().userSig = resp.data.userSig;
                    Log.I(String.Format("过期时间：{0}", _tokenExpTime.ToString()));
                    return resp.data.accessToken;
                }
                else
                {
                    Log.E(String.Format("GetToken error, code：{0}", resp.code));
                    return "";
                }

            }

        }

        //请求refreshToken
        private string GetRefreshToken()
        {

            var client = new RestClient(_baseUrl)
            {

            };

            var request = new RestRequest("app/refresh-token", Method.GET)
            { RequestFormat = DataFormat.Json };

            request.AddHeader("Content-Type", "application/json"); //设置HTTP头
            request.AddQueryParameter("refreshToken", _refreshToken);

            var response = client.Execute<LoginResponse>(request);
            if (response == null)
            {
                Log.E("GetRefreshToken return null");
                return null;
            }
            else
            {
                LoginResponse resp = response.Data;
                if (resp.code == 0)
                {
                    _tokenExpTime = DateTime.Now.AddSeconds(Convert.ToDouble(resp.data.expiresIn));

                    _accessToken = resp.data.accessToken;
                    _refreshToken = resp.data.refreshToken;
                    //
                    PocConstant.GetInstance().cmpId = resp.data.cmpId;
                    PocConstant.GetInstance().sdkappId = resp.data.sdkappId;
                    PocConstant.GetInstance().userSig = resp.data.userSig;
                    Log.I(String.Format("过期时间：{0}", _tokenExpTime.ToString()));
                    return resp.data.accessToken;
                }
                else
                {
                    Log.E(String.Format("GetRefreshToken  error, code：{0}", resp.code));  //refreshToken过期了
                    return "";
                }

            }

        }


    }
}
