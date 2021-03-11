using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public Material CommonMaterial;
    Vector3[] vertices;
    int[] triangles;
    List<Mesh> meshes;
    // Start is called before the first frame update
    void Start()
    {
        meshes = new List<Mesh>();
        UpdateMesh();
    }

    public List<Mesh> GetMeshes() { return meshes; }

    public void AddMesh(Mesh mesh)
    {
        if (meshes == null)
        {
            meshes = new List<Mesh>();
        }
        GameObject newObject = new GameObject();
        newObject.AddComponent<MeshFilter>();
        newObject.AddComponent<MeshRenderer>();
        newObject.GetComponent<MeshFilter>().mesh = mesh;
        newObject.GetComponent<MeshRenderer>().material = CommonMaterial;
        meshes.Add(mesh);
    }

    void UpdateMesh()
    {
        foreach (var mesh in meshes)
        {
            mesh.RecalculateNormals();
        }
    }
}
