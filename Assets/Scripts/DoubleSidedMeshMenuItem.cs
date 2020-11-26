using UnityEditor;
using UnityEngine;

public static class DoubleSideMeshMenuItem
{
    [MenuItem("Assets/Create/Double-Sided Mesh")]
    static void MakeDoubleSideMeshAsset()
    {
        var sourceMesh = Selection.activeObject as Mesh;
        if (sourceMesh == null)
        {
            Debug.LogWarning("You must have a mesh assed selected.");
            return;
        }

        //Create inside of the mesh
        Mesh insideMesh = Object.Instantiate(sourceMesh);
        FlipMeshFaces(ref insideMesh);

        //Combine the meshes
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(
            new CombineInstance[]
            {
                new CombineInstance { mesh = insideMesh },
                new CombineInstance { mesh = sourceMesh }
            },
            true, false, false
        );

        Object.DestroyImmediate(insideMesh);

        //Create Asset
        AssetDatabase.CreateAsset(
            combinedMesh,
            System.IO.Path.Combine(
                "Assets", sourceMesh.name + " Double-Sided.asset"
                )
            );
    }

    //Flips the faces of the mesh and it's normals
    static void FlipMeshFaces(ref Mesh source)
    {
        int[] triangles = source.triangles;
        System.Array.Reverse(triangles);
        source.triangles = triangles;

        Vector3[] normals = source.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        source.normals = normals;
    }
    
}
