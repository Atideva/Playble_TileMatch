using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGame : UIPage
    {
        [SerializeField] RectTransform safeAreaRectTransform;
        [SerializeField] UILevelNumberText levelNumberText;

        [SerializeField] Button exitButton;
        [SerializeField] UIFadeAnimation exitButtonFadeAnimation;

        [SerializeField] GameObject devOverlay;

        [LineSpacer("Tutorial")]
        [SerializeField] GameObject tutorialPanelObject;
        [SerializeField] TextMeshProUGUI tutorialTitleText;
        [SerializeField] TextMeshProUGUI tutorialDescriptionText;
        [SerializeField] Button tutorialSkipButton;

        public override void Initialise()
        {
            exitButton.onClick.AddListener(ShowExitPopUp);
            exitButtonFadeAnimation.Hide(immediately: true);

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
            NotchSaveArea.RegisterRectTransform((RectTransform) tutorialPanelObject.transform);

            DevPanelEnabler.RegisterPanel(devOverlay);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            exitButtonFadeAnimation.Show();

            UILevelNumberText.Show();

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            exitButtonFadeAnimation.Hide();

            UILevelNumberText.Hide();

            UIController.OnPageClosed(this);
        }

        public void UpdateLevelNumber(int levelNumber)
        {
            levelNumberText.UpdateLevelNumber(levelNumber);
        }

        #endregion

        public void ShowExitPopUp()
        {
            AudioController.PlaySound(AudioController.Sounds.buttonSound);
        }

        public void ExitPopUpConfirmExitButton()
        {
            UIController.HidePage<UIGame>();

            GameController.ReturnToMenu();
        }

    }
}