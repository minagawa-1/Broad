using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

[AddComponentMenu("UI/Donut Chart", 12)]
[RequireComponent(typeof(RectTransform))]
public class DonutChart : MonoBehaviour
{
    [SerializeField] float m_ChartRadius = 200f;
    [Min(0f)] public float thickness = 20f;  // ドーナツの太さ
    [Min(3)] public int resolution = 100;    // 解像度

    [SerializeField] float m_AnimationTime = 0f;

    static readonly Color default_color = new Color(1f, 1f, 1f);

    public class ChartData
    {
        public int value = 0;
        public DonutChip chip;

        public ChartData(int value, Color? color = null)
        {
            this.value = value;
            chip = null; 
        }
    }

    public List<ChartData> dataList = new List<ChartData>();

    private void Start()
    {
        int[] result = CalcBroad.Calc();

        for (int i = 0; i < GameSetting.instance.playerColors.Length; ++i)
        {
            Add(i, result[i], color: GameSetting.instance.playerColors[i]);
        }

        UpdateDonut();
    }

    public void Add(int num, int value, Color? color)
    {
        ChartData data = new ChartData(value, color);

        GameObject child = new GameObject($"DonutChip[{num}]");
        child.transform.SetParent(transform);
        child.transform.localPosition = Vector3.zero;

        // コンポーネント
        child.AddComponent<CanvasRenderer>();
        data.chip = child.AddComponent<DonutChip>();

        // DonutChipの情報
        data.chip.thickness = thickness;
        data.chip.resolution = resolution;

        // 位置と幅を設定
        data.chip.rectTransform.offsetMin = new Vector2(-m_ChartRadius / 2f, -m_ChartRadius / 2f);
        data.chip.rectTransform.offsetMax = new Vector2( m_ChartRadius / 2f,  m_ChartRadius / 2f);

        // 90度回転(上向きにする)
        data.chip.rectTransform.Rotate(new Vector3(0f, 0f, 90f));
        data.chip.color = color ?? default_color;

        dataList.Add(data);
    }

    public void UpdateDonut()
    {
        int[] result = CalcBroad.Calc();
        for(int i = 0; i < dataList.Count; ++i) dataList[i].value = result[i];

        int sum = dataList.Sum(d => d.value);

        if (sum == 0) return;

        float range = 0f;

        for(int i = 0; i < dataList.Count; ++i)
        {
            if (DOTween.IsTweening(dataList[i].chip)) break;

            float rate = (float)dataList[i].value / (float)sum;

            dataList[i].chip.DoChangeArea(range, range + rate, m_AnimationTime, Ease.OutCubic);

            range += rate;
        }
    }
}