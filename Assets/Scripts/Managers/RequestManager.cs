using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestManager : BaseManager {

    public RequestManager(GameFacade facade) : base(facade) { }

    private Dictionary<ActionCode, BaseRequest> requestDic = new Dictionary<ActionCode, BaseRequest>();
    //定义一个字典来存储所有的request

    /// <summary>
    /// 将requestCode及对应的baseRequest一同添加到requestDic中
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="baseRequest"></param>
    public void AddRequest(ActionCode actionCode,BaseRequest baseRequest)
    {
        requestDic.Add(actionCode, baseRequest);
    }

    /// <summary>
    /// 将requestCode及对应的baseRequest从requestDic中移除
    /// </summary>
    /// <param name="requestCode"></param>
    public void RemoveRequest(ActionCode actionCode)
    {
        requestDic.Remove(actionCode);
    }

    /// <summary>
    /// 根据服务器发送过来的命令，客户端进行响应
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void HandleResponse(ActionCode actionCode, string data)
    {
        BaseRequest request = requestDic.TryGet<ActionCode,BaseRequest>(actionCode);
        if (request == null)
        {
            Debug.LogWarning("无法得到ActionCode[" + actionCode + "]对应的Request类");
            return;
        }
        //Debug.Log("unity客户端对服务器端发送过来的响应命令进行响应...");
        request.OnResponse(data);
    }
}
