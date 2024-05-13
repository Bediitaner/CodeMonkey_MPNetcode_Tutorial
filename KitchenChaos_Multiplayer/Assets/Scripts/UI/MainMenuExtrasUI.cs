using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuExtrasUI : MonoBehaviour
    {
        #region Contents

        [SerializeField] private Button youTubeButton;

        #endregion
        
        #region Unity: Awake

        private void Awake()
        {
            youTubeButton.onClick.AddListener(() => { Application.OpenURL("https://www.youtube.com/watch?v=AmGSEH7QcDg"); });
        }

        #endregion
    }
}