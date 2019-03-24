using Facebook.Unity;
using QTFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopUpWindow_FriendComponentAwakeSystem : AAwake<UIPopUpWindow_FriendComponent>
{
    public override void Awake(UIPopUpWindow_FriendComponent _self)
    {
        _self.Awake();
    }
}

[UIEntityComponent(UI_PrefabPath.m_sUIPopUpWindow_Friend)]
public class UIPopUpWindow_FriendComponent : UIComponent
{
    public Button m_ButtonClose;
    public Button m_ButtonBind;
    public Button m_ButtonVisit;

    public Text m_TextTitle;
    public Text m_TextGift;
    public Text m_TextBind;
    public Text m_TextVisit;

    internal void Awake()
    {
        m_ButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_ButtonBind = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Button;
        m_ButtonVisit = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Button;
        m_TextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Text;
        m_TextGift = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_TextBind = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_TextVisit = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;
        m_ButtonClose.onClick.AddListener(onClick_Close);
        m_ButtonBind.onClick.AddListener(onClick_Bind);
        m_ButtonVisit.onClick.AddListener(onClick_Visit);

        Init();

    }
    public override void TranslateUI()
    {
        base.TranslateUI();
        m_TextTitle.text = UI_Helper.GetTextByLanguageID(261);
        m_TextGift.text = UI_Helper.GetTextByLanguageID(260,"12");
        m_TextBind.text = UI_Helper.GetTextByLanguageID(263);
        m_TextVisit.text = UI_Helper.GetTextByLanguageID(262);
    }
    private void Init()
    {
        m_ButtonClose.gameObject.SetActive(!FB.IsLoggedIn);
        m_ButtonVisit.gameObject.SetActive(FB.IsLoggedIn);
    }
    private void onClick_Visit()
    {
        
    }



    private void onClick_Bind()
    {
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log($"FaceBook登陆成功{aToken.UserId}");
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("FaceBook User cancelled login");
        }
    }
    private void onClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopUpWindow_Friend);
    }



}
