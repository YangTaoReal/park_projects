/*************************************
 * 可视化UGUI的接受事件功能
 * 功能：以蓝紫色线条显示出UGUI带有碰撞功能
 * author:SmartCoder
 **************************************/

using UnityEngine;
using UnityEngine.UI;

public class DebugDrawLine : MonoBehaviour
{
    public bool isShowLine = true;
#if UNITY_EDITOR
    static Vector3[] fourCorners = new Vector3[4];
    void OnDrawGizmos()
    {
        if (isShowLine)
        {
            foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
            {
                if (g.raycastTarget)
                {
                    RectTransform rectTransform = g.transform as RectTransform;
                    rectTransform.GetWorldCorners(fourCorners);
                    Gizmos.color = Color.blue;
                    for (int i = 0; i < 4; i++)
                        Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);

                }
            }
        }
    }
#endif
}
