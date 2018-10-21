using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
namespace StrangeEngine
{
    public class ScenesLoader : MonoBehaviour
    {
        [Tooltip("Canvas to Enable on scene load")]
        public GameObject canvas;
        [Tooltip("Proggress bar on scene load")]
        public Slider progressbar;
        private AsyncOperation ao;

        public void LoadScene(int SceneNumber)
        {
            PlayerPrefs.Save();
            StartCoroutine(load(SceneNumber));
        }
        IEnumerator load(int scene)
        {
            ao = SceneManager.LoadSceneAsync(scene);
            ao.allowSceneActivation = false;
            while (!ao.isDone)
            {
                canvas.SetActive(true);
                progressbar.value = ao.progress;
                
                if(ao.progress == 0.9f)
                {
                    ao.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}

