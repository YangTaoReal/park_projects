using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LogoScene : MonoBehaviour {

    public int firstGameSceneIdx = 1;
    public float changeSceneDelayAfterAnim = 2.0f;

    public void OnAnimComplete()
    {
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(changeSceneDelayAfterAnim);

        SceneManager.LoadScene(firstGameSceneIdx);
    }
}
