using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class scr_GenerateMesh : MonoBehaviour
{
    public int mDivisions = 64;   // resolución del terreno
    public float mSize = 10f;     // tamaño físico
    public float mAmplitude = 2f; // altura máxima

    private Vector3[] _mVerts;
    private int _mVertCount;
    private scr_DiamondSquare ds;

    void Start()
    {
        ds = GetComponent<scr_DiamondSquare>();
        SetHeightmap(); // ahora arranca con terreno generado
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            SetHeightmap(); // regenerar terreno presionando T
    }

    private void SetHeightmap()
    {
        float[,] hm = ds.Generate(mDivisions + 1, mDivisions + 1, mDivisions / 2, mAmplitude);

        _mVertCount = (mDivisions + 1) * (mDivisions + 1);
        _mVerts = new Vector3[_mVertCount];
        Vector2[] uvs = new Vector2[_mVertCount];
        int[] tris = new int[mDivisions * mDivisions * 2 * 3];

        float halfSize = mSize * 0.5f;
        float divisionSize = mSize / mDivisions;

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // 🔑 soporta más de 65k vértices
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Terrain";

        int triOffset = 0;

        for (int i = 0; i <= mDivisions; i++)
        {
            for (int j = 0; j <= mDivisions; j++)
            {
                _mVerts[i * (mDivisions + 1) + j] =
                    new Vector3(-halfSize + j * divisionSize, hm[i, j], halfSize - i * divisionSize);

                uvs[i * (mDivisions + 1) + j] =
                    new Vector2((float)i / mDivisions, (float)j / mDivisions);

                if (i < mDivisions && j < mDivisions)
                {
                    int topLeft = i * (mDivisions + 1) + j;
                    int botLeft = (i + 1) * (mDivisions + 1) + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = botLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = botLeft + 1;
                    tris[triOffset + 5] = botLeft;

                    triOffset += 6;
                }
            }
        }

        mesh.vertices = _mVerts;
        mesh.uv = uvs;
        mesh.triangles = tris;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
