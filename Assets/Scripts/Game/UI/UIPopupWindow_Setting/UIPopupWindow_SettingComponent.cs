using System;
using QTFramework;
using UnityEngine;
using UnityEngine.UI;

[ObjectEventSystem]
public class UIPopupWindow_SettingComponentAwakeSystem : AAwake<UIPopupWindow_SettingComponent>
{
    public override void Awake(UIPopupWindow_SettingComponent _self)
    {
        _self.Awake();
    }
}


[UIEntityComponent(UI_PrefabPath.m_sUIPopupWindow_Setting)]
public class UIPopupWindow_SettingComponent : UIComponent
{

    public Button m_ButtonClose;
    public Text m_TextTitle;
    public Toggle m_ToggleSound;
    public Toggle m_ToggleMusic;
    public Text m_TextToggleSoundTitle;
    public Text m_TextToggleSoundTxt;
    public Text m_TextToggleMusicTitle;
    public Text m_TextToggleMusicTxt;
    public Button m_ButtonSelectLanguage;
    public Text m_SelectLanguageTitle;
    public Text m_SelectLanguageTxt;
    public Button m_ButtonFQA;
    public Text m_TextButtonFQATitle;
    public Button m_ButtonAbout;
    public Text m_TextButtonAboutTitle;
    public Toggle m_ToggleChinese;
    public Text m_kTextChineseTitle;
    public Toggle m_ToggleEnglish;
    public Text m_TextEnglishTitle;
    public Toggle m_ToggleArab;
    public Text m_kTextArabTitle;
    public Toggle m_ToggleRussia;
    public Text m_TextRussiaTitle;
    public Button m_ButtonConfirm;
    public Text m_ButtonConfirmTitle;

    public Text m_kTextChineseTitle2;
    public Text m_TextEnglishTitle2;
    public Text m_kTextArabTitle2;
    public Text m_TextRussiaTitle2;

    public Transform m_TransformNode1;
    public Transform m_TransformNode2;

    internal void Awake()
    {
        m_ButtonClose = m_kParentEntity.m_kUIPrefab.GetCacheComponent(0) as Button;
        m_TextTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(1) as Text;
        m_ToggleSound = m_kParentEntity.m_kUIPrefab.GetCacheComponent(2) as Toggle;
        m_ToggleMusic = m_kParentEntity.m_kUIPrefab.GetCacheComponent(3) as Toggle;
        m_TextToggleSoundTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(4) as Text;
        m_TextToggleSoundTxt = m_kParentEntity.m_kUIPrefab.GetCacheComponent(5) as Text;
        m_TextToggleMusicTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(6) as Text;
        m_TextToggleMusicTxt = m_kParentEntity.m_kUIPrefab.GetCacheComponent(7) as Text;
        m_ButtonSelectLanguage = m_kParentEntity.m_kUIPrefab.GetCacheComponent(8) as Button;
        m_SelectLanguageTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(9) as Text;
        m_SelectLanguageTxt = m_kParentEntity.m_kUIPrefab.GetCacheComponent(10) as Text;
        m_ButtonFQA = m_kParentEntity.m_kUIPrefab.GetCacheComponent(11) as Button;
        m_TextButtonFQATitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(12) as Text;
        m_ButtonAbout = m_kParentEntity.m_kUIPrefab.GetCacheComponent(13) as Button;
        m_TextButtonAboutTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(14) as Text;
        m_ToggleChinese = m_kParentEntity.m_kUIPrefab.GetCacheComponent(15) as Toggle;
        m_kTextChineseTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(16) as Text;
        m_ToggleEnglish = m_kParentEntity.m_kUIPrefab.GetCacheComponent(17) as Toggle;
        m_TextEnglishTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(18) as Text;
        m_ToggleArab = m_kParentEntity.m_kUIPrefab.GetCacheComponent(19) as Toggle;
        m_kTextArabTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(20) as Text;
        m_ToggleRussia = m_kParentEntity.m_kUIPrefab.GetCacheComponent(21) as Toggle;
        m_TextRussiaTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(22) as Text;
        m_ButtonConfirm = m_kParentEntity.m_kUIPrefab.GetCacheComponent(23) as Button;
        m_ButtonConfirmTitle = m_kParentEntity.m_kUIPrefab.GetCacheComponent(24) as Text;
        m_kTextChineseTitle2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(25) as Text;
        m_TextEnglishTitle2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(26) as Text;
        m_kTextArabTitle2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(27) as Text;
        m_TextRussiaTitle2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(28) as Text;

        m_TransformNode1 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(29) as Transform;
        m_TransformNode2 = m_kParentEntity.m_kUIPrefab.GetCacheComponent(30) as Transform;

        m_ButtonClose.onClick.AddListener(onButtonClick_Close);
        m_ToggleSound.onValueChanged.AddListener(onValueChanged_ToggleSound);
        m_ToggleMusic.onValueChanged.AddListener(onValueChanged_ToggleMusic);
        m_ButtonSelectLanguage.onClick.AddListener(onButtonClick_ButtonSelectLanguage);
        m_ButtonFQA.onClick.AddListener(onButtonClick_ButtonFQA);
        m_ButtonAbout.onClick.AddListener(onButtonClick_ButtonAbout);
        m_ToggleChinese.onValueChanged.AddListener(onValueChanged_ToggleChinese);
        m_ToggleEnglish.onValueChanged.AddListener(onValueChanged_ToggleEnglish);
        m_ToggleArab.onValueChanged.AddListener(onValueChanged_ToggleArab);
        m_ToggleRussia.onValueChanged.AddListener(onValueChanged_ToggleRussia);
        m_ButtonConfirm.onClick.AddListener(onButtonClick_ButtonConfirm);

        m_TransformNode1.gameObject.SetActive(true);
        m_TransformNode2.gameObject.SetActive(false);

        m_ToggleChinese.isOn =false;
        m_ToggleEnglish.isOn = false;
        m_ToggleArab.isOn = false;
        m_ToggleRussia.isOn = false;
        m_ToggleChinese.graphic.gameObject.SetActive(false);
        m_ToggleEnglish.graphic.gameObject.SetActive(false);
        m_ToggleArab.graphic.gameObject.SetActive(false);
        m_ToggleRussia.graphic.gameObject.SetActive(false);

        selesctLanguage();
    }
    
    private void selesctLanguage()
    {
        int languageID = 0;
        if (World.m_kLanguage == QTLanguage.Chinese)
        {
            languageID = 189;
        }
        else if (World.m_kLanguage == QTLanguage.Arabic)
        {
            languageID = 191;
        }
        else if (World.m_kLanguage == QTLanguage.English)
        {
            languageID = 190;
        }
        else if (World.m_kLanguage == QTLanguage.Russian)
        {
            languageID = 192;
        }
        m_SelectLanguageTxt.text = UI_Helper.GetTextByLanguageID(languageID);
    }

    private void onButtonClick_ButtonConfirm()
    {
        m_TransformNode1.gameObject.SetActive(true);
        m_TransformNode2.gameObject.SetActive(false);
    }

    private void onValueChanged_ToggleRussia(bool arg0)
    {
        if (arg0)
        {
            PlayerPrefs.SetInt("QTLanguage", (int)QTLanguage.Russian);
            World.SetLanguage(QTLanguage.Russian);
            selesctLanguage();
        }
        m_ToggleRussia.graphic.gameObject.SetActive(arg0);

    }

    private void onValueChanged_ToggleArab(bool arg0)
    {
        if (arg0)
        {
            PlayerPrefs.SetInt("QTLanguage", (int)QTLanguage.Arabic);
            World.SetLanguage(QTLanguage.Arabic);
            selesctLanguage();
        }
        m_ToggleArab.graphic.gameObject.SetActive(arg0);

    }

    private void onValueChanged_ToggleEnglish(bool arg0)
    {
        if (arg0)
        {
            PlayerPrefs.SetInt("QTLanguage", (int)QTLanguage.English);
            World.SetLanguage(QTLanguage.English);
            selesctLanguage();
        }
        m_ToggleEnglish.graphic.gameObject.SetActive(arg0);

    }

    private void onValueChanged_ToggleChinese(bool arg0)
    {
        if (arg0)
        {
            PlayerPrefs.SetInt("QTLanguage", (int)QTLanguage.Chinese);
            World.SetLanguage(QTLanguage.Chinese);
            selesctLanguage();
        }
        m_ToggleChinese.graphic.gameObject.SetActive(arg0);

    }

    private void onButtonClick_ButtonAbout()
    {
        
    }

    private void onButtonClick_ButtonFQA()
    {
       
    }

    private void onButtonClick_ButtonSelectLanguage()
    {
        m_TransformNode1.gameObject.SetActive(false);
        m_TransformNode2.gameObject.SetActive(true);

        m_ToggleChinese.isOn = World.m_kLanguage == QTLanguage.Chinese;
        m_ToggleEnglish.isOn = World.m_kLanguage == QTLanguage.English;
        m_ToggleArab.isOn = World.m_kLanguage == QTLanguage.Arabic;
        m_ToggleRussia.isOn = World.m_kLanguage == QTLanguage.Russian;
    }

    private void onValueChanged_ToggleMusic(bool arg0)
    {
        PlayerPrefs.SetFloat("ToggleMusic", arg0 ? 1 : 0);
        m_TextToggleMusicTxt.text = UI_Helper.GetTextByLanguageID(arg0 ? 198 : 199);
        World.Scene.GetComponent<AudioManagerComponent>().SetMusicVolume(arg0 ? 1 : 0);
    }

    private void onValueChanged_ToggleSound(bool arg0)
    {
        PlayerPrefs.SetFloat("ToggleSound", arg0 ? 1 : 0);
        m_TextToggleSoundTxt.text = UI_Helper.GetTextByLanguageID(arg0 ? 198 : 199);
        World.Scene.GetComponent<AudioManagerComponent>().SetVoiceVolume(arg0 ? 1 : 0);
    }

    private void onButtonClick_Close()
    {
        World.Scene.GetComponent<UIManagerComponent>().Remove(UI_PrefabPath.m_sUIPopupWindow_Setting);
    }

    public override void TranslateUI()
    {
        base.TranslateUI();

        m_TextTitle.text = UI_Helper.GetTextByLanguageID(188);
        m_TextToggleSoundTitle.text = UI_Helper.GetTextByLanguageID(193);
        m_TextToggleSoundTxt.text = UI_Helper.GetTextByLanguageID(198);
        m_TextToggleMusicTitle.text = UI_Helper.GetTextByLanguageID(194);
        m_TextToggleMusicTxt.text = UI_Helper.GetTextByLanguageID(198);
        m_SelectLanguageTitle.text = UI_Helper.GetTextByLanguageID(195);
        m_TextButtonFQATitle.text = UI_Helper.GetTextByLanguageID(196);
        m_TextButtonAboutTitle.text = UI_Helper.GetTextByLanguageID(197);

        m_kTextChineseTitle.text = UI_Helper.GetTextByLanguageID(189);
        m_TextEnglishTitle.text = UI_Helper.GetTextByLanguageID(190);
        m_kTextArabTitle.text = UI_Helper.GetTextByLanguageID(191);
        m_TextRussiaTitle.text = UI_Helper.GetTextByLanguageID(192);
        m_kTextChineseTitle2.text = UI_Helper.GetTextByLanguageID(189);
        m_TextEnglishTitle2.text = UI_Helper.GetTextByLanguageID(190);
        m_kTextArabTitle2.text = UI_Helper.GetTextByLanguageID(191);
        m_TextRussiaTitle2.text = UI_Helper.GetTextByLanguageID(192);

        m_ButtonConfirmTitle.text = UI_Helper.GetTextByLanguageID(141);
    }
    public override void Dispose()
    {
        base.Dispose();
        m_ButtonClose.onClick.AddListener(onButtonClick_Close);
        m_ToggleSound.onValueChanged.AddListener(onValueChanged_ToggleSound);
        m_ToggleMusic.onValueChanged.AddListener(onValueChanged_ToggleMusic);
        m_ButtonSelectLanguage.onClick.AddListener(onButtonClick_ButtonSelectLanguage);
        m_ButtonFQA.onClick.AddListener(onButtonClick_ButtonFQA);
        m_ButtonAbout.onClick.AddListener(onButtonClick_ButtonAbout);
        m_ToggleChinese.onValueChanged.AddListener(onValueChanged_ToggleChinese);
        m_ToggleEnglish.onValueChanged.AddListener(onValueChanged_ToggleEnglish);
        m_ToggleArab.onValueChanged.AddListener(onValueChanged_ToggleArab);
        m_ToggleRussia.onValueChanged.AddListener(onValueChanged_ToggleRussia);
        m_ButtonConfirm.onClick.AddListener(onButtonClick_ButtonConfirm);

    }
}
