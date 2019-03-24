using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BundelTableItem
{
    public AssetBundleTableConfig.AssetType AssetType;
    public string AssetPath;
    public string BundelName;
    public bool IsFullBubdel;
}
[System.Serializable]
public class BundelTableConfig
{
    public string BundelName;
    public List<BundelTableConfigItem> BundelTableConfigItemList = new List<BundelTableConfigItem>();

    public void AddBundelTableConfigItem(List<BundelTableConfigItem> _BundelTableConfigItemList)
    {
        BundelTableConfigItemList.AddRange(_BundelTableConfigItemList);
    }

}

[System.Serializable]
public class BundelTableConfigItem
{
    public AssetBundleTableConfig.AssetType AssetType;
    public string AssetPath;
    public string BundelName;
}

public class AssetBundleTableConfig : ScriptableObject
{
    public enum AssetType
    {
        Any,
        Audio,
        Texture,
        Model,
        Shader,
        Font,
        Video,
        TextOrBinary,
        Scene,
        Prefab,
        Animation,
        AnimatorController,
        AnimatorOverrideController,
        AvatarMask,
        CustomFont,
        PhysicMaterial,
        PhysicMaterial2D,
        Material,
        RenderTexture,
        CustomRenderTexture,
        LightmaParameter,
        LensFlare,
        TimeLine,
        SpriteAtlas,
        Tilemap,
        GUISkin
    }
    public Dictionary<AssetType, List<string>> m_kDicAssetType = new Dictionary<AssetType, List<string>>()
    {
        {AssetType.Any,new List<string>(){".*" } },
        {AssetType.Audio,new List<string>(){ ".aiff" , ".wav", ".mp3", ".ogg" } },
        {AssetType.Texture,new List<string>(){ ".bmp", ".jpg", ".png" } },
        {AssetType.Model,new List<string>(){ ".aiff" } },
        {AssetType.Shader,new List<string>(){ ".shader" } },
        {AssetType.Font,new List<string>(){ ".ttf" } },
        {AssetType.Video,new List<string>(){ ".mp4", ".avi", ".mpeg" } },
        {AssetType.TextOrBinary,new List<string>(){ ".bytes", ".txt", ".xml" } },
        {AssetType.Scene,new List<string>(){ ".unity" } },
        {AssetType.Prefab,new List<string>(){ ".prefab" } },
        {AssetType.Animation,new List<string>(){ ".anim" } },
        {AssetType.AnimatorController,new List<string>(){ ".controller" } },
        {AssetType.AnimatorOverrideController,new List<string>(){ ".overrideController" } },
        {AssetType.AvatarMask,new List<string>(){ ".mask" } },
        {AssetType.CustomFont,new List<string>(){ ".fontsettings" } },
        {AssetType.PhysicMaterial,new List<string>(){ ".physicMaterial" } },
        {AssetType.PhysicMaterial2D,new List<string>(){ ".physicsMaterial2D" } },
        {AssetType.Material,new List<string>(){ ".mat" } },
        {AssetType.RenderTexture,new List<string>(){ ".renderTexture" } },
        {AssetType.CustomRenderTexture,new List<string>(){ ".asset" } },
        {AssetType.LightmaParameter,new List<string>(){ ".giparams" } },
        {AssetType.LensFlare,new List<string>(){ ".flare" } },
        {AssetType.TimeLine,new List<string>(){ ".playable" } },
        {AssetType.SpriteAtlas,new List<string>(){ ".spriteatlas" } },
        {AssetType.Tilemap,new List<string>(){ ".asset" } },
        {AssetType.GUISkin,new List<string>(){ ".guiskin" } },
    };

    public List< BundelTableItem> m_kDicBundelTable = new List< BundelTableItem>();
    
    public List<BundelTableConfig> m_kDicBundelTableConfig =new List<BundelTableConfig>();


    public void AddBundelTableItem(BundelTableItem _BundelTableItem)
    {
        bool _have = false;
        foreach(var item in m_kDicBundelTable)
        {
            if (item.AssetPath == _BundelTableItem.AssetPath)
            {
                _have = true;
                break;
            }
        }

        if (!_have)
        {
            m_kDicBundelTable.Add(_BundelTableItem);
        }
    }
    public bool CheckBundelTablePath(string _asset)
    {
        foreach (var item in m_kDicBundelTable)
        {
            if (item.AssetPath == _asset)
            {
                return true;
            }
        }

        return false;
    }


    public void AddBundelTableConfigItem(BundelTableConfigItem _BundelTableConfigItem)
    {
        bool _have = false;
        foreach (var item in m_kDicBundelTableConfig)
        {
            if (item.BundelName == _BundelTableConfigItem.BundelName)
            {
                item.BundelTableConfigItemList.Add(_BundelTableConfigItem);
                return;
            }
        }

        BundelTableConfig bundelTableConfig = new BundelTableConfig();
        bundelTableConfig.BundelName = _BundelTableConfigItem.BundelName;
        bundelTableConfig.BundelTableConfigItemList.Add(_BundelTableConfigItem);
        if (!_have)
        {
            m_kDicBundelTableConfig.Add(bundelTableConfig);
        }
    }

    public void AddBundelTableConfig(BundelTableConfig _BundelTableConfig)
    {
        foreach (var item in m_kDicBundelTableConfig)
        {
            if (item.BundelName == _BundelTableConfig.BundelName)
            {
                item.BundelTableConfigItemList.AddRange(_BundelTableConfig.BundelTableConfigItemList);
                return;
            }
        }

        m_kDicBundelTableConfig.Add(_BundelTableConfig);
    }
}
