using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSender : MonoBehaviour
{
    LegoPlace dummy;
    // Start is called before the first frame update
    void Awake()
    {
        dummy = GameObject.FindWithTag("Player").GetComponent<LegoPlace>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        dummy.iscollided = true;
    }

    public void OnTriggerStay(Collider other)
    {
        dummy.iscollided = true;
    }

    private void OnTriggerExit(Collider other)
    {
        dummy.iscollided = false;
    }
}
