using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummytester : BaseLegogenerator
{
    // Start is called before the first frame update
    void Start()
    {
        MeshCollider wawa = gameObject.GetComponent<MeshCollider>();
        wawa.sharedMesh = FullCylinder(2f, 4f, new Vector3(3, 5, 3), 12);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay(Collider other)
    {

    }
}
