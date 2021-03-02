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
        private List<Shape> shapes;
        private ShapeData[] shapesData;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            InitRenderTexture();
            InitEnvironment();
            SetComputeParams(source);

            int gridSizeX = Mathf.CeilToInt(cam.pixelWidth / 8.0f);
            int gridSizey = Mathf.CeilToInt(cam.pixelHeight / 8.0f);

            raymarcher.Dispatch(0, gridSizeX, gridSizey, 1);

            Graphics.Blit(renderTexture, destination);

            shapesBuffer.Dispose();
        }

        private void InitRenderTexture()
        {
            if (renderTexture == null || renderTexture.width != cam.pixelWidth || renderTexture.height != cam.pixelHeight)
            {
                if (renderTexture != null) renderTexture.Release();

                renderTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
                {
                    enableRandomWrite = true
                };
                renderTexture.Create();
            }
        }

        private void InitEnvironment()
        {
            shapes = new List<Shape>(FindObjectsOfType<Shape>());
            shapes.Sort((a, b) => a.BlendType.CompareTo(b.BlendType));

            //Convert shape data to structs
            shapesData = new ShapeData[shapes.Count];

            for (int i = 0; i < shapes.Count; i++)
            {
                shapesData[i] = new ShapeData()
                {
                    shapeType = (int)shapes[i].ShapeType,
                    blendType = (int)shapes[i].BlendType,
                    blendStrength = shapes[i].BlendStrength,
                    color = new Vector3(shapes[i].Color.r, shapes[i].Color.g, shapes[i].Color.b),
                    position = shapes[i].Position,
                    scale = shapes[i].Scale
                };
            }

            shapesBuffer = new ComputeBuffer(shapesData.Length, ShapeData.GetByteSize());
            shapesBuffer.SetData(shapesData);
        }

        private void SetComputeParams(RenderTexture source)
        {
            raymarcher.SetBuffer(0, "_shapes", shapesBuffer);
            raymarcher.SetInt("_numShapes", shapesData.Length);

            raymarcher.SetMatrix("_Cam2WorldMat", cam.cameraToWorldMatrix);
            raymarcher.SetMatrix("_CamInverseProjectionMat", cam.projectionMatrix.inverse);
            raymarcher.SetVector("_LightDir", lightSource.transform.forward);

            raymarcher.SetTexture(0, "_src", source);
            raymarcher.SetTexture(0, "_out", renderTexture);
        }
    }
}
