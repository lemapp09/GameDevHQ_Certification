using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace MetroMayhem.MainScreen
{

    public class MainMenu : MonoBehaviour
    {
        #region Variables
        [Header("Title Graphics")]
        [SerializeField] private Sprite[] _titleGraphics;
        [SerializeField] private Image _titleGraphic;
        [SerializeField] private TextMeshProUGUI _urbanText;
        
        [Header("Loading Next Scene Panel")]
        [SerializeField] private GameObject _loadingNextScenePanel;
        [SerializeField] private  Slider _progressBar;
        [SerializeField] private bool _gameWonScene;
        #endregion
        
        private void OnEnable(){
            if (_titleGraphics == null) {
                Debug.Log("No title graphic files assigned to Main Screen.");
            }
            if ( _titleGraphic == null) {
                Debug.Log("No UI Image container assigned to Main Screen");
            }
            if ( _urbanText  == null) {
                Debug.Log("No UI Text container assigned to Main Screen.");
            }
            if ( _loadingNextScenePanel == null ) {
                Debug.Log("No Loading Next Screen assigned to Main Screen.");
            }
            if ( _progressBar == null ) {
                Debug.Log("No Progress Bar assigned to Main Screen.");
            }

            _urbanText.color = new Color(1f, 1f, 1f, 0f);
            _titleGraphic.sprite = _titleGraphics[UnityEngine.Random.Range(0, _titleGraphics.Length)];
            StartCoroutine(FadeInUrbanDefendersText());
            if (_gameWonScene) {
                StartCoroutine(QuitGame10Seconds());
            }
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
                _progressBar.value = progress;
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

        private IEnumerator QuitGame10Seconds()
        {
            yield return new WaitForSeconds(10);
     
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            // end game while in iOS
#if UNITY_IOS
            Application.Quit;
#endif
            // end game while in Android
#if UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("finish");
#endif
            // end game while in Windows
#if UNITY_STANDALONE_WIN
            Application.Quit();
#endif
            // end game while in Mac OS
#if UNITY_STANDALONE_OSX
            Application.Quit();
#endif
            // end game while in Linux
#if UNITY_STANDALONE_LINUX
            Application.Quit();
#endif
            // end game while in Webgl
#if UNITY_WEBGL
            Application.Quit();
#endif

            // Application.Quit();
        }

    }
}