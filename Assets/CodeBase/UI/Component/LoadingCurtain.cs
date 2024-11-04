using TMPro;
using UnityEngine;

namespace CodeBase.UI.Component
{
    public class LoadingCurtain : MonoBehaviour
    {
        public CanvasGroup Curtain;
        public TextMeshProUGUI _loadingText;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Curtain.alpha = 1;
        }

        public void Hide()
        {
            Curtain.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}