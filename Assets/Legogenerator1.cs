using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legogenerator1 : BaseLegogenerator
{
    /// <summary>
    /// 물체에 대한 기본 길이 정보
    /// </summary>
    public int length = 2; //x
    public int height = 1; //y
    public int width = 2; //z
    public float realw = 0.8f;
    public float realh = 0.96f;
    public float cylr = 0.24f;
    public float cylh = 0.16f;
    public float thickw = 0.16f;
    public float thickh = 0.16f;
    public float lrealw = 0.79f;
    public float? holeo = null;
    public int polygon = 24; //원 구현에 필요한 다각형


    protected override (Vector3[], List<int>, int) PeakGenerator(int[] peak, int least_start)
    {
        Vector3[] peak_vertices = new Vector3[polygon * 2 * peak[0] * peak[1]]; //필요한 꼭짓점
        for (int i = 0; i < peak[0] * peak[1]; i++)
        {
            Vector3 base_center = new Vector3((-(peak[0] - 1) / 2f + i % peak[0]) * realw,
                                              height / 2f * realh,
                                              (-(peak[1] - 1) / 2f + i / peak[0]) * realw); // 중앙
            for (int j = 0; j < polygon; j++)
            {
                peak_vertices[j + i * 2 * polygon] = base_center +
                                                     new Vector3(cylr * Mathf.Cos((float)j / polygon * 2 * Mathf.PI),
                                                                 0,
                                                                 cylr * Mathf.Sin((float)j / polygon * 2 * Mathf.PI)); //원통 아랫부분
                peak_vertices[j + polygon + i * 2 * polygon] = peak_vertices[j + i * 2 * polygon] +
                                                               new Vector3(0, cylh, 0); // 원통 윗부분
            }
            //
        }
        List<int> peak_triangles = new List<int>();
        for (int i = 0; i < peak[0] * peak[1]; i++)
        {

            for (int j = 0; j < polygon - 1; j++)
            {
                peak_triangles.Add(least_start + j + 1 + i * 2 * polygon);
                peak_triangles.Add(least_start + j + i * 2 * polygon);
                peak_triangles.Add(least_start + j + polygon + i * 2 * polygon);
                peak_triangles.Add(least_start + j + 1 + i * 2 * polygon);
                peak_triangles.Add(least_start + j + polygon + i * 2 * polygon);
                peak_triangles.Add(least_start + j + 1 + polygon + i * 2 * polygon);
            }
            peak_triangles.Add(least_start + i * 2 * polygon);
            peak_triangles.Add(least_start + polygon - 1 + i * 2 * polygon);
            peak_triangles.Add(least_start + polygon * 2 - 1 + i * 2 * polygon);
            peak_triangles.Add(least_start + i * 2 * polygon);
            peak_triangles.Add(least_start + polygon * 2 - 1 + i * 2 * polygon);
            peak_triangles.Add(least_start + polygon + i * 2 * polygon);
            for (int j = 2; j < polygon; j++)
            {
                peak_triangles.Add(least_start + polygon + j + i * 2 * polygon);
                peak_triangles.Add(least_start + polygon + j - 1 + i * 2 * polygon);
                peak_triangles.Add(least_start + polygon + i * 2 * polygon);
            }
        }
        return (peak_vertices, peak_triangles, polygon * 2 * peak[0] * peak[1]);
    }

    protected override (Vector3[], List<int>, int) HoleGenerator(int[] hole, int least_start)
    {
        if (holeo == null)
        {
            holeo = realw * Mathf.Sqrt(2) / 2 - cylr;
        }

        Vector3[] peak_vertices = new Vector3[polygon * 4 * hole[0] * hole[1]];
        for (int i = 0; i < hole[0] * hole[1]; i++)
        {
            Vector3 base_center = new Vector3((-(hole[0] - 1) / 2f + i % hole[0]) * realw,
                                              -height / 2f * realh,
                                              (-(hole[1] - 1) / 2f + i / hole[0]) * realw); // 중앙

            for (int j = 0; j < polygon; j++)
            {
                peak_vertices[j + i * 4 * polygon] = base_center +
                    new Vector3((float)holeo * Mathf.Cos((float)j / polygon * 2 * Mathf.PI),
                    0,
                    (float)holeo * Mathf.Sin((float)j / polygon * 2 * Mathf.PI)); //원통 아랫부분 바깥
                peak_vertices[j + polygon + i * 4 * polygon] = base_center +
                    new Vector3(cylr * Mathf.Cos((float)j / polygon * 2 * Mathf.PI),
                    0,
                    cylr * Mathf.Sin((float)j / polygon * 2 * Mathf.PI)); //원통 아랫부분 안
                peak_vertices[j + 2 * polygon + i * 4 * polygon] = peak_vertices[j + i * 4 * polygon] +
                    new Vector3(0, realh * height - thickh, 0); // 원통 윗부분 바깥
                peak_vertices[j + 3 * polygon + i * 4 * polygon] = peak_vertices[j + i * 4 * polygon + polygon] +
                    new Vector3(0, realh * height - thickh, 0); // 원통 윗부분 안
            }

        }
        List<int> peak_triangles = new List<int>();
        for (int i = 0; i < hole[0] * hole[1]; i++)
        {

            for (int j = 0; j < polygon - 1; j++)
            {
                peak_triangles.Add(least_start + j + 1 + i * 4 * polygon);
                peak_triangles.Add(least_start + j + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 2 * polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 1 + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 2 * polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 1 + 2 * polygon + i * 4 * polygon); // 바깥

                peak_triangles.Add(least_start + j + polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 1 + polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 3 * polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 3 * polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 1 + polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 1 + 3 * polygon + i * 4 * polygon); // 안
            }
            peak_triangles.Add(least_start + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon * 3 - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon * 3 - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + 2 * polygon + i * 4 * polygon);


            peak_triangles.Add(least_start + 2 * polygon - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon * 4 - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon * 4 - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon + i * 4 * polygon);
            peak_triangles.Add(least_start + 3 * polygon + i * 4 * polygon);

            for (int j = 0; j < polygon - 1; j++)
            {
                peak_triangles.Add(least_start + j + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 1 + i * 4 * polygon);
                peak_triangles.Add(least_start + j + polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + polygon + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 1 + i * 4 * polygon);
                peak_triangles.Add(least_start + j + 1 + polygon + i * 4 * polygon);
            }
            peak_triangles.Add(least_start + polygon - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon * 2 - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon * 2 - 1 + i * 4 * polygon);
            peak_triangles.Add(least_start + i * 4 * polygon);
            peak_triangles.Add(least_start + polygon + i * 4 * polygon);
        }
        return (peak_vertices, peak_triangles, polygon * 4 * hole[0] * hole[1]);
    }

    protected override (Vector3[], List<int>, int) OutlineGenerator()
    {
        Vector3[] base_vertices = new Vector3[16];
        for (int i = 0; i < 8; i++) // 외곽과 두께

        {
            base_vertices[i] = new Vector3((((i >> 0) % 2) - 0.5f) * (lrealw * 2 + realw * (length - 2)),
                                      (((i >> 1) % 2) - 0.5f) * realh * height,
                                      (((i >> 2) % 2) - 0.5f) * (lrealw * 2 + realw * (width - 2)));
            base_vertices[i + 8] = new Vector3((((i >> 0) % 2) - 0.5f) * ((lrealw - thickw) * 2 + realw * (length - 2)),
                                      (((i >> 1) % 2) - 0.5f) * realh * height - (i >> 1) % 2 * thickh,
                                      (((i >> 2) % 2) - 0.5f) * ((lrealw - thickw) * 2 + realw * (width - 2)));
        }
        int[] base_base_triangles = new int[30 + 30 + 24] { 2, 1, 0, 1, 2, 3, 4, 2, 0, 2, 4, 6, 1, 3, 5, 7, 5, 3, 6, 3, 2, 3, 6, 7, 4, 5, 6, 7, 6, 5, //외곽
                                              13, 14, 15, 14, 13, 12, 15, 14, 11, 10, 11, 14, 11, 13, 15, 13, 11, 9, 14, 12, 10, 8, 10, 12, 11, 10, 9, 8, 9, 10, // 두께
                                              0,1,8,9,8,1,8,4,0,4,8,12,13,5,4,4,12,13,1,5,9,13,9,5};

        List<int> base_triangles = new List<int>(base_base_triangles);

        return (base_vertices, base_triangles, 16);
    }

    public override GameObject OutlineInfo() //아랫쪽
    {
        Destroy(Legooutline);
        Instantiate(Legooutline);
        Legooutline.layer = LayerMask.NameToLayer("LegoPlacer");
        MeshCollider dummy = Legooutline.AddComponent<MeshCollider>();
        Mesh dummymesh = new Mesh();
        dummy.convex = false;
        dummy.isTrigger = true;
        Vector3[] Realoutline = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            Realoutline[i] = gameObject.transform.rotation * new Vector3((((i >> 0) % 2) - 0.5f) * (lrealw * 2 + realw * (length - 2)),
                                      (((i >> 1) % 2) - 0.5f) * realh * height + ((i >> 1) % 2) * cylh,
                                      (((i >> 2) % 2) - 0.5f) * (lrealw * 2 + realw * (width - 2)));
        }
        dummymesh.vertices = Realoutline;
        dummy.sharedMesh = dummymesh;
        return Legooutline;
    }


    public override void inout()
    {
        int[] pp = new int[2] { length, width };
        inoutpeak = new Dictionary<int, List<Vector3>>
        {
            { 0, new List<Vector3>()},
            { 1, new List<Vector3>()}
        };
        peakplanelist = new List<peakplane>();
        peakplanelist.Add(new peakplane(
            new Vector3(0, -height / 2f * realh, 0),
            gameObject.transform.rotation * new Vector3(0, -1, 0),
            0
            ));
        peakplanelist.Add(new peakplane(
            new Vector3(0, height / 2f * realh, 0),
            gameObject.transform.rotation * new Vector3(0, 1, 0),
            1
            ));
        for (int i = 0; i < pp[0] * pp[1]; i++)
        {

            inoutpeak[0].Add(gameObject.transform.rotation * new Vector3((-(pp[0] - 1) / 2f + i % pp[0]) * realw,
                -height / 2f * realh,
                (-(pp[1] - 1) / 2f + i / pp[0]) * realw)); // 중앙
            inoutpeak[1].Add(gameObject.transform.rotation * new Vector3((-(pp[0] - 1) / 2f + i % pp[0]) * realw,
                height / 2f * realh,
                (-(pp[1] - 1) / 2f + i / pp[0]) * realw)); // 중앙

            peakplanelist[0].peaklist.Add(i);
            peakplanelist[1].peaklist.Add(i);
        }
    }

    protected override void TotalGenerator(MeshFilter meshFilter)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("LegoConnect"))
            {
                Destroy(child.gameObject);
            }
        }
        inoutpeak = new Dictionary<int, List<Vector3>>
        {
            { 0, new List<Vector3>()},
            { 1, new List<Vector3>()}
        };

        GameObject subgame;
        if (isplacer)
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            subgame = gameObject;
        }
        else
        {
            Legoconnector = Instantiate(Resources.Load<GameObject>("nothing"),
                new Vector3(0, realh * height / 2f + cylh / 2f, 0),
                Quaternion.identity);
            Legoconnector.name = "cyl";
            Legoconnector.transform.SetParent(this.transform, false);
            Legoconnector.layer = LayerMask.NameToLayer("LegoConnect");
            gameObject.layer = LayerMask.NameToLayer("LegoBase");
            subgame = Legoconnector;
        }

        maxreach = Vector3.Magnitude(new Vector3(lrealw + realw * (length - 2) / 2f,
            realh * height / 2f + cylh,
            lrealw + realw * (width - 2) / 2f));
        BoxCollider dummy1 = gameObject.AddComponent<BoxCollider>();
        dummy1.center = new Vector3(0, 0, 0);
        dummy1.size = new Vector3(lrealw * 2 + realw * (length - 2),
            realh * height,
            lrealw * 2 + realw * (width - 2));
        totalcolliders.Add(dummy1);


        BoxCollider subbox = subgame.AddComponent<BoxCollider>();

        if (isplacer)
        {
            subbox.center = new Vector3(0, realh * height / 2f + cylh / 2f, 0);
        }
        else
        {
            subbox.center = new Vector3(0, 0, 0);
        }
        subbox.size = new Vector3(lrealw * 2 + realw * (length - 2), cylh, lrealw * 2 + realw * (width - 2));
        cylcolliders.Add(subbox);

        mesh = new Mesh();
        int[] hole = new int[2] { length - 1, width - 1 };
        int[] peak = new int[2] { length, width };
        int least_start = 0;
        int dummy = 0;
        inout();

        Vector3[] base_vertices, peak_vertices, hole_vertices;
        List<int> base_triangles, peak_triangles, hole_triangles;

        (base_vertices, base_triangles, dummy) = OutlineGenerator();
        least_start += dummy;
        (peak_vertices, peak_triangles, dummy) = PeakGenerator(peak, least_start);
        least_start += dummy;
        (hole_vertices, hole_triangles, dummy) = HoleGenerator(hole, least_start);

        List<Vector3> vertices = new List<Vector3>();
        vertices.AddRange(base_vertices);
        vertices.AddRange(peak_vertices);
        vertices.AddRange(hole_vertices);

        List<int> triangles = new List<int>();
        triangles.AddRange(base_triangles);
        triangles.AddRange(peak_triangles);
        triangles.AddRange(hole_triangles);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.name = "wa sans";
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
    }

    /// <summary>
    /// 유니티 시작시 실행
    /// </summary>
    void Start()
    {
        MeshRenderer meshRenderer;
        if (isplacer)
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load<Material>("checking");
        }
        else
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load<Material>("dummy");
        }

        TotalGenerator(meshFilter);
    }

    /// <summary>
    /// 유니티 업데이트시 실행
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// 모양이 바뀌게 되면 바뀐 정보 구현
    /// </summary>
    /// <param name="l"></param>
    /// <param name="h"></param>
    /// <param name="w"></param>
    public override void ChangeShape(params object[] setters)
    {
        length = (int)setters[0];
        height = (int)setters[1];
        width = (int)setters[2];
        meshFilter.mesh.Clear(false);
        TotalGenerator(meshFilter);
    }


    public override void MeshColliderGenerator(GameObject realblego)
    {
        MeshCollider dummy;
        int[] hole = new int[2] { length - 1, width - 1 };
        int[] peak = new int[2] { length, width };
        for (int i = 0; i < peak[0] * peak[1]; i++)
        {
            Vector3 base_center = new Vector3((-(peak[0] - 1) / 2f + i % peak[0]) * realw,
                height / 2f * realh,
                (-(peak[1] - 1) / 2f + i / peak[0]) * realw); // 중앙

            dummy = realblego.AddComponent<MeshCollider>();
            dummy.convex = true;
            dummy.isTrigger = true;
            dummy.sharedMesh = FullCylinder(cylr, cylh, base_center, polygon);
        }
        for (int i = 0; i < hole[0] * hole[1]; i++)
        {
            Vector3 base_center = new Vector3((-(hole[0] - 1) / 2f + i % hole[0]) * realw,
                -height / 2f * realh,
                (-(hole[1] - 1) / 2f + i / hole[0]) * realw);

            dummy = realblego.AddComponent<MeshCollider>();
            dummy.convex = true;
            dummy.isTrigger = true;
            dummy.sharedMesh = FullCylinder((float)holeo, realh * height - thickh, base_center, polygon);
        }
        BoxCollider dummy2;
        dummy2 = realblego.AddComponent<BoxCollider>();
        dummy2.isTrigger = true;
        dummy2.center = new Vector3(0, height / 2f * realh - thickh / 2f, 0);
        dummy2.size = new Vector3(lrealw * 2 + realw * (length - 2) - 2 * allowlimit, thickh - 2 * allowlimit, lrealw * 2 + realw * (width - 2) - 2 * allowlimit);

        dummy2 = realblego.AddComponent<BoxCollider>();
        dummy2.isTrigger = true;
        dummy2.center = new Vector3(lrealw + realw * (length - 2) / 2f - thickw / 2f, -thickh / 2f + allowlimit, 0);
        dummy2.size = new Vector3(thickw - 2 * allowlimit, realh * height - thickh, lrealw * 2 + realw * (width - 2) - 2 * allowlimit);

        dummy2 = realblego.AddComponent<BoxCollider>();
        dummy2.isTrigger = true;
        dummy2.center = new Vector3(-lrealw - realw * (length - 2) / 2f + thickw / 2f, -thickh / 2f + allowlimit, 0);
        dummy2.size = new Vector3(thickw - 2 * allowlimit, realh * height - thickh, lrealw * 2 + realw * (width - 2) - 2 * allowlimit);

        dummy2 = realblego.AddComponent<BoxCollider>();
        dummy2.isTrigger = true;
        dummy2.center = new Vector3(0, -thickh / 2f + allowlimit, lrealw + realw * (width - 2) / 2f - thickw / 2f);
        dummy2.size = new Vector3(lrealw * 2 + realw * (length - 2) - 2 * thickw + 2 * allowlimit, realh * height - thickh, thickw - 2 * allowlimit);

        dummy2 = realblego.AddComponent<BoxCollider>();
        dummy2.isTrigger = true;
        dummy2.center = new Vector3(0, -thickh / 2f + allowlimit, -lrealw - realw * (width - 2) / 2f + thickw / 2f);
        dummy2.size = new Vector3(lrealw * 2 + realw * (length - 2) - 2 * thickw + 2 * allowlimit, realh * height - thickh, thickw - 2 * allowlimit);
    }

    public override void MeshColliderDeleter()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer != LayerMask.NameToLayer("LegoConnect"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}