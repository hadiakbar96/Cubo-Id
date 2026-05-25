using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "playScreen";
    
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

}
