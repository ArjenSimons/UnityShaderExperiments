using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raymarching
{
    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class RayMarching : MonoBehaviour
    {
        struct ShapeData
        {
            public int shapeType;
            public int blendType;
            public float blendStrength;
            public Vector3 color;
            public Vector3 position;
            public Vector3 scale;

            public static int GetByteSize() { return sizeof(int) * 2 + sizeof(float) * 10; }
        }

        [SerializeField] private ComputeShader raymarcher;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private Camera cam;
        [SerializeField] private Light lightSource;

        private ComputeBuffer shapesBuffer;
    }
}
