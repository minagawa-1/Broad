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

    // �Z�b�g�A�b�v
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

    // �I�u�W�F�N�g�v�[���̍쐬
    public void CreatePool(BulletName bulletName, GameObject poolPrefab, Sprite sprite, Vector2 scale, int poolSize)
    {
        m_BulletName        = bulletName;
        m_BulletScale       = scale;
        m_BulletSprite      = sprite;
        m_PoolPrefab        = poolPrefab;

        // poolSize���I�u�W�F�N�g�𐶐�
        for(int i = 0; i < poolSize; ++i)
            CreateNewPoolObject();
    }

    public GameObject GetPoolObject()
    {
        //�ҋ@�I�u�W�F�N�g�����邩���ׂ�
        if (m_UnactiveObjectTransform.childCount > 0)
        {
            Transform child = m_UnactiveObjectTransform.GetChild(0);
            child.GetComponent<SpriteRenderer>().sprite = m_BulletSprite;
            child.gameObject.SetActive(true);
            child.SetParent(m_ActiveObjectTransform);
            return child.gameObject;
        }

        // �S���g���Ă�����A�V�����������ĕԂ�
        GameObject newObject = CreateNewPoolObject();

        newObject.SetActive(true);
        newObject.transform.SetParent(m_ActiveObjectTransform);
        return newObject;
    }

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

        return poolPrefab;
    }

    // �v�[���ɓ����I�u�W�F�N�g�̐���
    private GameObject CreateNewPoolObject()
    {
        GameObject newObject = Instantiate(m_PoolPrefab, m_UnactiveObjectTransform);

        newObject.transform.localScale = m_BulletScale;
        newObject.GetComponent<SpriteRenderer>().sprite = m_BulletSprite;
        newObject.SetActive(false);

        // Bullet�����e
        if (m_BulletName == BulletName.Small)
            newObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineSize", 8f);

        return newObject;
    }
}
