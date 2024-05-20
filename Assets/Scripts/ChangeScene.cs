using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour{
    public void SwitchScene(string scenename){
        SceneManager.LoadScene(scenename);
    }
}
