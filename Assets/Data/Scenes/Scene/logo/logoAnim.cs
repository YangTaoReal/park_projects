using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logoAnim : MonoBehaviour {
    public LogoScene logoScene;


    public void OnAnimComplete(){
        logoScene.OnAnimComplete();
    }
}
