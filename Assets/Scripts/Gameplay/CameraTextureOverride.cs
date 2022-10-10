using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTextureOverride : MonoDodge
{
    public Camera cameraA;

    public override void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(cameraA.targetTexture,dest);
    }
}
