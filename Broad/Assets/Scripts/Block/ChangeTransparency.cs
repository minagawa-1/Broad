
using UnityEngine;
using rendering = UnityEngine.Rendering;
using rm = woskni.RenderingMode;

public class ChangeTransparency : MonoBehaviour
{
    const float m_transparent_alpha = 0.33f;
    public Material[] blockMaterials;
    public int player;

    /// <summary>�����E��������؂�ւ���</summary>
    public void Change(ref GameObject[] objs) => Set(ref objs, rm.GetBlendMode(blockMaterials[player - 1]) != rm.Mode.Fade);

    /// <summary>�����E�������ɂ���</summary>
    /// <param name="transparent">true : �����ɂ���<br></br>false: �������ɂ���</param>
    public void Set(ref GameObject[] objs, bool transparent)
    {
        for (int i = 0; i < objs.Length; ++i)
        {
            var renderer = objs[i].GetComponent<Renderer>();

            //renderer.shadowCastingMode = transparent ? rendering.ShadowCastingMode.Off : rendering.ShadowCastingMode.On;
        }

        rm.Mode mode = transparent ? rm.Mode.Fade : rm.Mode.Opaque;
        float alpha  = transparent ? m_transparent_alpha : 1f;

        blockMaterials[player - 1] = rm.GetAttachedBlend(blockMaterials[player - 1], mode);
        blockMaterials[player - 1].color = blockMaterials[player - 1].color.GetAlphaColor(alpha);
    }
}
