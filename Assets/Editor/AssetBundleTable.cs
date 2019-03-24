using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using OfficeOpenXml;

public class AssetBundleTable : EditorWindow
{
    static AssetBundleTable myWindow;

    private  readonly string _assetBundleTableAsset = "Assets/Editor/AssetBundleTable.asset";
    private  readonly string _assetBundleTableExcel = "Assets/Editor/AssetBundleTable.xlsx";
    public  AssetBundleTableConfig m_kAssetBundleTableConfig;
    private AssetBundleTableConfig.AssetType _assetType;
    private string _path = "";
    private string _bundelName = "";
    private bool _isPr = false;
    private static bool _Have = true;

    private Vector2 scrollPosition;
    private static List<bool> m_kBoolShow = new List<bool>();
    [MenuItem("Tools/QT-Framework/AssetBundleTable2Excel")]
    static void Init()
    {
        myWindow = (AssetBundleTable)GetWindow(typeof(AssetBundleTable), false, "AssetBundleTable", true);//创建窗口
        myWindow.Show(true);//展示
        myWindow.maximized = false;

    }

    private void Awake()
    {
        m_kAssetBundleTableConfig = AssetDatabase.LoadAssetAtPath<AssetBundleTableConfig>(_assetBundleTableAsset);
        if (m_kAssetBundleTableConfig == null)
        {
            Debug.Log("创建新的配置文件了");
            _Have = false;
            m_kAssetBundleTableConfig = ScriptableObject.CreateInstance<AssetBundleTableConfig>() as AssetBundleTableConfig;
        }
        m_kBoolShow.Clear();
        foreach (var item in m_kAssetBundleTableConfig.m_kDicBundelTable)
        {
            m_kBoolShow.Add(false);
        }
    }


    void OnGUI()
    {
        if (m_kAssetBundleTableConfig == null)
            return;
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("文 件 夹 路 径:", GUILayout.Width(80));
        _path = EditorGUILayout.TextField(_path, GUILayout.Width(820));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("BundelName:", GUILayout.Width(80));
        _bundelName = EditorGUILayout.TextField(_bundelName, GUILayout.Width(820));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Bundel   前缀:", GUILayout.Width(80));
        _isPr = EditorGUILayout.Toggle(_isPr, GUILayout.Width(820));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("资  源   类  型:", GUILayout.Width(80));
        _assetType = (AssetBundleTableConfig.AssetType)EditorGUILayout.EnumPopup(_assetType, GUILayout.Width(300));
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if (GUILayout.Button("添加", GUILayout.Width(900)))
        {
            if (m_kAssetBundleTableConfig.CheckBundelTablePath(_path))
            {
                myWindow.ShowNotification(new GUIContent("文件路径已经存在"));
                return;
            }
            if (string.IsNullOrEmpty(_path) || string.IsNullOrEmpty(_bundelName))
            {
                myWindow.ShowNotification(new GUIContent("文件路径或Bundel名为空！"));
                return;
            }
            BundelTableItem bundelTableItem =new BundelTableItem();
            bundelTableItem.AssetPath = _path.Replace('\\','/');
            bundelTableItem.AssetType = _assetType;
            bundelTableItem.BundelName = _bundelName;
            bundelTableItem.IsFullBubdel = _isPr;

            m_kAssetBundleTableConfig.AddBundelTableItem(bundelTableItem);
            _path = null;
            _bundelName = null;
            _isPr = false;

            m_kBoolShow.Clear();
            foreach (var item in m_kAssetBundleTableConfig.m_kDicBundelTable)
            {         
                m_kBoolShow.Add(false);
            }

            Refreshdraw();
        }
        if (GUILayout.Button("刷新BundelConfig", GUILayout.Width(900)))
        {
            m_kBoolShow.Clear();
            foreach (var item in m_kAssetBundleTableConfig.m_kDicBundelTable)
            {
                m_kBoolShow.Add(false);
            }
            Refreshdraw();
            Config2Excel();
        }
        GUILayout.EndVertical();

        GUILayout.Space(20);
        drawBundelTable();


        GUILayout.Space(20);
        drawBundelTableConfig();
    }

    private void drawBundelTable()
    {
        EditorGUILayout.LabelField("BundelSetting");
        GUI.backgroundColor = Color.black;
        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(900));
        GUI.backgroundColor = Color.white;
        EditorGUILayout.LabelField("文件路径", GUILayout.Width(400));
        EditorGUILayout.LabelField("Bundle", GUILayout.Width(250));
        EditorGUILayout.LabelField("资源类型", GUILayout.Width(100));
        EditorGUILayout.LabelField("是否是前缀", GUILayout.Width(100));
        EditorGUILayout.LabelField("操作", GUILayout.Width(50));
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        foreach (var item in m_kAssetBundleTableConfig.m_kDicBundelTable)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(item.AssetPath, GUILayout.Width(400));
            EditorGUILayout.LabelField(item.BundelName, GUILayout.Width(250));
            EditorGUILayout.LabelField(item.AssetType.ToString(), GUILayout.Width(100));
            EditorGUILayout.LabelField(item.IsFullBubdel.ToString(), GUILayout.Width(100));
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                m_kAssetBundleTableConfig.m_kDicBundelTable.Remove(item);
                Refreshdraw();
                break;
            }
            GUILayout.EndHorizontal();
        }
    }

    private void Refreshdraw()
    {
        RefreshBundelTable();
    }

    private void drawBundelTableConfig()
    {
        EditorGUILayout.LabelField("BundelTableConfig");
        GUI.backgroundColor = Color.black;
        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(900));
        GUI.backgroundColor = Color.white;
        EditorGUILayout.LabelField("资源路径", GUILayout.Width(600));
        EditorGUILayout.LabelField("Bundle名", GUILayout.Width(300));
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(900), GUILayout.Height(500));
        int itemIndex = 0;
        foreach (var item in m_kAssetBundleTableConfig.m_kDicBundelTableConfig)
        {
            GUILayout.Space(5);
            GUI.backgroundColor = Color.gray;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            m_kBoolShow[itemIndex] = GUILayout.Toggle(m_kBoolShow[itemIndex], $"BundelName:[{item.BundelName}] Count:{item.BundelTableConfigItemList.Count}", "toggle", GUILayout.Width(900));
            GUILayout.EndHorizontal();

            if (m_kBoolShow[itemIndex])
            {
                GUILayout.BeginVertical();
                foreach (var config in item.BundelTableConfigItemList)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(config.AssetPath, GUILayout.Width(600));
                    EditorGUILayout.LabelField(config.BundelName, GUILayout.Width(300));
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUI.backgroundColor = Color.white;

            itemIndex++;
        }
        GUILayout.EndScrollView();
    }
    private void RefreshBundelTable()
    {
        m_kAssetBundleTableConfig.m_kDicBundelTableConfig.Clear();
        foreach (var item in m_kAssetBundleTableConfig.m_kDicBundelTable)
        {
            BundelTableConfig bundelTableConfig = new BundelTableConfig();
            bundelTableConfig.BundelName = item.BundelName;
            if (Directory.Exists(item.AssetPath))
            {
                DirectoryInfo direction = new DirectoryInfo(item.AssetPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                List<BundelTableConfigItem> bundelTableConfigList = new List<BundelTableConfigItem>();
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }
                    bool contine = false;
                    var extensionList = m_kAssetBundleTableConfig.m_kDicAssetType[item.AssetType];
                    foreach (var extension in extensionList)
                    {
                        if (files[i].Name.ToLower().EndsWith(extension.ToLower()))
                        {
                            contine = true;
                        }
                    }
                    if (!contine)
                    {
                        continue;
                    }

                    BundelTableConfigItem bundelTableConfigItem = new BundelTableConfigItem();
                    string _assetPath = files[i].FullName.Substring(files[i].FullName.IndexOf("Assets"));
                    bundelTableConfigItem.AssetPath = _assetPath.Replace('\\','/');
                    bundelTableConfigItem.AssetType = item.AssetType;
                    if (item.IsFullBubdel)
                    {
                        bundelTableConfigItem.BundelName = item.BundelName + Path.GetFileNameWithoutExtension(files[i].Name);
                    }
                    else
                    {
                        bundelTableConfigItem.BundelName = item.BundelName;
                    }
                    bundelTableConfigList.Add(bundelTableConfigItem);
                }
                bundelTableConfig.AddBundelTableConfigItem(bundelTableConfigList);

                m_kAssetBundleTableConfig.AddBundelTableConfig(bundelTableConfig);               
            }
        }



    }
    private void Config2Excel()
    {
        FileInfo newFile = new FileInfo(_assetBundleTableExcel);
        if (newFile.Exists)
        {
            newFile.Delete();
            newFile = new FileInfo(_assetBundleTableExcel);
        }
        //通过ExcelPackage打开文件
        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            //添加sheet
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
            //添加列名
            worksheet.Cells[1, 1].Value = "资源路径";
            worksheet.Cells[1, 2].Value = "BundelName";

            //添加行数据
            worksheet.Cells["A2"].Value = "Unity中资源的路径";
            worksheet.Cells["B2"].Value = "BundelName";
            //添加行数据
            worksheet.Cells["A3"].Value = "String";
            worksheet.Cells["B3"].Value = "String";
            //添加行数据
            worksheet.Cells["A4"].Value = "_AssetPath";
            worksheet.Cells["B4"].Value = "_BundelName";

            int i = 5;
            foreach (var item in m_kAssetBundleTableConfig.m_kDicBundelTableConfig)
            {
                foreach (var bundel in item.BundelTableConfigItemList)
                {
                    worksheet.Cells[$"A{i}"].Value = bundel.AssetPath;
                    worksheet.Cells[$"B{i}"].Value = bundel.BundelName;
                    i++;
                }

            }
            package.Save();
            Debug.Log("Create Success!");
        }

        if (!_Have)
        {
            _Have = true;
            AssetDatabase.CreateAsset(m_kAssetBundleTableConfig, _assetBundleTableAsset);
        }


        EditorUtility.SetDirty(m_kAssetBundleTableConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
