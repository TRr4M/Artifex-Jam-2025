using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePicker : MonoBehaviour
{
    public void Play() {
        SceneManager.LoadScene(1);
    }
}
