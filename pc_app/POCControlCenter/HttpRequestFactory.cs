using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace POCControlCenter
{
    /// <summary>
    /// 用于Http异步请求的工厂类封装
    /// 参考 http://blog.csdn.net/dyllove98/article/details/9075557
    /// </summary>
    public class HttpRequestFactory
    {
        static HttpRequestFactory()
        {
            MaxRequestCount = 10;
            ThreadPool.QueueUserWorkItem(new WaitCallback(Process));
        }
        class RequestTask
        {
            public RequestInfo Request { set; get; }
            public Action<ResponseInfo> Action { set; get; }
        }

        static List<RequestTask> requestTask = new List<RequestTask>();
        static readonly object Lockobj = new object();
        static List<HttpAPI_Async> Handlers = new List<HttpAPI_Async>();

        public static void AddRequestTask(RequestInfo info, Action<ResponseInfo> act)
        {
            if (!string.IsNullOrEmpty(info.Url))
            {
                lock (Lockobj)
                {
                    requestTask.Add(new RequestTask() { Request = info, Action = act });
                }
            }
        }

        private static HttpAPI_Async GetAvailableHttpRequest()
        {
            HttpAPI_Async handler = null;
            for (int i = 0; i < Handlers.Count; i++)
            {
                if (!Handlers[i].IsBusy)
                {
                    handler = Handlers[i];
                    break;
                }
            }
            if (handler != null)
                return handler;
            if (Handlers.Count < MaxRequestCount)
            {
                handler = new HttpAPI_Async();
                lock (Lockobj)
                {
                    Handlers.Add(handler);
                }
                return handler;
            }
            Thread.Sleep(100);
            return GetAvailableHttpRequest();
        }

        private static RequestTask GetTask()
        {
            RequestTask task = null;
            if (requestTask.Count > 0)
            {
                lock (Lockobj)
                {
                    task = requestTask[0];
                    requestTask.RemoveAt(0);
                }
                return task;
            }
            else if (Closed)
            {
                return null;
            }
            else
            {
                Thread.Sleep(500);
                while (task == null)
                {
                    return GetTask();
                }

            }
            return task;
        }

        private static void Process(object obj)
        {
            while (!Closed || requestTask.Count > 0)
            {
                HttpAPI_Async handler = GetAvailableHttpRequest();
                RequestTask task = GetTask();
                if (task != null && handler != null)
                {
                    handler.GetResponseAsync(task.Request, task.Action);
                }
            }
            Thread.Sleep(10);
        }

        static int MaxRequestCount { set; get; }
        static bool Closed { set; get; }

    }
}
