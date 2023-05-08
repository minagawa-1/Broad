
using UnityEngine;
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
        if (transparent)
        {
            blockMaterials[player - 1] = rm.GetAttachedBlend(blockMaterials[player - 1], rm.Mode.Fade);
            blockMaterials[player - 1].color = blockMaterials[player - 1].color.GetAlphaColor(m_transparent_alpha);

            for (int i = 0; i < objs.Length; ++i)
                objs[i].GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        else
        {
            blockMaterials[player - 1] = rm.GetAttachedBlend(blockMaterials[player - 1], rm.Mode.Opaque);
            blockMaterials[player - 1].color = blockMaterials[player - 1].color.GetAlphaColor(1f);

            for (int i = 0; i < objs.Length; ++i)
                objs[i].GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

}
