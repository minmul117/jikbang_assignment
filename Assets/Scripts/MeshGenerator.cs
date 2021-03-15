using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public enum Face
    {
        Front,
        UpAndDown,
        LeftAndRight
    }

    public enum Plane
    {
        XY,
        XZ,
        YZ
    }

    public Material CommonMaterial;
    Vector3[] vertices;
    int[] triangles;
    List<Mesh> meshes;
    List<GameObject> objects;
    // Start is called before the first frame update
    void Start()
    {
        meshes = new List<Mesh>();
    }

    public void AddMesh(Mesh mesh)
    {
        if (meshes == null)
        {
            meshes = new List<Mesh>();
        }

        if (objects == null)
        {
            objects = new List<GameObject>();
        }

        var newObject = new GameObject();
        newObject.AddComponent<MeshFilter>();
        newObject.AddComponent<MeshRenderer>();
        newObject.GetComponent<MeshFilter>().mesh = mesh;
        newObject.GetComponent<MeshRenderer>().material = CommonMaterial;

        meshes.Add(mesh);
        objects.Add(newObject);
    }

    public void ProcessUVs()
    {
        for (var i = 0; i < meshes.Count; i++)
        {
            var mesh = meshes[i];
            var meshVertices = mesh.vertices;
            var uvs = new Vector2[meshVertices.Length];
            var normals = mesh.normals;
            var gameObject = objects[i];

            for (var j = 0; j < meshVertices.Length; j++)
            {
                var norm0 = normals[j];
                var firstDir = FindDirection(norm0, gameObject);

                if (firstDir == Face.UpAndDown)
                {
                    // (min, max)
                    var minMaxValues = GetMinMaxVerticesByPlane(mesh, Plane.XZ);
                    var xNormalized = Mathf.InverseLerp(minMaxValues[0].x, minMaxValues[0].y, meshVertices[j].x);
                    var xInversedNormalized = (Mathf.Lerp(768.0f, 1024.0f, xNormalized)) / 1024.0f;

                    var zNormalized = Mathf.InverseLerp(minMaxValues[1].x, minMaxValues[1].y, meshVertices[j].z);
                    var zInversedNormalized = Mathf.Lerp(0.0f, 512.0f, zNormalized) / 512.0f;

                    uvs[j] = new Vector2(xInversedNormalized, zInversedNormalized);
                }
                else if (firstDir == Face.Front)
                {
                    var minMaxValues = GetMinMaxVerticesByPlane(mesh, Plane.XY);
                    var xNormalized = Mathf.InverseLerp(minMaxValues[0].x, minMaxValues[0].y, meshVertices[j].x);
                    var xInversedNormalized = Mathf.Lerp(0.0f, 512.0f, xNormalized) / 1024.0f;

                    var yNormalized = Mathf.InverseLerp(minMaxValues[1].x, minMaxValues[1].y, meshVertices[j].y);
                    var yInversedNormalized = Mathf.Lerp(0.0f, 512.0f, yNormalized) / 512.0f;

                    uvs[j] = new Vector2(xInversedNormalized, yInversedNormalized);
                }
                else if (firstDir == Face.LeftAndRight)
                {
                    var minMaxValues = GetMinMaxVerticesByPlane(mesh, Plane.YZ);
                    var yNormalized = Mathf.InverseLerp(minMaxValues[0].x, minMaxValues[0].y, meshVertices[j].y);
                    var yInversedNormalized = Mathf.Lerp(0.0f, 512.0f, yNormalized) / 512.0f;

                    var zNormalized = Mathf.InverseLerp(minMaxValues[1].x, minMaxValues[1].y, meshVertices[j].z);
                    var zInversedNormalized = Mathf.Lerp(512.0f, 768.0f, zNormalized) / 1024.0f;

                    uvs[j] = new Vector2(zInversedNormalized, yInversedNormalized);
                }
            }

            mesh.uv = uvs;
        }
    }

    Vector2[] GetMinMaxVerticesByPlane(Mesh mesh, Plane plane)
    {
        var results = new Vector2[2];
        var meshVertices = mesh.vertices;
        switch (plane)
        {
            case Plane.XZ:
                results[0] = new Vector2(
                    meshVertices.Min(v => v.x),
                    meshVertices.Max(v => v.x)
                );
                results[1] = new Vector2(
                    meshVertices.Min(v => v.z),
                    meshVertices.Max(v => v.z)
                );
                break;
            case Plane.XY:
                results[0] = new Vector2(
                    meshVertices.Min(v => v.x),
                    meshVertices.Max(v => v.x)
                );
                results[1] = new Vector2(
                    meshVertices.Min(v => v.y),
                    meshVertices.Max(v => v.y)
                );
                break;
            case Plane.YZ:
                results[0] = new Vector2(
                    meshVertices.Min(v => v.y),
                    meshVertices.Max(v => v.y)
                );
                results[1] = new Vector2(
                    meshVertices.Min(v => v.z),
                    meshVertices.Max(v => v.z)
                );
                break;
        }
        return results;
    }

    Face FindDirection(Vector3 normal, GameObject obj)
    {
        if (Mathf.Abs(normal.y) > 0.5f)
        {
            return Face.UpAndDown;
        }
        else
        {
            // object space transform, maybe?
            var worldNormal = obj.transform.TransformDirection(normal);
            var angle = Vector3.SignedAngle(obj.transform.forward, worldNormal, obj.transform.up) + 180.0f;
            var offsiteAngle = Vector3.SignedAngle(-obj.transform.forward, worldNormal, obj.transform.up) + 180.0f;
            if ((180.0f <= angle && angle <= 220.0f) || (180.0f <= offsiteAngle && offsiteAngle <= 220.0f))
            {
                return Face.Front;
            }
            else
            {
                return Face.LeftAndRight;
            }
        }
    }
}
