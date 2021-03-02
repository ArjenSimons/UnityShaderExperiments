using UnityEngine;

namespace Raymarching
{
    public enum ShapeType
    {
        SPERE,
        PLANE,
        CUBE,
        TORUS,
    }
    public enum BlendType
    {
        NONE,
        BLEN,
        CUT,
        MASK
    }

    [DisallowMultipleComponent]
    public class Shape : MonoBehaviour
    {
        public ShapeType ShapeType => shapeType;
        public BlendType BlendType => blendType;
        public Color Color => color;
        public float BlendStrength => BlendStrength;
        public Vector3 Position { get { return transform.position; } }
        public Vector3 Scale { get { return transform.lossyScale; } }

        [SerializeField] private ShapeType shapeType;
        [SerializeField] private BlendType blendType;
        [SerializeField] private Color color;
        [SerializeField, Range(0, 1)] private float blendStrength;
    }
}
