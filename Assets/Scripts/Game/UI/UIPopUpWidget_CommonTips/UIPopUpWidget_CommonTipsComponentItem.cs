using System;
using System.Collections;
using System.Collections.Generic;

using QTFramework;

using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWidget_CommonTipsComponentItemAwakeSystem : AAwake<UIPopUpWidget_CommonTipsComponentItem>
	{
		public override void Awake (UIPopUpWidget_CommonTipsComponentItem _self)
		{
			_self.Awake ( );
		}
	}

[ObjectEventSystem]
public class UIPopUpWidget_CommonTipsComponentItemFixedUpdateSystem : AFixedUpdate<UIPopUpWidget_CommonTipsComponentItem>
	{
		public override void FixedUpdate (UIPopUpWidget_CommonTipsComponentItem _self)
		{
			_self.FixedUpdate ( );
		}
	}

[UIEntityComponent (UI_PrefabPath.m_sUIPopUpWidget_CommonTipsItem)]
public class UIPopUpWidget_CommonTipsComponentItem : UIComponent
{
	public Text m_kTextContent;

	public float showTime
	{
		get;
		set;
	}
	public readonly float durationTime = 3;
	internal void Awake ( )
	{
        m_kTextContent = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Text;
    }

	internal void FixedUpdate ( )
	{}

	public void Init (string _content)
	{
		m_kTextContent.text = _content;
	}

}
