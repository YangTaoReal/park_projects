/***********************************************
 * 资源加载器
 * author:SmartCoder
 **********************************************/

using QTFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class AssetCreater<T> : ICreater<T> where T:class
{
    private string m_sAssetName;
    private UnityEngine.Object m_kMaternalObject = null;

    public AssetCreater(string _assetName)
    {
        m_sAssetName = _assetName;
    }

    public T Create()
    {
        UnityEngine.Object kInstance = null;
        if (m_kMaternalObject == null)
        {
            kInstance = m_kMaternalObject = LoadAsset();
        }
        if (m_kMaternalObject == null)
        {
            Log.Error("AssetCreater", "无效资源" + m_sAssetName);
            return null;
        }
        kInstance = UnityEngine.Object.Instantiate(m_kMaternalObject);



        kInstance.name = m_sAssetName + "|";

        if (kInstance as GameObject)
        {
            GameObject _go = kInstance as GameObject;
            _go.transform.SetParent(AssetPoolManager.Instance.transform);
            _go.transform.position = new Vector3(0, 0, 0);
            _go.SetActive(false);
        }

        return kInstance as T;

    }

    private UnityEngine.Object LoadAsset()
    {
#if UNITY_EDITOR
     return editorOrStandaloneLoadasset();
#else
      return  mobilePlatformLoadAsset();
#endif
    }

    private UnityEngine.Object editorOrStandaloneLoadasset()
    {
        UnityEngine.Object kPrefab = null;
#if UNITY_EDITOR
        kPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(m_sAssetName);
#endif
        return kPrefab;
    }

    static Dictionary<string, AssetBundle> m_kAssetBundle = new Dictionary<string, AssetBundle>();
    private static AssetBundleManifest AssetBundleManifest;
    private UnityEngine.Object mobilePlatformLoadAsset()
    {
        CS_AssetBundleTable.DataEntry DataEntry = DBManager.Instance.m_kAssetBundleTable.GetEntryPtr(m_sAssetName.Replace("\\","/"));
        if (DataEntry == null)
        {
            Debug.LogError("@@@@@@@@@@@@@@"+ m_sAssetName);
            return null;
        }
        string bundleName = DBManager.Instance.m_kAssetBundleTable.GetEntryPtr(m_sAssetName.Replace("\\", "/"))._BundelName;
       // return AssetBundles.AssetBundleManager.LoadAsset(bundleName, m_sAssetName);

        if (AssetBundleManifest == null)
        {
            Debug.Log("资源路径" + Application.streamingAssetsPath + $"/{AssetBundles.Utility.GetPlatformName()}/{AssetBundles.Utility.GetPlatformName()}");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + $"/{AssetBundles.Utility.GetPlatformName()}/{AssetBundles.Utility.GetPlatformName()}");

            AssetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        UnityEngine.Object ob = LoadAssetBundle(bundleName).LoadAsset(m_sAssetName);
        Debug.Log("加载" + m_sAssetName);
        return ob;
    }

    private AssetBundle LoadAssetBundle(string bundleName)
    {
        Debug.Log("@@@@加载" + bundleName);
        AssetBundle ab = null;
        if (!m_kAssetBundle.TryGetValue(bundleName, out ab))
        {
            ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + $"/{AssetBundles.Utility.GetPlatformName()}/" + bundleName);
            m_kAssetBundle[bundleName] = ab;
            LoadDependencies(bundleName);
        }

        return ab;
    }

     protected void LoadDependencies(string assetBundleName)
    {

        if (AssetBundleManifest == null)
        {
            return;
        }

        // Get dependecies from the AssetBundleManifest object..
        string[] dependencies = AssetBundleManifest.GetAllDependencies(assetBundleName);
        if (dependencies.Length == 0)
            return;

        // Record and load all dependencies.
        for (int i = 0; i < dependencies.Length; i++)
        {
            Debug.Log("@@@@加载因爱" + dependencies[i]);
            LoadAssetBundle(dependencies[i]);
        }
    
    }
}
