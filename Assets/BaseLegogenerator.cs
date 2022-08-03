using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLegogenerator : MonoBehaviour
{
    public float scale = 0.4f;
    public float allowlimit = 0.01f;
    public bool isplacer = false;
    public float maxreach;

    /// <summary>
    /// 연결부위들의 위치와 정보 구현
    /// </summary>
    public Dictionary<int, List<Vector3>> inoutpeak;

    /// <summary>
    /// 구현에 필요한 meshFilter와 boxCollider
    /// </summary>
    protected MeshFilter meshFilter; //메쉬 생성
    protected List<Collider> totalcolliders = new List<Collider>(); //일반 collider
    protected List<Collider> cylcolliders = new List<Collider>(); //원통 collider
    protected GameObject Legoconnector; //cyl 오브젝트 생성
    protected Mesh mesh; //메쉬 만들기
    protected GameObject Legooutline; //outline으로 블록 위치시키는 오브젝트


    /// <summary>
    /// 피크 평면 구현
    /// </summary>
    public class peakplane
    {
        public Vector3 zeroPoint = new Vector3(0, 0, 0);
        public Vector3 normalPlane = new Vector3(0, 1, 0);
        public int possiblePeakTyle;
        public List<int> peaklist = new List<int>();

        public peakplane()
        {

        }
        public peakplane(Vector3 zeroPoint, Vector3 normalPlane)
        {
            this.zeroPoint = zeroPoint;
            this.normalPlane = normalPlane;
        }
        public peakplane(Vector3 zeroPoint, Vector3 normalPlane, int possiblePeakTyle)
        {
            this.zeroPoint = zeroPoint;
            this.normalPlane = normalPlane;
            this.possiblePeakTyle = possiblePeakTyle;
        }
        public peakplane(Vector3 zeroPoint, Vector3 normalPlane, int possiblePeakTyle, List<int> peaklist)
        {
            this.zeroPoint = zeroPoint;
            this.normalPlane = normalPlane;
            this.possiblePeakTyle = possiblePeakTyle;
            this.peaklist = peaklist;
        }
    }
    public List<peakplane> peakplanelist = new List<peakplane>();


    public Mesh FullCylinder(float cylr, float cylh, Vector3 basepos, int polygon)
    {
        Mesh mesh = new Mesh();
        Vector3[] peak_vertices = new Vector3[polygon * 2];
        for (int j = 0; j < polygon; j++)
        {
            peak_vertices[j] = basepos + new Vector3((cylr - allowlimit) * Mathf.Cos((float)j / polygon * 2 * Mathf.PI), allowlimit, (cylr - allowlimit) * Mathf.Sin((float)j / polygon * 2 * Mathf.PI)); //원통 아랫부분
            peak_vertices[j + polygon] = peak_vertices[j] + new Vector3(0, (cylh - allowlimit), 0); // 원통 윗부분
        }
        mesh.vertices = peak_vertices;
        return mesh;

    }

    public Mesh FullCube(float length, float height, float width, Vector3 basepos)
    {
        return new Mesh();
    }

    /// <summary>
    /// 메쉬로 피크 구현
    /// </summary>
    /// <param name="peak"></param>
    /// <param name="least_start"></param>
    /// <returns></returns>
    protected virtual (Vector3[], List<int>, int) PeakGenerator(int[] peak, int least_start)
    {
        return (new Vector3[1], new List<int>(), 0);
    }

    /// <summary>
    /// 메쉬로 하부측 구멍 구현
    /// </summary>
    /// <param name="hole"></param>
    /// <param name="least_start"></param>
    /// <returns></returns>
    protected virtual (Vector3[], List<int>, int) HoleGenerator(int[] hole, int least_start)
    {
        return (new Vector3[1], new List<int>(), 0);
    }

    /// <summary>
    /// 메쉬로 외곽부 구현
    /// </summary>
    /// <returns></returns>
    protected virtual (Vector3[], List<int>, int) OutlineGenerator()
    {
        return (new Vector3[1], new List<int>(), 0);
    }

    /// <summary>
    /// 외곽부 좌표 반환
    /// </summary>
    /// <returns></returns>
    public virtual GameObject OutlineInfo()
    {
        return new GameObject();
    }

    /// <summary>
    /// 피크 정보 계산
    /// </summary>
    /// <param name="peak"></param>
    public virtual void inout()
    {

    }


    /// <summary>
    /// 전체 메쉬 구현
    /// </summary>
    /// <param name="meshFilter"></param>
    protected virtual void TotalGenerator(MeshFilter meshFilter)
    {

    }

    /// <summary>
    /// 모양이 바뀌게 되면 바뀐 정보 구현
    /// </summary>
    /// <param name="setters"></param>
    public virtual void ChangeShape(params object[] setters)
    {

    }

    public virtual void MeshColliderGenerator()
    {

    }

    public virtual void MeshColliderGenerator(GameObject realblego)
    {

    }
    public virtual void MeshColliderDeleter()
    {

    }
    public virtual void MeshColliderDeleter(GameObject realblego)
    {

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
