
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public delegate void VoidDelegate(int x, int y);
    public VoidDelegate FindPath;
    public Action<int, int> wall;
    public int X;
    public int Y;

    private void OnMouseDown()
    {
        if (FindPath != null)
        {
            FindPath((int)this.transform.localPosition.x, (int)this.transform.localPosition.y);
        }
    }

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        wall(X, Y);
        Debug.Log(Time.time + ":进入该触发器的对象是：" + other.gameObject.name);

    }
}