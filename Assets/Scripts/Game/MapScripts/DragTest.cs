using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTest : MonoBehaviour{


    Vector3 pos;
    bool flag = false;
    public Camera camera;

    private void Update()
    {
        if(Input.GetMouseButton(2)){
            var curPos = camera.ScreenToViewportPoint(Input.mousePosition);
            if(flag){
                Vector3 dst = pos - curPos;

                transform.position += new Vector3(dst.x * 20, 0, dst.y  * 20);
            }else{
                flag = true;
            }

            pos = curPos;
        }
        else{
            flag = false;
        }

        //Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (camera.fieldOfView <= 80)
                camera.fieldOfView += 2;
            if (camera.orthographicSize <= 20)
                camera.orthographicSize += 0.5F;
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (camera.fieldOfView > 10)
                camera.fieldOfView -= 2;
            if (camera.orthographicSize >= 1)
                camera.orthographicSize -= 0.5F;
        }
    }
}
