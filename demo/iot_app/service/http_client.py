import ujson
import request
import modem


try:
    from common import PrintLog
except:
    from usr.common import  PrintLog
    
    
# HTTP服务器地址（可根据需要修改）
HTTP_SERVER = "http://159.75.230.229:17003/ptt"

HttpMethod = {
    "GET": "GET",
    "POST": "POST"
}

class ApiEndpoint:
    LOGIN = "/app/loginimei"   #终端登录
    
    LOGOUT = "/app/logout"  #终端登出
    QUERY_FIX_GROUP =  "/app/group/query-belonggroup-by-userid-short" #查询固定组
    QUERY_TEMP_GROUP = "/app/group/query-belongtempgroup-by-userid-short"  #查询临时组
    # 可继续添加其他接口...

# === HTTP 响应处理 ===
class HttpResponse:
    def __init__(self, code=0, error=0, errorMsg='', data=None):
        self.code = code
        self.error = error
        self.errorMsg = errorMsg
        self.data = data or {}
        
    def to_dict(self):
        return {"code": self.code,
                "error": self.error,
                "errorMsg": self.errorMsg,
                "data": self.data               
                
                }    

    @classmethod
    def from_json(cls, json_obj):
        try:            
            
            return cls(
                code=json_obj.get('code', 0),
                error=json_obj.get('error', 0),
                errorMsg=json_obj.get('errorMsg', ''),
                data=json_obj.get('data', {})
            )
        except ValueError:
            return cls(code=-1, error=-1, errorMsg='Invalid JSON response')

    def is_success(self):
        return self.code == 0

# === HTTP 客户端 ===
class HttpClient:
    
    def __init__(self, base_url):
        self.base_url = base_url.rstrip('/')
        self.token = None
        self.userId = None  #登录后才有
        self.headers = {
            'Content-Type': 'application/json',
            'User-Agent': 'QuecPython-Device'
        }

    def _request(self, method, endpoint, data=None, params=None, need_token=True):
        
        #url = f"{self.base_url}{endpoint}"  #拼接接口地址
        url= "%s%s" % (self.base_url, endpoint)
        headers = self.headers.copy()   #浅拷贝请求头
        
        if need_token and self.token:
            headers['token'] = self.token

        try:
            # 构建请求参数（QuecPython request 模块的格式）
            body = None  #请求报文的消息体。           

            # GET或POST都有可能有请求拼接参数
            if  params:
                url += '?' + '&'.join([ "%s=%s" % (k, v) for k, v in params.items() ])                          
           

            print("url:", url)
            # POST 请求添加 body
            if method == HttpMethod["POST"] and data:
                body = ujson.dumps(data)  # 将python对象转成json字符串

            # 发送请求
            if method == HttpMethod["POST"]:
                response = request.post(url, headers=headers, data=body)
            else:
                response = request.get(url,  headers=headers)
            #response.json() 返回dict 类型
            response_data = HttpResponse.from_json(response.json()) 
            #ujson.dumps不能对对象进行序列化成json,只能对dict,list
            print("response_data:",response_data.to_dict())
            response.close()
            return response_data

        except Exception as e:
            return HttpResponse(code=-1, error=-1, errorMsg=str(e))

    # === 登录/登出接口 ===
    def login(self):
        
        # 获取设备IMEI
        try:
            imei = modem.getDevImei()  # 获取设备IMEI
            print("设备IMEI:", imei)
        except Exception as e:
            print("获取IMEI失败:", e)
            imei = "default_imei"  # 失败时使用默认值
            
        params_ = {'imei': imei}
        
        response = self._request(
            method=HttpMethod["POST"],
            endpoint=ApiEndpoint.LOGIN,
            params=params_,
            need_token=False
        )
        if response.is_success() and 'accessToken' in response.data:
            self.token = response.data['accessToken']
            self.userId = response.data['userId']
            
        return response

    def logout(self):
        response = self._request(
            method=HttpMethod["POST"],
            endpoint=ApiEndpoint.LOGOUT
        )
        if response.is_success():
            self.token = None
        return response

    # === 固定组查询接口 ===
    def get_fixgroup_info(self):
        
        return self._request(
            method=HttpMethod["GET"],
            endpoint=ApiEndpoint.QUERY_FIX_GROUP,
            params=None,
            need_token=True
        )
    
    # === 临时组查询接口 ===
    def get_tempgroup_info(self):
        
        return self._request(
            method=HttpMethod["GET"],
            endpoint=ApiEndpoint.QUERY_TEMP_GROUP,
            params=None,
            need_token=True
        )

#直接利用 Python 模块的天然单例特性（模块在首次导入时初始化，后续导入复用同一对象
# 在模块级别初始化单例
http_client_singleton = HttpClient(HTTP_SERVER)

    
