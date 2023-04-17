using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    //! �v�[���ɓ����v���n�u
    private GameObject   m_PoolPrefab;
    //�A�N�e�B�u�ȃI�u�W�F�N�g�̐e
    private GameObject   m_ActiveObject             = null;
    //��A�N�e�B�u�ȃI�u�W�F�N�g�̐e
    private GameObject   m_UnactiveObject           = null;
    //�A�N�e�B�u�ȃI�u�W�F�N�g�̐e�̃g�����X�t�H�[��
    private Transform    m_ActiveObjectTransform    = null;
    //��A�N�e�B�u�ȃI�u�W�F�N�g�̐e�̃g�����X�t�H�[��
    private Transform    m_UnactiveObjectTransform  = null;
    //�e���摜
    private Sprite       m_BulletSprite             = null;
    //�e���g�嗦
    private Vector2      m_BulletScale              = Vector2.one;
    // �e�̎��
    private BulletName   m_BulletName               = BulletName.Small;

    /// <summary> �Z�b�g�A�b�v </summary>
    public void Setup()
    {
        // ��ҋ@�I�u�W�F�N�g��ݒ�
        m_ActiveObject = new GameObject();
        m_ActiveObject.transform.SetParent(transform);
        m_ActiveObject.name = "ActiveObjects";
        m_ActiveObjectTransform = m_ActiveObject.transform;

        // �ҋ@�I�u�W�F�N�g��ݒ�
        m_UnactiveObject = new GameObject();
        m_UnactiveObject.transform.SetParent(transform);
        m_UnactiveObject.name = "UnactiveObjects";
        m_UnactiveObjectTransform = m_UnactiveObject.transform;
    }

    /// <summary>                   �v�[���̍쐬 </summary>
    /// <param name="bulletName">   �e���� </param>
    /// <param name="poolPrefab">   �����������v���t�@�u </param>
    /// <param name="sprite">       �A�^�b�`�������摜 </param>
    /// <param name="scale">        �g�嗦 </param>
    /// <param name="poolSize">     �v�[���̑傫�� </param>
    public void CreatePool(BulletName bulletName, GameObject poolPrefab, Sprite sprite, Vector2 scale, int poolSize)
    {
        // �e�����E�v���t�@�u�E�摜�E�g�嗦�����ꂼ��ݒ�
        m_BulletName        = bulletName;
        m_PoolPrefab        = poolPrefab;
        m_BulletSprite      = sprite;
        m_BulletScale       = scale;

        // poolSize���I�u�W�F�N�g�𐶐�
        for(int i = 0; i < poolSize; ++i)
            CreateNewPoolObject();
    }

    /// <summary> �v�[�����̃I�u�W�F�N�g�擾 </summary>
    /// <returns> �v�[���I�u�W�F�N�g </returns>
    public GameObject GetPoolObject()
    {
        //�ҋ@�I�u�W�F�N�g�����邩���ׂ�
        if (m_UnactiveObjectTransform.childCount > 0)
        {
            // �ҋ@�I�u�W�F�N�g��0�Ԃ��擾
            Transform child = m_UnactiveObjectTransform.GetChild(0);

            // �摜�E�A�N�e�B�u�E�e�̐ݒ������
            child.GetComponent<SpriteRenderer>().sprite = m_BulletSprite;
            child.gameObject.SetActive(true);
            child.SetParent(m_ActiveObjectTransform);

            // �I�u�W�F�N�g��Ԃ�
            return child.gameObject;
        }

        // �ҋ@�I�u�W�F�N�g�����Ȃ�������A�V��������
        GameObject newObject = CreateNewPoolObject();

        // �A�N�e�B�u�E�e�̐ݒ�
        newObject.SetActive(true);
        newObject.transform.SetParent(m_ActiveObjectTransform);

        // ���������I�u�W�F�N�g��Ԃ�
        return newObject;
    }

    /// <summary>                   �g���I������I�u�W�F�N�g���A�N�e�B�u������ </summary>
    /// <param name="poolPrefab">   ��A�N�e�B�u���������I�u�W�F�N�g </param>
    /// <returns>                   �I�u�W�F�N�g </returns>
    public GameObject SetUnActive(GameObject poolPrefab)
    {
        // UnActiveObject�̎q���ɂ���
        poolPrefab.transform.SetParent(m_UnactiveObjectTransform);

        //�A�^�b�`����Ă���R���|�[�l���g���폜����
        Destroy(poolPrefab.GetComponent<MoveBullet>());

        // poolPrefab�̃}�e���A���J���[��White�ɂ���
        poolPrefab.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

        // poolPrefab���A�N�e�B�u�ɂ���
        poolPrefab.SetActive(false);

        // �v���t�@�u��Ԃ�
        return poolPrefab;
    }

    /// <summary> �I�u�W�F�N�g���� </summary>
    /// <returns> ���������I�u�W�F�N�g </returns>
    private GameObject CreateNewPoolObject()
    {
        // �I�u�W�F�N�g�̐���
        GameObject newObject = Instantiate(m_PoolPrefab, m_UnactiveObjectTransform);

        // �g�嗦�E�摜�E�A�N�e�B�u�̐ݒ�
        newObject.transform.localScale = m_BulletScale;
        newObject.GetComponent<SpriteRenderer>().sprite = m_BulletSprite;
        newObject.SetActive(false);

        // Bullet�����e
        if (m_BulletName == BulletName.Small)
            // �}�e���A���̐ݒ�
            newObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineSize", 8f);

        // ���������I�u�W�F�N�g��Ԃ�
        return newObject;
    }
}
