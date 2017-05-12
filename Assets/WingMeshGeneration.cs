using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WingMeshGeneration : MonoBehaviour {

    [SerializeField]
    protected float radius = 10;
    [SerializeField]
    protected float halfAnglePositive = 30;
    [SerializeField]
    protected float halfAngleNegative = -60;
    [SerializeField]
    protected float offset = 2;
    [SerializeField]
    protected float thicknessBegin = 4;
    [SerializeField]
    protected AnimationCurve thicknessEnd = AnimationCurve.Linear(0, 6, 1, 6);
    [SerializeField]
    protected float animationCycleTime = 4;

    [SerializeField]
    protected int numVertexSegments = 32;

	// Use this for initialization
	void Update () {

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();

        Vector3[] vertices = new Vector3[numVertexSegments * 4];
        Vector2[] uvs = new Vector2[numVertexSegments * 4];
        int[] triangles = new int[(numVertexSegments - 1) * 12];

        //start on the right wing first, then mirror data to make the left wing
        Vector3 center = (radius - offset) * Vector3.left;

        //top edge of the right wing
        //inner vertex first, then outer vertex
        vertices[0] = center + (Vector3)(halfAnglePositive.DegreeToVector2() * radius);
        vertices[1] = center + (Vector3)(halfAnglePositive.DegreeToVector2() * (radius + thicknessBegin));

        uvs[0] = Vector2.zero;
        uvs[1] = Vector2.up;

        for(int i = 1; i < numVertexSegments; i++) {
            int vertexIndexBase = 2 * i;
            float interpolationValue = ((float)i) / (numVertexSegments - 1); //progress along the wing, range [0..1]
            float angle = Mathf.Lerp(halfAnglePositive, halfAngleNegative, interpolationValue);
            Vector3 direction = angle.DegreeToVector2();

            vertices[vertexIndexBase] = center + (direction * radius);
            vertices[vertexIndexBase + 1] = center + (direction * (radius + Mathf.Lerp(thicknessBegin, thicknessEnd.Evaluate((Time.time / animationCycleTime) % 1), interpolationValue)));

            uvs[vertexIndexBase] = new Vector2(interpolationValue, 0);
            uvs[vertexIndexBase + 1] = new Vector2(interpolationValue, 1);

            int triangleIndexBase = 6 * (i - 1);
            triangles[triangleIndexBase] = vertexIndexBase - 2;
            triangles[triangleIndexBase + 1] = vertexIndexBase - 1;
            triangles[triangleIndexBase + 2] = vertexIndexBase;

            triangles[triangleIndexBase + 3] = vertexIndexBase - 1;
            triangles[triangleIndexBase + 4] = vertexIndexBase + 1;
            triangles[triangleIndexBase + 5] = vertexIndexBase;
        }

        int halfVertexIndex = vertices.Length / 2;
        for(int i = 0; i < halfVertexIndex; i++) {
            vertices[i + halfVertexIndex] = new Vector3(-vertices[i].x, vertices[i].y);
            uvs[i + halfVertexIndex] = uvs[i];
        }

        int halfTriangleIndex = triangles.Length / 2;
        for(int i = 0; i < halfTriangleIndex; i+= 6) {
            triangles[halfTriangleIndex + i] = triangles[i] + halfVertexIndex;
            triangles[halfTriangleIndex + i + 1] = triangles[i + 2] + halfVertexIndex; //swap two so that triangle normals still face the same way
            triangles[halfTriangleIndex + i + 2] = triangles[i + 1] + halfVertexIndex;

            triangles[halfTriangleIndex + i + 3] = triangles[i + 3] + halfVertexIndex;
            triangles[halfTriangleIndex + i + 4] = triangles[i + 5] + halfVertexIndex; // again, swap order of two triangles for normal direction
            triangles[halfTriangleIndex + i + 5] = triangles[i + 4] + halfVertexIndex;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        //this.enabled = false;
	}
}
