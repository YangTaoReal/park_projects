using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class LowPolyWater : MonoBehaviour {

    public Material material;
    public Camera _camera;
    public int sizeX = 30;
    public int sizeZ = 30;
    public float waveScale = 1;
    [Range(0, 1)]
    public float noise = 0;
    public enum GridType { Hexagonal, Square };
    public GridType gridType;

    bool generate;

    const int maxVerts = ushort.MaxValue;
    const float sin60 = 0.86602540378f;
    const float inv_tan60 = 0.57735026919f;

    // Deprecated
    [HideInInspector]
    public float scale = -1;
    [HideInInspector]
    public int size = -1; 

    void Start() {
        if (material == null || !material.HasProperty("_EdgeBlend")) return;
        if (material.GetFloat("_EdgeBlend") > 0.1f) {
            SetupCamera();
        }
    }

    void SetupCamera() {
        if (_camera == null) _camera = Camera.main;
        if(_camera != null)
        {
            _camera.depthTextureMode |= DepthTextureMode.Depth;
        }

    }

    void OnEnable() {
        // update deprecated parameters
        if(scale != -1 || size != -1) {
            sizeX = size;
            sizeZ = size;
            waveScale = scale;
            scale = -1;
            size = -1;
        }
        Scale(material, waveScale);
        generate = true;
        Generate();
    }

    public static void Scale(Material material, float scale) {
        if (material == null || !material.HasProperty("_Scale_")) return;
        material.SetFloat("_Scale_", scale);
        material.SetFloat("_RHeight_", material.GetFloat("_RHeight")* scale);
        material.SetFloat("_RSpeed_", material.GetFloat("_RSpeed")* scale);
        material.SetFloat("_Height_", material.GetFloat("_Height")* scale);
        material.SetFloat("_Speed_", material.GetFloat("_Speed")* scale);
        var noiseTex = material.GetTexture("_NoiseTex");
        if (noiseTex != null)
            material.SetFloat("_TexSize_", noiseTex.height * scale);
        var steepness = material.GetFloat("_Steepness") * material.GetFloat("_Length");
        var angle = Mathf.Deg2Rad * material.GetFloat("_Direction");
        var cos = Mathf.Cos(angle);
        var sin = Mathf.Sin(angle);
        material.SetVector("__Direction", new Vector4(cos, sin, cos * steepness, sin * steepness));
    }


#if UNITY_EDITOR
    // the code below only compiles inside the editor!

    void OnValidate() {
        sizeX = Mathf.Clamp(sizeX, 1, 256);
        sizeZ = Mathf.Clamp(sizeZ, 1, 256);
        var mfs = GetComponentsInChildren<MeshFilter>();

        generate = GUI.changed || mfs == null || mfs.Length == 0;
        if (!generate) {
            for (int i = 0; i < mfs.Length; i++) {
                if (mfs[i].sharedMesh == null) {
                    generate = true;
                    break;
                }
            }
        }
    }

    void Update() {
        Generate();
    }

#endif

    void OnDestroy() {
        CleanUp();
    }

    void CleanUp() {
        // clear all previous objects
        var mfs = GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < mfs.Length; i++) {
            if (Application.isPlaying) {
                Destroy(mfs[i].gameObject);
            } else {
                DestroyImmediate(mfs[i].gameObject);
            }
        }
    }

    #region Generation code

    void Generate() {
        if (material == null || !material.HasProperty("_EdgeBlend")) return;

        if (material.GetFloat("_EdgeBlend") > 0.1f) {
            SetupCamera();
        }

        Scale(material, waveScale);

        if (!generate) return;
        generate = false;
        if (gridType == GridType.Hexagonal) {
            GenerateHexagonal();
        } else {
            GenerateSquare();
        }
    }

    float Encode(Vector3 v) {
        var uv0 = Mathf.Round((v.x + 5) * 10000f);
        var uv1 = Mathf.Round((v.z + 5) * 10000f) / 100000f;
        return uv0 + uv1;
    }

    void BakeMesh(List<Vector3> verts, List<int> inds, float rotation = 0f) {
        var uvs = new List<Vector2>(inds.Count);
        var splitIndices = new List<int>(inds.Count);
        var splitVertices = new List<Vector3>(inds.Count);

        for (int i = 0; i < inds.Count; i += 3) {
            splitIndices.Add(i % maxVerts);
            splitIndices.Add((i + 1) % maxVerts);
            splitIndices.Add((i + 2) % maxVerts);

            var v0 = verts[inds[i]];
            var v1 = verts[inds[i + 1]];
            var v2 = verts[inds[i + 2]];

            splitVertices.Add(v0);
            splitVertices.Add(v1);
            splitVertices.Add(v2);

            var uv = new Vector2();
            uv.x = Encode(v0 - v1);
            uv.y = Encode(v0 - v2);
            uvs.Add(uv);

            uv.x = Encode(v1 - v2);
            uv.y = Encode(v1 - v0);
            uvs.Add(uv);

            uv.x = Encode(v2 - v0);
            uv.y = Encode(v2 - v1);
            uvs.Add(uv);
        }

        CleanUp();

        int numGO = Mathf.CeilToInt(splitVertices.Count / (float)maxVerts);
        for (int i = 0, pos = 0; i < numGO; i++, pos += maxVerts) {
            var go = new GameObject("WaterChunk");
            if (gameObject != null) go.layer = gameObject.layer;
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(0, rotation, 0);
            go.transform.localScale = Vector3.one;
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = material;
            mr.receiveShadows = false;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var mesh = new Mesh();
            mesh.name = "WaterChunk";

            var len = i == numGO - 1 ? splitVertices.Count - pos : maxVerts;

            mesh.SetVertices(splitVertices.GetRange(pos, len));
            mesh.SetTriangles(splitIndices.GetRange(pos, len), 0);
            mesh.SetUVs(0, uvs.GetRange(pos, len));
            mesh.hideFlags = HideFlags.HideAndDontSave;
            mf.mesh = mesh;
            go.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void Add(List<Vector3> verts, Vector3 toAdd, float delta) {
        if (noise > 0) {
            var n = UnityEngine.Random.insideUnitCircle * noise * delta / 2f;
            toAdd.x += n.x;
            toAdd.z += n.y;
        }
        verts.Add(toAdd);
    }

    //int sizeX, sizeZ;  //todo

    void GenerateSquare() {
        var verts = new List<Vector3>();
        var inds = new List<int>();
        //90degr rot
        var numVertsX = sizeX*2;
        var numVertsZ = sizeZ*2;
        var delta = sin60;
        var deltaX = Vector3.right * delta;
        var vO = new Vector3(-sizeX * sin60, 0, -sizeZ * sin60);

        for (int j = 0; j < numVertsZ + 1; j++) {
            bool reverse = j % 2 != 0;
            var v = vO + Vector3.forward * j * delta;
            int cols = numVertsX + (reverse ? 2 : 1);
            for (int i = 0; i < cols; i++) {
                Add(verts, v, delta);
                if (reverse && (i == 0 || i == cols - 2)) {
                    v += deltaX / 2f;
                } else {
                    v += deltaX;
                }
            }
        }
        int iCur = 0;
        for (int j = 0; j < numVertsZ; j++) {
            bool reverse = j % 2 != 0;
            int ofs = numVertsX + (reverse ? 2 : 1); 
            int cols = numVertsX + (reverse ? 0 : 0);

            int iForw = iCur + ofs;

            for (int i = 0; i < cols; i++) {
                int iRight = iCur + 1;
                int iForwRight = iForw + 1;

                inds.Add(iCur);
                if (reverse) {
                    inds.Add(iForw);
                    inds.Add(iRight);
                    inds.Add(iForw);
                    inds.Add(iForwRight);
                    inds.Add(iRight);
                } else {
                    inds.Add(iForwRight);
                    inds.Add(iRight);
                    inds.Add(iCur);
                    inds.Add(iForw);
                    inds.Add(iForwRight);
                }
                iCur = iRight;
                iForw = iForwRight;
            }
            inds.Add(iCur);
            if (reverse) {
                inds.Add(iForw);
                inds.Add(iCur + 1);
                iCur += 2;
            } else {
                inds.Add(iForw);
                inds.Add(iForw + 1);
                iCur++;
            }
        }

        BakeMesh(verts, inds);
    }

    void GenerateHexagonal() {
        var verts = new List<Vector3>();
        var inds = new List<int>();

        float delta = size / size;
        int vertIndex = 0;
        int curNumPoints = 0;
        int prevNumPoints = 0;
        int numPointsCol0 = sizeX+sizeZ + 1;
        int colMin = -sizeX;
        int colMax = sizeX;

        for (int i = colMin; i <= colMax; i++) {
            float x = sin60 * delta * i;

            int numPointsColi = numPointsCol0 - Mathf.Abs(i);

            int rowMin = -(sizeZ+sizeX)/2;
            if (i < 0) rowMin += Mathf.Abs(i);

            int rowMax = rowMin + numPointsColi - 1;

            curNumPoints += numPointsColi;

            for (int j = rowMin; j <= rowMax; j++) {
                float z = inv_tan60 * x + delta * j;

                var v = new Vector3(x, 0, z);
                if (noise > 0) {
                    var n = UnityEngine.Random.insideUnitCircle * noise * delta / 2f;
                    v.x += n.x;
                    v.z += n.y;
                }
                verts.Add(v);

                if (vertIndex < (curNumPoints - 1)) {
                    if (i >= colMin && i < colMax) {
                        int padLeft = 0;
                        if (i < 0) padLeft = 1;
                        inds.Add(vertIndex);
                        inds.Add(vertIndex + 1);
                        inds.Add(vertIndex + numPointsColi + padLeft);
                    }

                    if (i > colMin && i <= colMax) {
                        int padRight = 0;
                        if (i > 0) padRight = 1;
                        inds.Add(vertIndex + 1);
                        inds.Add(vertIndex);
                        inds.Add(vertIndex - prevNumPoints + padRight);
                    }
                }

                vertIndex++;
            }

            prevNumPoints = numPointsColi;
        }

        BakeMesh(verts, inds);
    }
    #endregion

}

