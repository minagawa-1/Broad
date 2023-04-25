using UnityEngine;
using DG.Tweening;


public static class Vibration
{
    static Tweener m_Tweener = null;

    /// <summary>�U������</summary>
    /// <param name="intensity">�U���̋��x</param>
    /// <param name="rate">�U����</param>
    /// <param name="time">�U������</param>
    /// <param name="ease">�U���̎��(DG.Tweening.Ease)</param>
    public static Tweener Vibrate(this Transform transform, float intensity, Vector3 rate, float time, Ease ease = Ease.Linear)
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

    /// <summary>�U������(Vibrate�֐������s����)</summary>
    public static bool IsVibrating(this Transform transform)
    {
        if (transform == null)      return false;
        if (m_Tweener == null)      return false;
        if (!m_Tweener.IsActive())  return false;

        return true;
    }

    /// <summary>�x�N�g�����m�̏�Z</summary>
    /// <param name="v">�x�N�g��(�����w��\)</param>
    /// <returns>��Z���ꂽ�x�N�g��</returns>
    static Vector3 Multiple(params Vector3[] v)
    {
        for (int i = 1; i < v.Length; ++i) v[0] = new Vector3(v[0].x * v[i].x, v[0].y * v[i].y, v[0].z * v[i].z);

        return v[0];
    }
}