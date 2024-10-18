using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIMainMenu : UIPage
    {
        public readonly float STORE_AD_RIGHT_OFFSET_X = 300F;

        [SerializeField] RectTransform safeAreaRectTransform;

        [Space]
        [SerializeField] RectTransform tapToPlayRect;
        [SerializeField] Button playButton;
        [SerializeField] TMP_Text playButtonText;

        [Space]
        [SerializeField] UIScaleAnimation coinsLabelScalable;
        [SerializeField] UIScaleAnimation livesIndicatorScalable;

        [Space]
        [SerializeField] UIMainMenuButton iapStoreButton;
        [SerializeField] UIMainMenuButton noAdsButton;

        private TweenCase tapToPlayPingPong;
        private TweenCase showHideStoreAdButtonDelayTweenCase;

        public override void Initialise()
        {
            iapStoreButton.Init(STORE_AD_RIGHT_OFFSET_X);
            noAdsButton.Init(STORE_AD_RIGHT_OFFSET_X);

            playButton.onClick.AddListener(PlayButton);

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            showHideStoreAdButtonDelayTweenCase?.Kill();

            HideAdButton(true);
            iapStoreButton.Hide(true);
            ShowTapToPlay();

            coinsLabelScalable.Show();
            livesIndicatorScalable.Show();

            UILevelNumberText.Show();
            playButtonText.text = "LEVEL " + (LevelController.MaxReachedLevelIndex + 1);

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.12f, delegate
            {
                ShowAdButton();
                iapStoreButton.Show();
            });

            SettingsPanel.ShowPanel();

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            if (!isPageDisplayed)
                return;

            showHideStoreAdButtonDelayTweenCase?.Kill();

            isPageDisplayed = false;

            HideTapToPlayButton();

            coinsLabelScalable.Hide();
            livesIndicatorScalable.Hide();

            HideAdButton();

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.1f, delegate { iapStoreButton.Hide(); });

            SettingsPanel.HidePanel();

            Tween.DelayedCall(0.5f, delegate { UIController.OnPageClosed(this); });
        }

        #endregion

        #region Tap To Play Label

        public void ShowTapToPlay(bool immediately = false)
        {
            if (tapToPlayPingPong != null && tapToPlayPingPong.IsActive)
                tapToPlayPingPong.Kill();

            if (immediately)
            {
                tapToPlayRect.localScale = Vector3.one;

                tapToPlayPingPong = tapToPlayRect.transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

                return;
            }

            // RESET
            tapToPlayRect.localScale = Vector3.zero;

            tapToPlayRect.DOPushScale(Vector3.one * 1.2f, Vector3.one, 0.35f, 0.2f, Ease.Type.CubicOut, Ease.Type.CubicIn).OnComplete(delegate { tapToPlayPingPong = tapToPlayRect.transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true); });
        }

        public void HideTapToPlayButton(bool immediately = false)
        {
            if (tapToPlayPingPong != null && tapToPlayPingPong.IsActive)
                tapToPlayPingPong.Kill();

            if (immediately)
            {
                tapToPlayRect.localScale = Vector3.zero;

                return;
            }

            tapToPlayRect.DOScale(Vector3.zero, 0.3f).SetEasing(Ease.Type.CubicIn);
        }

        #endregion

        #region Ad Button Label

        private void ShowAdButton(bool immediately = false)
        {
            noAdsButton.Hide(immediately: true);
        }

        private void HideAdButton(bool immediately = false)
        {
            noAdsButton.Hide(immediately);
        }

        #endregion

        #region Buttons

        private void PlayButton()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            OnPlayTriggered(LevelController.MaxReachedLevelIndex);
        }

        private void OnLevelOnMapSelected(int levelId)
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);

            OnPlayTriggered(levelId);
        }

        private void OnPlayTriggered(int levelId)
        {
            GameController.LoadLevel(levelId);
        }



        #endregion

    }

}