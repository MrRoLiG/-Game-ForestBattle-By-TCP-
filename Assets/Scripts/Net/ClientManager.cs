using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 这个是用来管理跟服务器端的Socket连接
/// </summary>
public class ClientManager:BaseManager  {

    public ClientManager(GameFacade facade) : base(facade) { }

    private const string IP = "172.17.111.5";
    private const int PORT = 6688;

    private Socket clientSocket;
    private Message message = new Message();
    /// <summary>
    /// 重写初始化方法
    /// </summary>
    public override void OnInit()
    {
        base.OnInit();

        //连接服务器端
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            clientSocket.Connect(IP, PORT);
            Start();
            Debug.Log("unity客户端连接到服务器端...");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("无法连接到服务器端，请检查您的网络！" + ex);
        }
    }

    /// <summary>
    /// 客户端开始监听并接收来自服务器的命令，并通过Message类进行处理
    /// </summary>
    public void Start()
    {
        clientSocket.BeginReceive(message.Data,message.StartIndex,message.RemainSize(), SocketFlags.None, ReceiveCallBack,null);
        Debug.Log("Unity客户端开始监听并准备接收来自服务器的响应命令...");

    }
    
    /// <summary>
    /// 客户端接受回调函数
    /// </summary>
    /// <param name="ar"></param>
    public void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            int count = clientSocket.EndReceive(ar);
            //Debug.Log("unity客户端已经完成对服务器端响应命令的接收...");
            message.AddCount(count);

            //Debug.Log("unity客户端完成对服务器的响应命令的解析...");
            message.ReadMessage(OnProcessCallBack);

            Start();
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    /// <summary>
    /// 处理从服务器端发送过来的命令，并进行响应
    /// </summary>
    /// <param name="actionCode">actionCode即服务器端的响应，通过RequestManager来处理</param>
    /// <param name="data"></param>
    public void OnProcessCallBack(ActionCode actionCode,string data)
    {
        facade.HandleResponse(actionCode, data);
    }

    /// <summary>
    /// 客户端向服务器端发送请求命令
    /// 格式：长度+requestCode+actionCode+数据
    /// 其命令通过Message类进行打包处理
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void SendRequest(RequestCode requestCode,ActionCode actionCode,string data)
    {
        byte[] bytes = Message.PackData(requestCode, actionCode, data);
        clientSocket.Send(bytes);
        //发送
        Debug.Log("客户端已经向服务器端完成发送请求...");
    }

    /// <summary>
    /// 重写销毁方法
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();

        try
        {
            clientSocket.Close();
            Debug.Log("unity客户端关闭与服务器端的连接...");
        }
        catch(Exception ex)
        {
            Debug.LogWarning("无法关闭跟服务器端的连接" + ex);
        }
    }
}
