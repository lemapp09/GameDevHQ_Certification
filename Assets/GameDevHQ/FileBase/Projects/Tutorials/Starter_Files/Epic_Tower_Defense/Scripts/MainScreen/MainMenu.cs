using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MetroMayhem.MainScreen
{

    public class MainMenu : MonoBehaviour
    {

        [SerializeField] private Sprite[] _titleGraphics;
        [SerializeField] private Image _titleGraphic;
        [SerializeField] private TextMeshProUGUI _urbanText;

        private void OnEnable(){
            _urbanText.color = new Color(1f, 1f, 1f, 0f);
            _titleGraphic.sprite = _titleGraphics[UnityEngine.Random.Range(0, _titleGraphics.Length)];
            StartCoroutine(FadeInUrbanDefendersText());
        }

        public void OnPlayClicked() {
            SceneManager.LoadScene(1);
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