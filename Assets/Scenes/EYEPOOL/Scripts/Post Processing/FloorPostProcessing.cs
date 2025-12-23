using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Custom/FloorPostProcessing")]
public sealed class FloorPostProcessing : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Input textures to splice/blend")]
    public TextureParameter textZero = new TextureParameter(null);
    public TextureParameter textOne = new TextureParameter(null);
    public TextureParameter textTwo = new TextureParameter(null);
    public TextureParameter textThree = new TextureParameter(null);
    public TextureParameter textFour = new TextureParameter(null);

    Material m_Material;
    public bool IsActive() => textZero.value != null;

    // Do not forget to add this post process in the Custom Post Process Orders list (Project Settings > Graphics > HDRP Global Settings).
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    const string kShaderName = "Hidden/Shader/FloorPostProcessing";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume FloorPostProcessing is unable to load. To fix this, please edit the 'kShaderName' constant in FloorPostProcessing.cs or change the name of your custom post process shader.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;
        m_Material.SetFloat("_Intensity", intensity.value);
        m_Material.SetTexture("_MainTex", source);

        // Bind inputs
        m_Material.SetTexture("_Input0", textZero.value);
        m_Material.SetTexture("_Input1", textOne.value);
        m_Material.SetTexture("_Input2", textTwo.value);
        m_Material.SetTexture("_Input3", textThree.value);
        m_Material.SetTexture("_Input4", textFour.value);

        HDUtils.DrawFullScreen(cmd, m_Material, destination, shaderPassId: 0);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
