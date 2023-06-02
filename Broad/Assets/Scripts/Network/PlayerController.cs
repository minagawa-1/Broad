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
        // ���́A�����g���T�[�o�[���A�N���C�A���g���A���̑�������
        //Debug.Log($"{gameObject.name}.type: " + (isClient ? "Client" : isServer ? "Server" : "���̑�"));

        Move();

        if (Input.GetKeyDown(KeyCode.Z)) CmdCreate(m_Object);
    }

    public void Move()
    {
        // �ړ�����

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow    ) || Input.GetKey(KeyCode.W)) move += Vector3.up;
        if (Input.GetKey(KeyCode.RightArrow ) || Input.GetKey(KeyCode.D)) move += Vector3.right;
        if (Input.GetKey(KeyCode.DownArrow  ) || Input.GetKey(KeyCode.S)) move += Vector3.down;
        if (Input.GetKey(KeyCode.LeftArrow  ) || Input.GetKey(KeyCode.A)) move += Vector3.left;

        transform.position += move;


        //Debug.Log($"{gameObject.name}.position: {transform.position}");
    }

    /// <summary>�T�[�o�[�ɃI�u�W�F�N�g�𐶐��������Ƃ�`����</summary>
    /// ��[Command]�̓N���C�A���g�̃v���C���[����T�[�o�[�̃v���C���[�ɑ��M����
    [Command]
    void CmdCreate(GameObject gameObject)
    {
        RpcCreate();
    }

    /// <summary>�e�N���C�A���g�Ɏ��s������</summary>
    [ClientRpc]
    void RpcCreate()
    {
        GameObject obj = Instantiate(m_Object);

        //NetworkServer.Spawn(obj);
    }
}
