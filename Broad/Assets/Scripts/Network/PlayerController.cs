using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] GameObject m_Object = null;

    private void Start()
    {
        if (!isLocalPlayer) return;
    }

    // Update is called once per frame
    void Update()
    {
        // 問題は、俺自身がサーバーか、クライアントか、その他かだな
        //Debug.Log($"{gameObject.name}.type: " + (isClient ? "Client" : isServer ? "Server" : "その他"));

        Move();

        if (Input.GetKeyDown(KeyCode.Z)) CmdCreate(m_Object);
    }

    public void Move()
    {
        // 移動操作

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow    ) || Input.GetKey(KeyCode.W)) move += Vector3.up;
        if (Input.GetKey(KeyCode.RightArrow ) || Input.GetKey(KeyCode.D)) move += Vector3.right;
        if (Input.GetKey(KeyCode.DownArrow  ) || Input.GetKey(KeyCode.S)) move += Vector3.down;
        if (Input.GetKey(KeyCode.LeftArrow  ) || Input.GetKey(KeyCode.A)) move += Vector3.left;

        transform.position += move;


        //Debug.Log($"{gameObject.name}.position: {transform.position}");
    }

    /// <summary>サーバーにオブジェクトを生成したことを伝える</summary>
    /// ※[Command]はクライアントのプレイヤーからサーバーのプレイヤーに送信する
    [Command]
    void CmdCreate(GameObject gameObject)
    {
        RpcCreate();
    }

    /// <summary>各クライアントに実行させる</summary>
    [ClientRpc]
    void RpcCreate()
    {
        GameObject obj = Instantiate(m_Object);

        //NetworkServer.Spawn(obj);
    }
}
