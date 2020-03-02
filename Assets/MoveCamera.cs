using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public Transform camera;
    private bool top;
    // Start is called before the first frame update
    void Start()
    {
        top = true;
        camera.position = new Vector3(30.04f, 64.5f, -25.7f);
        camera.localRotation = Quaternion.Euler(83.1f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (top)
            {
                camera.position = new Vector3(-3.69f, 7.9f, -66.7f);
                camera.localRotation = Quaternion.Euler(9.5f, 18.8f, 0);
            }
            else 
            {
                camera.position = new Vector3(30.04f, 64.5f, -25.7f);
                camera.localRotation = Quaternion.Euler(83.1f, 0, 0);
            }
            top = !top;
        }
    }
}
