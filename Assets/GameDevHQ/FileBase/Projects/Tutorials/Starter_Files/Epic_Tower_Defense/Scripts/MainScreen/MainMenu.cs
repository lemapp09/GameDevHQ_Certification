using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MetroMayhem.MainScreen
{

    public class MainMenu : MonoBehaviour
    {
        [Header("Title Graphics")]
        [SerializeField] private Sprite[] _titleGraphics;
        [SerializeField] private Image _titleGraphic;
        [SerializeField] private TextMeshProUGUI _urbanText;
        
        [Header("Loading Next Scene Panel")]
        [SerializeField] private GameObject _loadingNextScenePanel;
        [SerializeField] private  Slider progressBar;
        
        private void OnEnable(){
            _urbanText.color = new Color(1f, 1f, 1f, 0f);
            _titleGraphic.sprite = _titleGraphics[UnityEngine.Random.Range(0, _titleGraphics.Length)];
            StartCoroutine(FadeInUrbanDefendersText());
        }

        public void OnPlayClicked() {
            _loadingNextScenePanel.SetActive(true);
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(1);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                progressBar.value = progress;
                yield return null;
            }
        }

        
        private IEnumerator FadeInUrbanDefendersText() {
            float i = 0f;
            while (i < 1) {
                    i += Time.deltaTime * 0.333f;
                _urbanText.color = new Color(1f, 1f, 1f, i);
                yield return null;
            }
        }

    }
}