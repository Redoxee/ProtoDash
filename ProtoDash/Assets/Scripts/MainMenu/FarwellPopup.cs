using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dasher
{
    public class FarwellPopup : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup m_canvasGroup = null;

        private void Awake()
        {
            LevelFlow levelFlow = MainProcess.Instance.levelFlow;
            int currentLevelIndex = levelFlow.GetLevelIndex(levelFlow.GetMostInterestingLevel().sceneName);
            int numberOfLevels = MainProcess.Instance.levelFlow.LevelList.Count;
            if (currentLevelIndex > numberOfLevels - 2)
            {
                this.SetDisplay(true);
            }
            else
            {
                this.SetDisplay(false);
            }
        }

        private void SetDisplay(bool display)
        {
            m_canvasGroup.alpha = display ? 1 : 0;
            m_canvasGroup.interactable = display;
            m_canvasGroup.blocksRaycasts = display;
        }

        public void OnCelestePressed()
        {
            Application.OpenURL("https://mattmakesgames.itch.io/celeste");
        }

        public void On8LinksPressed()
        {
            string url = "https://antonoti.itch.io/8-links";
#if UNITY_ANDROID
            url = "https://play.google.com/store/apps/details?id=com.antonmakesgames.tilelinks";
#elif UNITY_IOS
            url = "https://itunes.apple.com/fr/app/8-links/id1325831544?mt=8";
#endif

            Application.OpenURL(url);
        }

        public void OnClosePressed()
        {
            this.SetDisplay(false);
        }
    }
}