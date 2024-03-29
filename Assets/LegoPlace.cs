using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LegoPlace : BaseLegogenerator
{
    /// <summary>
    /// 기본 정보 구현
    /// </summary>
    public float blockDis = 5.0f; //가상의 블록을 놓는 곳
    public float detectLim = 2f; //피크 탐지 배수
    public GameObject reallego;
    public GameObject Lset;
    public GameObject Hset;
    public GameObject Wset;
    public GameObject Rxset;
    public GameObject Ryset;
    public GameObject Rzset;
    public bool iscollided = false;


    /// <summary>
    /// 필요한 변수들
    /// </summary>
    GameObject blego; //중앙에 띄우는 레고
    MeshRenderer blegorenderer = new MeshRenderer(); //중앙 레고 매터리얼
    BaseLegogenerator outlineset; //외곽부
    int layermask; //ray에서 안볼 레이어만 거르기
    float realdetectLim; //legomod1에서 피크 탐지 구간
    TMP_InputField ChangeL, ChangeW, ChangeH, ChangeRx, ChangeRy, ChangeRz; //가로세로높이 변경
    int currentmod = 1; // legomod 결정
    int peaktype, targetpeaktype; //legomod1에서 가장 가까운 피크 결정
    Dictionary<int, int> minnumber, minnumber2; //그 피크의 번호 결정
    BaseLegogenerator targetlego; // 가장 가까운 레고의 정보
    Transform targetplace; // 그 레고의 위치
    Dictionary<int, List<Vector3>> peakset; //원래 레고의 피크 저장
    Dictionary<int, List<Vector3>> targetpeakset; //타겟 레고 피크 저장
    public Dictionary<int, List<int>> connectionlist = new Dictionary<int, List<int>>
    {
            { 0, new List<int>() { 1 } },
            { 1, new List<int>() { 0 } },
    }; //연결될 수 있는 피크끼리모으기
    float? d = null; //레고를 평면 위에 붙이기 위한 상수
    List<GameObject> oldchecker = new List<GameObject>(); //과거에 탐지했던 목록
    List<GameObject> newchecker = new List<GameObject>(); //지금 탐지된 목록
    bool alreadyrealblego = false; //탐지하기 위해 메쉬를 이미 만들어놨나?
    GameObject realblego; //붙일 레고의 메쉬 콜라이더 설정하는 오브젝트. 무조건 레이어는 Realistic

    /// <summary>
    /// Awake
    /// </summary>
    void Awake()
    {
        realdetectLim = 0.8f * scale * detectLim;
        layermask = (-1) - (1 << LayerMask.NameToLayer("Ignore Raycast")) - (1 << LayerMask.NameToLayer("Player")) - (1 << LayerMask.NameToLayer("Realistic"));
        ChangeL = Lset.GetComponent<TMP_InputField>();
        ChangeH = Hset.GetComponent<TMP_InputField>();
        ChangeW = Wset.GetComponent<TMP_InputField>();
        ChangeRx = Rxset.GetComponent<TMP_InputField>(); 
        ChangeRy = Ryset.GetComponent<TMP_InputField>();
        ChangeRz = Rzset.GetComponent<TMP_InputField>();
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        blego = Instantiate(reallego);
        outlineset = blego.GetComponent<BaseLegogenerator>();
        blegorenderer = blego.AddComponent<MeshRenderer>();
    }

    /// <summary>
    /// Update
    /// </summary>
    void FixedUpdate()
    {
        switch (currentmod) //블록을 놓는 모드
        {
            case 1:
                break;
            case 2:
                LegoMod2_fixedupdate();
                break;
            case 3:
                LegoMod3_fixedupdate();
                break;
        }

    }


    private void Update()
    {
        switch (currentmod) //블록을 놓는 모드
        {
            case 1:
                LegoMod1();
                break;
            case 2:
                LegoMod2_update();
                break;
        }
    }

    /// <summary>
    /// 블록 모드 1. 
    /// 카메라에 따라 가상의 블록을 놓고
    /// 블록 위에 놓이게 되면, 그 블록 가장자이에 가상의 블록이 위치하게 함.
    /// 이 때, 우측 버튼을 누르게 되면,
    /// 레이가 탐지하는 위치에 가장 가까운 (결합 가능한) 블록 피크들을 탐지함
    /// 탐지에 성공하면 블록 모드 2로 변환
    /// </summary>
    void LegoMod1()
    {
        blegorenderer.material = Resources.Load<Material>("checking"); //매터리얼 표현
        Vector3[] outline = outlineset.OutlineInfo();
        int signfinder = (Camera.main.transform.forward.x >= 0 ? 1 : 0) +
                         (Camera.main.transform.forward.y >= 0 ? 2 : 0) +
                         (Camera.main.transform.forward.z >= 0 ? 4 : 0);
        for (int i = 0; i < outline.Length; i++)
        {
            int dummyfinder = (outline[i].x >= 0 ? 1 : 0) +
                              (outline[i].y >= 0 ? 2 : 0) +
                              (outline[i].z >= 0 ? 4 : 0);
            if (signfinder == dummyfinder)
            {
                signfinder = i;
                break;
            }
        }
        float minlengthsqr = Vector3.SqrMagnitude(Camera.main.transform.forward * blockDis + outline[signfinder]);
        float savedlength = Mathf.Sqrt(minlengthsqr);
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitinfo;
        bool finding = Physics.Raycast(ray, out hitinfo, Mathf.Infinity, layermask) &&
                       Vector3.SqrMagnitude(hitinfo.point - Camera.main.transform.position) < minlengthsqr;
        if (finding)
        {
            Vector3 normalv = hitinfo.normal;
            blego.transform.position = hitinfo.point + new Vector3(outline[signfinder].x * -Mathf.Abs(normalv.x),
                                                                   outline[signfinder].y * -Mathf.Abs(normalv.y),
                                                                   outline[signfinder].z * -Mathf.Abs(normalv.z));
            //print(hitinfo.collider);

        }
        else
        {
            blego.transform.position = Camera.main.transform.position + Camera.main.transform.forward * blockDis;
        }
        if (Input.GetMouseButtonDown(1) && finding)
        {
            if (hitinfo.collider.gameObject.tag == "Terrain")
            {
                currentmod = 3;
            }
            else
            {
                LegoMod1_notterrain(hitinfo);
            }
        }
    }

    void LegoMod1_notterrain(RaycastHit hitinfo)
    {


        peakset = outlineset.inoutpeak;
        float minsqrlength = realdetectLim * realdetectLim;
        int peakchecker = peakset.Aggregate((x, y) => x.Key > y.Key ? x : y).Key + 1;
        peaktype = peakchecker;
        minnumber = new Dictionary<int, int>();
        foreach (KeyValuePair<int, List<Vector3>> items in peakset)
        {
            minnumber.Add(items.Key, items.Value.Count);
            for (int i = 0; i < items.Value.Count; i++)
            {
                float dummy = Vector3.SqrMagnitude(blego.transform.position + peakset[items.Key][i] - hitinfo.point);
                if (dummy < minsqrlength)
                {
                    peaktype = items.Key;
                    minnumber[items.Key] = i;
                    minsqrlength = dummy;
                }
            }
        }
        if (peaktype != peakchecker && minnumber[peaktype] != peakset[peaktype].Count)
        {


            if (hitinfo.collider.gameObject.layer == LayerMask.NameToLayer("LegoConnect"))
            {
                targetlego = hitinfo.collider.gameObject.transform.parent.GetComponent<BaseLegogenerator>();
                targetplace = hitinfo.collider.gameObject.transform.parent.transform;
            }
            else
            {
                targetlego = hitinfo.collider.gameObject.GetComponent<BaseLegogenerator>();
                targetplace = hitinfo.collider.gameObject.GetComponent<Transform>();
            }

            targetpeakset = targetlego.inoutpeak;
            minsqrlength = realdetectLim * realdetectLim;
            peakchecker = targetpeakset.Aggregate((x, y) => x.Key > y.Key ? x : y).Key + 1;
            targetpeaktype = peakchecker;
            minnumber2 = new Dictionary<int, int>();
            foreach (int peaknum in connectionlist[peaktype])
            {
                minnumber2[peaknum] = targetpeakset[peaknum].Count;
                for (int i = 0; i < targetpeakset[peaknum].Count; i++)
                {
                    float dummy = Vector3.SqrMagnitude(targetplace.position + targetpeakset[peaknum][i] - hitinfo.point);

                    if (dummy < minsqrlength)
                    {
                        targetpeaktype = peaknum;
                        minnumber2[peaknum] = i;
                        minsqrlength = dummy;
                    }
                }
            }
            if (targetpeaktype != peakchecker && minnumber2[targetpeaktype] != targetpeakset[targetpeaktype].Count)
            {
                currentmod = 2;
            }
        }
    }

    void LegoMod3_fixedupdate()
    {
        ChangeL.interactable = false;
        ChangeH.interactable = false;
        ChangeW.interactable = false;
        ChangeRx.interactable = false;
        ChangeRy.interactable = false;
        ChangeRz.interactable = false;
    }
    void LegoMod2_fixedupdate()
    {
        ChangeL.interactable = false;
        ChangeH.interactable = false;
        ChangeW.interactable = false;
        ChangeRx.interactable = false;
        ChangeRy.interactable = false;
        ChangeRz.interactable = false;

        Vector3 basenormalPlane = new Vector3();
        List<int> basepeakPlane = new List<int>();

        Vector3 targetnormalPlane = new Vector3();
        List<int> targetpeakPlane = new List<int>();
        int baser = 0;
        for (int i = 0; i < outlineset.peakplanelist.Count; i++)
        {

            if (outlineset.peakplanelist[i].possiblePeakTyle == peaktype &&
                outlineset.peakplanelist[i].peaklist.Contains(minnumber[peaktype]))
            {
                baser = i;
                basenormalPlane = outlineset.peakplanelist[i].normalPlane;
                basepeakPlane = outlineset.peakplanelist[i].peaklist;
                break;
            }
        }
        for (int i = 0; i < targetlego.peakplanelist.Count; i++)
        {

            if (targetlego.peakplanelist[i].possiblePeakTyle == targetpeaktype &&
                targetlego.peakplanelist[i].peaklist.Contains(minnumber2[targetpeaktype]))
            {
                targetnormalPlane = targetlego.peakplanelist[i].normalPlane ;
                targetpeakPlane = targetlego.peakplanelist[i].peaklist;
                break;
            }
        }
        Quaternion rotter = Quaternion.identity;
        if (-basenormalPlane != targetnormalPlane)
        {
            /*
            Vector3 eaxis = Vector3.Cross(-basenormalPlane, targetnormalPlane);
            float cosmu = Mathf.Cos(Mathf.Acos(Vector3.Dot(-basenormalPlane, targetnormalPlane))/2);
            float sinmu = Mathf.Sin(Mathf.Acos(Vector3.Dot(-basenormalPlane, targetnormalPlane)) / 2);
            rotter = new Quaternion(sinmu * eaxis.x, sinmu * eaxis.y, sinmu * eaxis.z, cosmu);
            */
            rotter = Quaternion.FromToRotation(-basenormalPlane, targetnormalPlane);

            blego.transform.rotation *= rotter;
            outlineset.inout();
            peakset = outlineset.inoutpeak;
            basenormalPlane = outlineset.peakplanelist[baser].normalPlane;
            basepeakPlane = outlineset.peakplanelist[baser].peaklist;

        }
        if (d == null)
        {
            d = Vector3.Dot(targetnormalPlane, targetplace.position + targetpeakset[targetpeaktype][minnumber2[targetpeaktype]]);
        }
        if (Vector3.Dot(targetnormalPlane, Camera.main.transform.forward) < 0)
        {
            float k = ((float)d - Vector3.Dot(Camera.main.transform.position, targetnormalPlane)) /
                                              Vector3.Dot(Camera.main.transform.forward, targetnormalPlane);
            Vector3 changepos = new Vector3(Camera.main.transform.position.x + Camera.main.transform.forward.x * k,
                                            Camera.main.transform.position.y + Camera.main.transform.forward.y * k,
                                            Camera.main.transform.position.z + Camera.main.transform.forward.z * k) -
                                targetnormalPlane * Vector3.Dot(peakset[peaktype][minnumber[peaktype]], targetnormalPlane);

            float finaldistance = Mathf.Infinity;
            int finaltargetconnectpeak = targetpeakPlane.Aggregate((x, y) => x > y ? x : y) + 1;
            int finalbaseconnectpeak = basepeakPlane.Aggregate((x, y) => x > y ? x : y) + 1;
            foreach (int i in targetpeakPlane)
            {
                float dummy = Vector3.SqrMagnitude(targetplace.position + targetpeakset[targetpeaktype][i] - changepos);
                if (dummy < finaldistance)
                {
                    finaldistance = dummy;
                    finaltargetconnectpeak = i;
                }
            }
            finaldistance = Mathf.Infinity;
            foreach (int i in basepeakPlane)
            {
                float dummy = Vector3.SqrMagnitude(changepos + peakset[peaktype][i] - targetpeakset[targetpeaktype][finaltargetconnectpeak] - targetplace.position);

                if (dummy < finaldistance)
                {
                    finaldistance = dummy;
                    finalbaseconnectpeak = i;

                }
            }
            blego.transform.position = targetplace.position + targetpeakset[targetpeaktype][finaltargetconnectpeak] -
                                       peakset[peaktype][finalbaseconnectpeak];
            if (!alreadyrealblego)
            {
                realblego = Instantiate(Resources.Load<GameObject>("nothing"), new Vector3(0, 0, 0), Quaternion.identity);
                realblego.name = "realistic_lego";
                realblego.transform.SetParent(blego.transform, false);
                realblego.layer = LayerMask.NameToLayer("Realistic");
                Rigidbody realrigid = realblego.AddComponent<Rigidbody>();
                realblego.AddComponent<EventSender>();
                realrigid.isKinematic = true;
                realrigid.useGravity = false;
                realrigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
                outlineset.MeshColliderGenerator(realblego);
                alreadyrealblego = true;

            }
            Collider[] allhitters = Physics.OverlapSphere(blego.transform.position, outlineset.maxreach, layermask);
            foreach (Collider eachhitters in allhitters)
            {
                if (eachhitters.gameObject.layer == LayerMask.NameToLayer("LegoConnect") && !newchecker.Contains(eachhitters.transform.parent.gameObject))
                {
                    newchecker.Add(eachhitters.transform.parent.gameObject);
                }
                else if(eachhitters.gameObject.layer == LayerMask.NameToLayer("LegoBase") && !newchecker.Contains(eachhitters.gameObject))
                {
                    newchecker.Add(eachhitters.transform.gameObject);
                }
            }
            foreach (GameObject older in oldchecker)
            {
                if (!newchecker.Contains(older))
                {
                    older.GetComponent<BaseLegogenerator>().MeshColliderDeleter();
                }
            }
            foreach (GameObject newer in newchecker)
            {
                if (!oldchecker.Contains(newer))
                {
                    GameObject dummy2 = Instantiate(Resources.Load<GameObject>("nothing"), new Vector3(0, 0, 0), Quaternion.identity);
                    dummy2.name = "realistic_lego";
                    dummy2.transform.SetParent(newer.transform, false);
                    dummy2.tag = "Real";
                    dummy2.layer = LayerMask.NameToLayer("Realistic");
                    newer.GetComponent<BaseLegogenerator>().MeshColliderGenerator(dummy2);
                }
            }
            oldchecker = newchecker;
            newchecker = new List<GameObject>();
            //outlineset.MeshColliderGenerator();
            //targetlego.MeshColliderGenerator();
        }


    }

    void LegoMod2_update()
    {
        if (iscollided)
        {
            blegorenderer.material = Resources.Load<Material>("noway");
        }
        else
        {
            blegorenderer.material = Resources.Load<Material>("yes");
        }
        if (Input.GetMouseButtonDown(0) && !iscollided)
        {
            GameObject finaladder = Instantiate(Resources.Load<GameObject>("nothing"), blego.transform.position, blego.transform.rotation);

            Component[] objs = blego.GetComponents<Component>();

            foreach (Component indobjs in objs)
            {
                if (indobjs is BaseLegogenerator)
                {
                    finaladder.AddComponent(indobjs.GetType());
                    break;
                }
            }
            BaseLegogenerator lastsetter = finaladder.GetComponent<BaseLegogenerator>();

            string cl = ChangeL.text;
            int il = (cl != "") ? Mathf.Max(int.Parse(cl), 2) : 2;
            string ch = ChangeH.text;
            int ih = (ch != "") ? Mathf.Max(int.Parse(ch), 1) : 1;
            string cw = ChangeW.text;
            int iw = (cw != "") ? Mathf.Max(int.Parse(cw), 2) : 2;
            lastsetter.ChangeShape(il, ih, iw);


            LegoMod2_additional();
            iscollided = false;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            LegoMod2_additional();
            iscollided = false;
        }
    }

    void LegoMod2_additional()
    {
        currentmod = 1;
        d = null;
        alreadyrealblego = false;
        Destroy(realblego);
        ChangeL.interactable = true;
        ChangeH.interactable = true;
        ChangeW.interactable = true;
        ChangeRx.interactable = true;
        ChangeRy.interactable = true;
        ChangeRz.interactable = true;
        Vector3 euler_rot = ToEulerAngles(blego.transform.rotation);
        ChangeRx.text = (euler_rot[0] * 180 / Mathf.PI).ToString();
        ChangeRy.text = (euler_rot[1] * 180 / Mathf.PI).ToString();
        ChangeRz.text = (euler_rot[2] * 180 / Mathf.PI).ToString();
        GameObject[] deleting = GameObject.FindGameObjectsWithTag("Real");
        foreach (GameObject saddelete in deleting)
        {
            Destroy(saddelete);
        }
        newchecker = new List<GameObject>();
        oldchecker = new List<GameObject>();
    }

    /// <summary>
    /// 블록 바꾸라는 명령이 들어오면 블록 바꾸기
    /// </summary>
    public void ChangeLHW()
    {
        string cl = ChangeL.text;
        int il = (cl != "") ? Mathf.Max(int.Parse(cl), 2) : 2;
        ChangeL.text = (cl != "") ? il.ToString() : "";
        string ch = ChangeH.text;
        int ih = (ch != "") ? Mathf.Max(int.Parse(ch), 1) : 1;
        ChangeH.text = (ch != "") ? ih.ToString() : "";
        string cw = ChangeW.text;
        int iw = (cw != "") ? Mathf.Max(int.Parse(cw), 2) : 2;
        ChangeW.text = (cw != "") ? iw.ToString() : "";
        blockDis = 5.0f + Mathf.Sqrt((il - 2) * (il - 2) + (ih - 2) * (ih - 2) + (iw - 2) * (iw - 2));
        outlineset.ChangeShape(il, ih, iw);
    }

    /// <summary>
    /// 회전
    /// </summary>
    public void ChangeR()
    {
        string cx = ChangeRx.text;
        float ix = (cx != "") ? (float.Parse(cx) % 360 > 180 ? float.Parse(cx) % 360 - 360 : float.Parse(cx) % 360) : 0;
        ChangeRx.text = (cx != "") ? ix.ToString() : "";
        string cy = ChangeRy.text;
        float iy = (cy != "") ? (float.Parse(cy) % 360 > 180 ? float.Parse(cy) % 360 - 360 : float.Parse(cy) % 360) : 0;
        ChangeRy.text = (cy != "") ? iy.ToString() : "";
        string cz = ChangeRz.text;
        float iz = (cz != "") ? (float.Parse(cz) % 360 > 180 ? float.Parse(cz) % 360 - 360 : float.Parse(cz) % 360) : 0;
        ChangeRz.text = (cz != "") ? iz.ToString() : "";

        blego.transform.rotation = Quaternion.Euler(ix,iy,iz);
        outlineset.inout();
        peakset = outlineset.inoutpeak;
    }
    Vector3 ToEulerAngles(Quaternion q)
    {
        Vector3 angles = new Vector3();

        // roll (x-axis rotation)
        float sinr_cosp = 2 * (q.w * q.x + q.y * q.z);
        float cosr_cosp = 1 - 2 * (q.x * q.x + q.y * q.y);
        angles[0] = Mathf.Atan2(sinr_cosp, cosr_cosp);

        // pitch (y-axis rotation)
        float sinp = 2 * (q.w * q.y - q.z * q.x);
        if (Mathf.Abs(sinp) >= 1)
            angles[2] = Mathf.Sign(sinp) * Mathf.PI / 2; // use 90 degrees if out of range
        else
            angles[2] = Mathf.Asin(sinp);

        // yaw (z-axis rotation)
        float siny_cosp = 2 * (q.w * q.z + q.x * q.y);
        float cosy_cosp = 1 - 2 * (q.y * q.y + q.z * q.z);
        angles[1] = Mathf.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }
}
