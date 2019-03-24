/***********************************************************
 * 游戏世界管理
 * 负责游戏对象在场景中的添加删除
 * author:SmartCoder
 * *********************************************************/

using System.Collections.Generic;
using System.Linq;

using QTFramework;

using UnityEngine;
using UnityEngine.EventSystems;

[ObjectEventSystem]
public class PlayerManagerComponentAwakeSystem : AAwake<PlayerManagerComponent>
{
    public override void Awake(PlayerManagerComponent _self)
    {
        _self.Awake();
    }
}

public class PlayerManagerComponent : QTComponent
{
    public Player GamePlayer = null;
    public Player NetPlayer = null;
    public void Awake()
    {
        Log.Info("PlayerManagerComponent", "角色管理器器");
        GamePlayer = QTComponentFactory.Instance.Create<Player>();

        ////使用本地数据
        //var dicPlayerBasicAsset = DataManager._instance.GetAllData<PlayerBasicAsset>();
        //PlayerBasicAsset PBAsset = null;
        //foreach (var baseAsset in dicPlayerBasicAsset)
        //{
        //    if (PBAsset != null)
        //        break;
        //    PBAsset = baseAsset.Value;
        //}

        GamePlayer.Init();

        List<CS_Shop.DataEntry> _shopList = DBManager.Instance.m_kShop.m_kDataEntryTable.Values.ToList();
        for (int i = 0; i < _shopList.Count; i++)
        {
            GamePlayer.m_kPlayerShopAsset.m_kShop.Add(_shopList[i]._ID, false);
        }

        var goods = DBManager.Instance.m_kShop.m_kDataEntryTable.GetEnumerator();
        while (goods.MoveNext())
        {
            GamePlayer.UnlockGoods(goods.Current.Value._ID);
        }
    }
}