using UnityEngine;
using DG.Tweening;


public static class Vibration
{
    static Tweener m_Tweener = null;

    /// <summary>振動処理</summary>
    /// <param name="intensity">振動の強度</param>
    /// <param name="rate">振動率</param>
    /// <param name="time">振動時間</param>
    /// <param name="ease">振動の種別(DG.Tweening.Ease)</param>
    public static Tweener OnVibrate(this Transform transform, float intensity, Vector3 rate, float time, Ease ease = Ease.Linear)
    {
        if (transform.IsVibrating()) return m_Tweener;

        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        m_Tweener = DOTween.To(() => intensity, x => intensity = x, 0f, time).SetEase(ease)
            .OnUpdate(() =>
            {
                elapsed += Time.deltaTime;
                Vector3 newPosition = originalPosition + Multiple(Random.onUnitSphere * intensity, rate);
                transform.position = newPosition;
            })
            .OnComplete(() => transform.position = originalPosition);

        return m_Tweener;
    }

    /// <summary>振動中か(Vibrate関数を実行中か)</summary>
    public static bool IsVibrating(this Transform transform)
    {
        if (transform == null)      return false;
        if (m_Tweener == null)      return false;
        if (!m_Tweener.IsActive())  return false;

        return true;
    }

    /// <summary>ベクトル同士の乗算</summary>
    /// <param name="v">ベクトル(複数指定可能)</param>
    /// <returns>乗算されたベクトル</returns>
    static Vector3 Multiple(params Vector3[] v)
    {
        for (int i = 1; i < v.Length; ++i) v[0] = new Vector3(v[0].x * v[i].x, v[0].y * v[i].y, v[0].z * v[i].z);

        return v[0];
    }
}