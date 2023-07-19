using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�~�`���񃌃C�A�E�g
public class CircleLayout : LayoutGroup
{
    //����̊p�x
    static int ONE_CIRCLE = 360;

    [SerializeField, Range(1, 360)]
    int     angleInterval   = 10;           //�p�x�̊Ԋu
    [SerializeField]
    float   radius          = 100.0f;       //���a

    public int AngleInterval
    {
        get { return angleInterval; }
        set { angleInterval = value; Alignment(); }
    }

    public float Radius
    {
        get { return radius; }
        set { radius = value; SetRadius(); }
    }

    public void ManualUpdateLayout()
    {
        SetRadius();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        SetRadius();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        SetRadius();
    }
#endif

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        SetRadius();
    }

    //�q�v�f�̐���
    public void Alignment()
    {
        Transform myTransform = transform;

        //�K�v�Ȏq���̐�
        int require_count = ONE_CIRCLE / angleInterval;
        //���݂̎q���̐�
        int now_count = myTransform.childCount;

        //�K�v����茻�݂̎q���̐��̕�������
        if (now_count > require_count)
        {
            //�]�蕪�̎q��������
            for (int i = 0; i < now_count - require_count; ++i)
                DestroyImmediate(myTransform.GetChild(0).gameObject);
        }
        //���݂̎q���̐����K�v���̕�������
        else if (require_count > now_count)
        {
            //�K�v��������������
            for (int i = 0; i < require_count - now_count; ++i)
            {
                GameObject child = new GameObject();
                child.transform.SetParent(myTransform);
                child.AddComponent<RectTransform>();
            }
        }

        //�p�x�ݒ�
        SetRadius();
    }

    //�p�x�ݒ�
    void SetRadius()
    {
        int i = 0;

        foreach (Transform child in transform)
        {
            //���O��ς���
            child.transform.name = "Child" + (i + 1).ToString();

            //���[�J���p�x���v�Z����
            float angle = -i * angleInterval;
            child.localEulerAngles = new Vector3(0, 0, angle);

            //���[�J���ʒu��ݒ肷��
            child.localPosition = child.up * radius;

            i++;
        }
    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
