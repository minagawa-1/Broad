
using UnityEngine;
using rendering = UnityEngine.Rendering;
using rm = woskni.RenderingMode;

public class ChangeTransparency : MonoBehaviour
{
    const float m_transparent_alpha = 0.33f;
    public Material[] blockMaterials;
    public int player;

    /// <summary>“§–¾E”¼“§–¾‚ğØ‚è‘Ö‚¦‚é</summary>
    public void Change(ref Transform[] transforms) => Set(ref transforms, rm.GetBlendMode(blockMaterials[player - 1]) != rm.Mode.Fade);

    /// <summary>“§–¾E”¼“§–¾‚É‚·‚é</summary>
    /// <param name="transparent">true : “§–¾‚É‚·‚é<br></br>false: ”¼“§–¾‚É‚·‚é</param>
    public void Set(ref Transform[] transforms, bool transparent)
    {
        for (int i = 0; i < transforms.Length; ++i)
        {
            var renderer = transforms[i].GetComponent<Renderer>();

            //renderer.shadowCastingMode = transparent ? rendering.ShadowCastingMode.Off : rendering.ShadowCastingMode.On;
        }

        rm.Mode mode = transparent ? rm.Mode.Fade : rm.Mode.Opaque;
        float alpha  = transparent ? m_transparent_alpha : 1f;

        blockMaterials[player - 1] = rm.GetAttachedBlend(blockMaterials[player - 1], mode);
        blockMaterials[player - 1].color = blockMaterials[player - 1].color.GetAlphaColor(alpha);
    }
}
