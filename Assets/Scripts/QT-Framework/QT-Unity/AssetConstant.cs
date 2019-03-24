using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetConstant
{
    public enum AssetsType
    {
        None=-1,
        Audio = 0,  // 音效
        Texture,    //图片
        Font,       //字体
        Gfx,        //特效
        UI,         // UI
        Logic,      //逻辑对象
        Model,      //acotor模型资源

    }

    public static Dictionary<AssetsType, string> BundleNameByPrefabType = new Dictionary<AssetsType, string>
    {
        {AssetsType.Audio,"bundle_audio" },
        {AssetsType.Texture,"bundle_texture" },
        {AssetsType.Font,"bundle_font" },
        {AssetsType.Gfx,"bundle_gfx" },
        {AssetsType.UI,"bundle_ui" },
        {AssetsType.Logic,"bundle_logic" },
        {AssetsType.Model,"bundle_model" },   
    };
}
