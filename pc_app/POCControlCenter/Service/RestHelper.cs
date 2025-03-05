using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using POCControlCenter.DataEntity;
using RestSharp;

namespace POCControlCenter.Service
{
    public  class RestHelper
    {

        /// <summary>
        /// params 
        /// </summary>
        RestClient _restClient;
        RestClient _restVideoClient;
        RestClient _restNewClient;

        private Dictionary<string, RestClient> clientDic;
        private static readonly object PadLock = new object();

        private static RestHelper mInstance;

        public static string REST_NEW = "new";
        public static string REST_VIDEO = "video";
        public static string REST_DEFAULT = "default";

        private RestHelper()
        {
            _restClient = new RestClient
            {
                Timeout = 30 * 1000,
                BaseUrl = new Uri(PocClient.BaseUrl)
            };

            _restClient.ThrowOnAnyError = true;  //设置不然不会报异常
            _restClient.ThrowOnDeserializationError = true;
         
            ///////////
            _restVideoClient = new RestClient
            {
                Timeout = 30 * 1000,
                BaseUrl = new Uri(PocClient.BaseUrl_Video)
            };

            _restVideoClient.ThrowOnAnyError = true;  //设置不然不会报异常
            _restVideoClient.ThrowOnDeserializationError = true;

            /////////////////
            ///
            _restNewClient = new RestClient
            {
                Timeout = 30 * 1000,
                BaseUrl = new Uri(PocClient.BaseUrl_New),
                Authenticator= new PocAuthenticator(PocClient.BaseUrl_New,
                PocConstant.GetInstance().userName, PocConstant.GetInstance().password)
            };

            _restNewClient.ThrowOnAnyError = true;  //设置不然不会报异常
            _restNewClient.ThrowOnDeserializationError = true;

            ///////////////////
            clientDic = new Dictionary<string, RestClient>();
            clientDic.Add(REST_DEFAULT, _restClient);
            clientDic.Add(REST_VIDEO, _restVideoClient);
            clientDic.Add(REST_NEW, _restNewClient);
        }

        public static RestHelper GetInstance()
        {
            if (mInstance == null)
            {
                lock (PadLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = new RestHelper();

                    }
                }
                
            }
            return mInstance;
        }
       

        /// <summary>
        /// 基地址
        /// </summary>
        public  Uri BaseUrl
        {
            set { _restClient.BaseUrl = value; }
        }



        /// <summary>
        /// GET
        /// </summary>
        /// <param name="resource">资源</param>
        /// <returns>IRestResponse</returns>
        public  IRestResponse Get(string resource)
        {
            try
            {
                var request = new RestRequest(resource, Method.GET);
                IRestResponse response = _restClient.Execute(request);
                return response;
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return null;
            }
        }



        /// <summary>
        /// GET
        /// 一般返回string或Enum
        /// </summary>
        /// <param name="resource">资源</param>
        /// <param name="obj">返回对象</param>
        /// <returns>true/false</returns>
        public  bool Get(string resource, out object obj)
        {
            try
            {
                IRestResponse response = Get(resource);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    obj = JsonConvert.DeserializeObject(response.Content);
                    return true;
                }
                else
                {
                    obj = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                obj = null;
                return false;
            }
        }


        /// <summary>
        /// GET返回T类型对象
        /// </summary>
        /// <typeparam name="T">存在无参构造函数</typeparam>
        /// <param name="resource">资源</param>
        /// <returns>T</returns>
        public  T Get<T>(string resource) where T : new()
        {
            try
            {
                var request = new RestRequest(resource, Method.GET);
                IRestResponse<T> response = _restClient.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                    return default(T);

                if (response.Data != null)
                    return response.Data;

                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return default(T);
            }
        }

        /// <summary>
        /// GET返回T类型对象
        /// </summary>
        /// <typeparam name="T">存在无参构造函数</typeparam>
        /// <param name="resource">资源</param>
        /// <param name="isData">是否使用IRestResponse<T>.Data</param>
        /// <returns>T</returns>
        public  T Get<T>(string resource, bool isData) where T : new()
        {
            try
            {
                var request = new RestRequest(resource, Method.GET);
                IRestResponse<T> response = _restClient.Execute<T>(request);

                if (response.StatusCode != HttpStatusCode.OK)
                    return default(T);
                if (isData)
                {
                    if (response.Data != null)
                        return response.Data;
                }

                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return default(T);
            }
        }



        /// <summary>
        /// GET返回T类型对象
        /// </summary>
        /// <typeparam name="T">存在无参构造函数的类型</typeparam>
        /// <param name="resource">资源</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>T</returns>
        public  T Get<T>(string resource, List<Parameter> parameters, string defaultClient = null ) where T : new()
        {
            try
            {
                var request = new RestRequest(resource, Method.GET, DataFormat.Json);
                request.AddHeader("content-type", "application/json");
                foreach (var param in parameters)
                {
                    request.AddParameter(param);
                }
                IRestResponse response;
                if (null!= defaultClient && defaultClient.Equals(RestHelper.REST_VIDEO))
                response = _restVideoClient.Execute(request);

                else if (null != defaultClient &&  defaultClient.Equals(RestHelper.REST_NEW))
                    response = _restNewClient.Execute(request);

                else  
                    response = _restClient.Execute(request);

                if (!response.IsSuccessful)
                      return default(T);

                return JsonConvert.DeserializeObject<T>(response.Content,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    }
                    );

            }

            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return default(T);
            }
        }

        



        /// <summary>
        /// GET返回T类型对象
        /// </summary>
        /// <typeparam name="T">存在无参构造函数的类型</typeparam>
        /// <param name="resource">资源</param>
        /// <param name="queryParam">参数列表</param>
        /// <param name="isData">是否使用response.Data</param>
        /// <returns>T</returns>
        public  T Get<T>(string resource, Dictionary<string, string> queryParam, bool isData = false) where T : new()
        {
            try
            {
                var request = new RestRequest(resource, Method.GET);
                foreach (var param in queryParam)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }

                var response = _restClient.Execute<T>(request);

                if (isData)
                    return response.Data;

                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception ex)
            {

                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return default(T);
            }

        }

        /// <summary>
        /// POST方法,且有data返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="obj"></param>
        /// <param name="defaultClient"></param>
        /// <returns></returns>
        public T Post_hasData<T>(string resource, object obj, string defaultClient = null) where T : new()
        {
            try
            {
                var request = new RestRequest(resource, Method.POST) { RequestFormat = DataFormat.Json };

                request.AddHeader("Content-Type", "application/json"); //设置HTTP头
                request.AddBody(obj);//account实体类的一个对象

                IRestResponse response;

                if (defaultClient.Equals(REST_VIDEO))
                {
                    response = clientDic[REST_VIDEO].Execute(request);

                }
                else if (defaultClient.Equals(REST_NEW))
                {
                    response = clientDic[REST_NEW].Execute(request);
                }
                else
                {
                    response = _restClient.Execute(request);
                }

                if (!response.IsSuccessful)
                    return default(T);

                return JsonConvert.DeserializeObject<T>(response.Content,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    }
                    );

            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return default(T);
            }
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <param name="resource">resource</param>
        /// <param name="obj">body参数</param>
        /// <returns>HttpStatusCode</returns>
        public  HttpStatusCode Post(string resource, object obj, string defaultClient=null)
        {
            try
            {
                var request = new RestRequest(resource, Method.POST) { RequestFormat = DataFormat.Json };

                request.AddHeader("Content-Type", "application/json"); //设置HTTP头
                request.AddBody(obj);//account实体类的一个对象
                if (defaultClient.Equals(REST_VIDEO))
                {
                    return clientDic[REST_VIDEO].Execute(request).StatusCode;
                }
                else if (defaultClient.Equals(REST_NEW))
                {
                    return clientDic[REST_NEW].Execute(request).StatusCode;
                }
                else 
                {
                    return _restClient.Execute(request).StatusCode;
                }
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return 0;
            }
        }




        /// <summary>
        /// POST
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="values"></param>
        /// <param name="silence"></param>
        /// <returns></returns>
        public  HttpStatusCode Post(string resource, Dictionary<string, object> values)
        {
            try
            {
                IRestRequest request = new RestRequest(resource, Method.POST);
                foreach (var each in values)
                {
                    request.AddParameter(each.Key, each.Value);
                }

                IRestResponse response = _restClient.Execute(request);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return 0;
            }



        }

        /// <summary>
        /// PUT
        /// 测试通过
        /// </summary>
        /// <param name="resource">资源</param>
        /// <param name="obj">Body</param>
        /// <returns>HttpStatusCode</returns>
        public  HttpStatusCode Put(string resource, object obj)
        {
            try
            {
                var request = new RestRequest(resource, Method.PUT) { RequestFormat = DataFormat.Json };
                request.AddHeader("Content-Type", "application/json"); //设置HTTP头
                request.AddBody(obj);

                IRestResponse response = _restClient.Execute(request);
                HttpStatusCode result = response.StatusCode;
                return result;
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return 0;
            }

        }

        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="resource">资源</param>
        /// <param name="obj">参数</param>
        /// <returns></returns>
        public  HttpStatusCode Delete(string resource, object obj)
        {
            try
            {
                var request = new RestRequest(resource, Method.DELETE) { RequestFormat = DataFormat.Json };
                request.AddHeader("Content-Type", "application/json"); //设置HTTP头
                request.AddBody(obj);//account实体类的一个对象
                IRestResponse response = _restClient.Execute(request);
                HttpStatusCode result = response.StatusCode;
                return result;
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return 0;
            }

        }


        public  HttpStatusCode Delete(string resource)
        {
            try
            {
                var request = new RestRequest(resource, Method.DELETE) { RequestFormat = DataFormat.Json };
                request.AddHeader("Content-Type", "application/json"); //设置HTTP头
                IRestResponse response = _restClient.Execute(request);
                HttpStatusCode result = response.StatusCode;

                return result;
            }

            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return 0;
            }
        }



        /// <summary>
        /// PUT
        /// 测试通过
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="values"></param>
        /// <param name="silence"></param>
        /// <returns></returns>
        public  bool Put(string resource, Dictionary<string, object> values, bool silence = false)
        {
            try
            {
                IRestRequest request = new RestRequest(resource, Method.PUT);
                foreach (var each in values)
                {
                    request.AddParameter(each.Key, each.Value);
                }
                IRestResponse response = _restClient.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return false;
            }

        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="resource">资源</param>
        /// <param name="path">文件路径</param>
        /// <returns>成功：true/失败：false</returns>
        public  bool Put(string resource, string path)
        {
            try
            {
                var request = new RestRequest(resource, Method.PUT) { RequestFormat = DataFormat.Json };
                request.AddHeader("Content-Disposition", "form-data");
                request.AddFile("file", path);

                IRestResponse response = _restClient.Execute(request);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Log.E(JsonConvert.SerializeObject(ex.InnerException));
                return false;
            }
        }

    }
}
