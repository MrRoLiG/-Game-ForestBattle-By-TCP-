using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BaseManager
{

    public PlayerManager(GameFacade facade) : base(facade) { }

    private UserData userData;
    public UserData UserData
    {
        set { userData = value; }
        get { return userData; }
    }

    private Dictionary<RoleType, RoleData> roleDataDic = new Dictionary<RoleType, RoleData>();
    //定义一个关于角色类型的字典管理所有的角色

    private Transform rolePositions;
    private RoleType currentRoleType;
    private GameObject currentRoleGameObject;

    private GameObject playerSyncRequest;
    private ShotRequest shotRequest;
    private AttackRequest attackRequest;

    private GameObject remoteRoleGameObject;

    public void SetCurrentRoleType(RoleType roleType)
    {
        currentRoleType = roleType;
    }
    
    public override void OnInit()
    {
        rolePositions = GameObject.Find("RolePositions").transform;
        InitRoleDataDic();
    }
    
    /// <summary>
    /// 初始化角色表
    /// </summary>
    private void InitRoleDataDic()
    {
        roleDataDic.Add(RoleType.Blue, new RoleData(RoleType.Blue, "Hunter_BLUE", "Arrow_BLUE", "Explosion_BLUE", rolePositions.Find("Position1")));
        roleDataDic.Add(RoleType.Red, new RoleData(RoleType.Red, "Hunter_RED", "Arrow_RED", "Explosion_RED", rolePositions.Find("Position2")));
    }

    /// <summary>
    /// 生成角色
    /// </summary>
    public void SpawnRoles()
    {
        foreach (RoleData roleData in roleDataDic.Values)
        {
            //如果是当前角色，保存下来
            GameObject go = GameObject.Instantiate(roleData.RolePrefab, roleData.SpawnPosition, Quaternion.identity);
            go.tag = "Player";
            if (roleData.RoleType == currentRoleType)
            {
                currentRoleGameObject = go;
                currentRoleGameObject.GetComponent<PlayerInfo>().isLocal = true;
            }
            else
            {
                remoteRoleGameObject = go;
            }
        }
    }

    /// <summary>
    /// 得到当前游戏角色
    /// </summary>
    public GameObject GetCurrentRoleGameObject()
    {
        return currentRoleGameObject;
    }

    /// <summary>
    /// 根据角色类型得到角色数据（信息）
    /// </summary>
    /// <param name="roleType"></param>
    /// <returns></returns>
    public RoleData GetRoledataByRolrtype(RoleType roleType)
    {
        RoleData roleData = null;
        roleDataDic.TryGetValue(roleType, out roleData);
        return roleData;
    }

    /// <summary>
    /// 向角色动态添加控制脚本
    /// </summary>
    public void AddControlScripts()
    {
        currentRoleGameObject.AddComponent<PlayerMove>();
        PlayerAttack playerAttack = currentRoleGameObject.AddComponent<PlayerAttack>();
        RoleType roleType = currentRoleGameObject.GetComponent<PlayerInfo>().roleType;
        RoleData roleData = GetRoledataByRolrtype(roleType);

        playerAttack.arrowPrefab = roleData.ArrowPrefab;

        playerAttack.SetPlayerManager(this);
    }

    /// <summary>
    /// 创建同步的Request
    /// </summary>
    public void CreateSyncRequest()
    {
        playerSyncRequest = new GameObject("PlayerSyncRequest");

        playerSyncRequest.AddComponent<MoveRequest>().SetLocalPlayer(currentRoleGameObject.transform, currentRoleGameObject.GetComponent<PlayerMove>()).SetRemotePlayer(remoteRoleGameObject.transform);
        shotRequest = playerSyncRequest.AddComponent<ShotRequest>();
        shotRequest.playerManager = this;

        attackRequest = playerSyncRequest.AddComponent<AttackRequest>();
    }

    /// <summary>
    /// 本地（本客户端）实例化箭
    /// </summary>
    /// <param name="arrowPrefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void Shot(GameObject arrowPrefab, Vector3 position, Quaternion rotation)
    {
        facade.PlayNormalSound(AudioManager.Sound_Timer);
        GameObject.Instantiate(arrowPrefab, position, rotation).GetComponent<Arrow>().isLocal = true;
        shotRequest.SendRequest(arrowPrefab.GetComponent<Arrow>().roleType, position, rotation.eulerAngles);
    }

    /// <summary>
    /// 其他客户端中实例化箭
    /// </summary>
    /// <param name="roleType"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void RemoteShot(RoleType roleType, Vector3 position, Vector3 rotation)
    {
        GameObject arrowPrefab = GetRoledataByRolrtype(roleType).ArrowPrefab;
        Transform transform = GameObject.Instantiate(arrowPrefab).GetComponent<Transform>();

        transform.position = position;
        transform.eulerAngles = rotation;
    }

    /// <summary>
    /// 发送伤害
    /// </summary>
    /// <param name="damage"></param>
    public void SendAttack(int damage)
    {
        attackRequest.SendRequest(damage);
    }

    /// <summary>
    /// 游戏结束后的物体的销毁和命令的置空
    /// </summary>
    public void GameOver()
    {
        GameObject.Destroy(currentRoleGameObject);
        GameObject.Destroy(playerSyncRequest);
        GameObject.Destroy(remoteRoleGameObject);
        shotRequest = null;
        attackRequest = null;
    }

    /// <summary>
    /// 游戏结束后更新战绩信息
    /// </summary>
    /// <param name="totalCount"></param>
    /// <param name="winCount"></param>
    public void UpdateResult(int totalCount,int winCount)
    {
        userData.TotalCount = totalCount;
        userData.WinCount = winCount;
    }
}
