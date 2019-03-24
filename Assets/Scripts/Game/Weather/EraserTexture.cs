using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class EraserTexture : MonoBehaviour
{

    RawImage image;
    Camera cameraUI;
    int brushScale;
    Texture2D texRender;
    RectTransform mRectTransform;
    Texture2D texRenderUse;
    Dictionary<Rect, bool> childRectList = new Dictionary<Rect, bool>();
    float grad;
    Color[] srcColor;

    public void Init(Camera _cameraUI, int scale, float _grad)
    {

        cameraUI = _cameraUI;
        brushScale = scale;
        grad = _grad;

        image = GetComponent<RawImage>();
        mRectTransform = GetComponent<RectTransform>();
        if (image != null) texRenderUse = (Texture2D)image.texture;
        texRender = new Texture2D(texRenderUse.width, texRenderUse.height, TextureFormat.Alpha8, false);
        srcColor = texRenderUse.GetPixels();

        var size = brushScale * 2;
        var w = texRenderUse.width / size;
        var h = texRenderUse.height / size;
        int x = 0;
        int y = 0;
        var w1 = texRenderUse.width - w * size;
        var h1 = texRenderUse.height - h * size;

        for (x = 0; x < w; x++)
        {
            for (y = 0; y < h; y++)
            {
                childRectList.Add(new Rect(x * size, y * size, size, size), false);
            }

            if (h1 > brushScale)
            {
                childRectList.Add(new Rect(x * size, y * size, size, h1), false);
            }
        }
        if (w1 > brushScale && h1 > brushScale)
        {
            childRectList.Add(new Rect(x * size, y * size, w1, h1), false);
        }
    }


    public void OnPointerUp()
    {
        begin = Vector2.zero;
    }


    void OnEnable()
    {
        Reset();
    }

    Vector2 ConvertSceneToUI(Vector3 posi)
    {
        Vector2 postion;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(mRectTransform , posi, cameraUI, out postion)){
            return postion;
        }
        return Vector2.zero;
    }

    Vector2 begin;

    public float Progress
    {
        get{
            float val = 0;
            foreach (var item in childRectList)
            {
                if (item.Value)
                {
                    val++;
                }
            }
            return val / childRectList.Count;
        }
    }

    public void OnMouseMove(Vector2 position)
    {
        position = ConvertSceneToUI(position) + new Vector2(texRender.width / 2, texRender.height / 2);

        if (Vector2.Distance(position, begin) < 5){
            return;
        }

        //if(!begin.Equals(Vector2.zero)){
        //    var dst = Vector2.Distance(begin, position);
        //    if(dst > brushScale){
        //        int segments = (int)(2 * dst / brushScale);//计算出平滑的段数
        //        if(segments > 1){
        //            var temp = begin + position;
        //            for (int i = 1; i < segments; i++){
        //                var p = temp * i / segments;
        //                SetPixed(p);
        //            }
        //            SetPixed(position);
        //        }
        //        else{
        //            SetPixed(position);
        //        }
        //    }
        //    else{
        //        SetPixed(position);
        //    }
        //}
        //else{
            SetPixed(position);
        //}



        texRender.Apply();
        if (image != null) image.material.SetTexture("_RendTex", texRender);

        begin = position;

        //Debug.LogWarning(position);
        foreach (var item in childRectList)
        {
            if (item.Key.Contains(position))
            {
                childRectList[item.Key] = true;
                break;
            }
        }
    }

    void SetPixed(Vector2 position){
        for (int x = (int)position.x - brushScale; x < (int)position.x + brushScale; x++)
        {
            for (int y = (int)position.y - brushScale; y < (int)position.y + brushScale; y++)
            {
                var dst = Vector2.Distance(new Vector2(x, y), position);
                if (x < 0 || x > texRender.width || y < 0 || y > texRender.height || dst > brushScale)
                {
                    continue;
                }

                Color color = texRender.GetPixel(x, y);
                if(dst < brushScale-grad){
                    color.a = 0;
                }
                else{
                    var a = Mathf.Lerp(1, 0, (brushScale - dst) / grad);
                    if(a < color.a) color.a = a;
                }

                texRender.SetPixel(x, y, color);
            }
        }
    }

    void Reset()
    {

        texRender.SetPixels(srcColor);

        texRender.Apply();
        if(image != null) image.material.SetTexture("_RendTex", texRender);

        List<Rect> temp = new List<Rect>();
        foreach (var item in childRectList)
        {
            temp.Add(item.Key);
        }
        foreach (var item in temp)
        {
            childRectList[item] = false;
        }

        begin = Vector2.zero;
    }
}

