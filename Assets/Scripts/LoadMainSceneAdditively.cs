namespace NetworkTest
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public sealed class LoadMainSceneAdditively : MonoBehaviour
    {
        public string MainSceneName = "TestScene";

        private void OnEnable()
        {
            if (!SceneManager.GetSceneByName(MainSceneName).isLoaded)
            {
                SceneManager.LoadScene(MainSceneName, LoadSceneMode.Additive);
            }
        }
    }
}
