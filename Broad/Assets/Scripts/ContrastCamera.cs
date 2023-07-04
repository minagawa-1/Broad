using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class ContrastCamera : MonoBehaviour
{
    private static readonly int m_contrast_id = Shader.PropertyToID("_Contrast");

    [SerializeField] Material m_Material;
    [SerializeField, Range(0f, 10f)] float m_ContrastValue = 1f;

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Contrast(m_contrast_id, m_ContrastValue);

        Graphics.Blit(source, dest, m_Material);
    }

    void Contrast(int contrastID, float value) => m_Material.SetFloat(contrastID, value);
}
