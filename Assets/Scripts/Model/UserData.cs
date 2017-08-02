using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 该类包含了登录者的个人信息，包括用户名，总参加局数，总赢局数
/// </summary>
public class UserData{
    
	public string Username { get; private set; }
    public int TotalCount { get;  set; }
    public int WinCount { get;  set; }
    public int Id { get; private set; }

    public UserData(string username,int totalcount,int wincount)
    {
        this.Username = username;
        this.TotalCount = totalcount;
        this.WinCount = wincount;
    }

    public UserData(int id,string username, int totalcount, int wincount)
    {
        this.Id = id;
        this.Username = username;
        this.TotalCount = totalcount;
        this.WinCount = wincount;
    }

    public UserData(string userData)
    {
        string[] strs = userData.Split(',');

        this.Id = int.Parse(strs[0]);
        this.Username = strs[1];
        this.TotalCount = int.Parse(strs[2]);
        this.WinCount = int.Parse(strs[3]);
    }
}
