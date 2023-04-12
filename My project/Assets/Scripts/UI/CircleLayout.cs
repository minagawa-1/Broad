using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//円形整列レイアウト
public class CircleLayout : LayoutGroup
{
    //一周の角度
    static int ONE_CIRCLE = 360;

    [SerializeField, Range(1, 360)]
    int     angleInterval   = 10;           //角度の間隔
    [SerializeField]
    float   radius          = 100.0f;       //半径

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

    //子要素の生成
    public void Alignment()
    {
        Transform myTransform = transform;

        //必要な子供の数
        int require_count = ONE_CIRCLE / angleInterval;
        //現在の子供の数
        int now_count = myTransform.childCount;

        //必要数より現在の子供の数の方が多い
        if (now_count > require_count)
        {
            //余剰分の子供を消す
            for (int i = 0; i < now_count - require_count; ++i)
                DestroyImmediate(myTransform.GetChild(0).gameObject);
        }
        //現在の子供の数より必要数の方が多い
        else if (require_count > now_count)
        {
            //必要数だけ生成する
            for (int i = 0; i < require_count - now_count; ++i)
            {
                GameObject child = new GameObject();
                child.transform.SetParent(myTransform);
                child.AddComponent<RectTransform>();
            }
        }

        //角度設定
        SetRadius();
    }

    //角度設定
    void SetRadius()
    {
        int i = 0;

        foreach (Transform child in transform)
        {
            //名前を変える
            child.transform.name = "Child" + (i + 1).ToString();

            //ローカル角度を計算する
            float angle = -i * angleInterval;
            child.localEulerAngles = new Vector3(0, 0, angle);

            //ローカル位置を設定する
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
