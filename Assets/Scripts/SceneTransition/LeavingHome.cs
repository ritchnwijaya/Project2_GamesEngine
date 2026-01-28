using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExitTrigger : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "farmCity";
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        SceneManager.LoadScene(targetSceneName);
    }
}
