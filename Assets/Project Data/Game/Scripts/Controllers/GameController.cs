using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Watermelon
{
    public class GameController : MonoBehaviour
    {
        private static GameController gameController;

        [DrawReference]
        [SerializeField] GameData data;

        [LineSpacer]
        [SerializeField] UIController uiController;

        private LevelController levelController;
        private ParticlesController particlesController;
        private FloatingTextController floatingTextController;

        public static GameData Data => gameController.data;

        private static bool isGameActive;
        public static bool IsGameActive => isGameActive;

        private void Awake()
        {
            gameController = this;

            SaveController.Initialise(useAutoSave: false);

            // Cache components
            CacheComponent(out particlesController);
            CacheComponent(out floatingTextController);
            CacheComponent(out levelController);
        }

        private void Start()
        {
            InitialiseGame();
        }

        public void InitialiseGame()
        {
            uiController.Initialise();
            particlesController.Initialise();
            floatingTextController.Inititalise();
            levelController.Initialise();
            uiController.InitialisePages();

#if UNITY_EDITOR
            CheckIfNeedToAutoRunLevel();
#endif

            GameLoading.MarkAsReadyToHide();
        }

        public static void LoadLevel(int index, SimpleCallback onLevelLoaded = null)
        {
            UIController.ShowPage<UIGame>();
            gameController.levelController.LoadLevel(index, onLevelLoaded);
            isGameActive = true;
        }

        public static void OnLevelCompleted()
        {
            if (!isGameActive)
                return;

            SaveController.Save();

            UIController.HidePage<UIGame>(UIController.ShowPage<UIComplete>);

            isGameActive = false;
        }

        public static void OnLevelFailed()
        {
            if (!isGameActive)
                return;

            UIController.HidePage<UIGame>(() => { UIController.ShowPage<UIGameOver>(); });

            isGameActive = false;
        }

        public static void LoadNextLevel(SimpleCallback onLevelLoaded = null)
        {
            LoadLevel(LevelController.DisplayedLevelIndex, onLevelLoaded);
        }

        public static void ReplayLevel()
        {
            isGameActive = false;

            LoadLevel(LevelController.DisplayedLevelIndex);
        }

        public static void ReturnToMenu()
        {
            isGameActive = false;

            LevelController.UnloadLevel();
        }

        public static void Revive()
        {
            isGameActive = true;

            LevelController.ReturnTiles(3, null);
        }

        #region Extensions

        public bool CacheComponent<T>(out T component) where T : Component
        {
            Component unboxedComponent = gameObject.GetComponent(typeof(T));

            if (unboxedComponent != null)
            {
                component = (T) unboxedComponent;

                return true;
            }

            Debug.LogError(string.Format("Scripts Holder doesn't have {0} script added to it", typeof(T)));

            component = null;

            return false;
        }

        #endregion

        #region Dev

#if UNITY_EDITOR

        private static readonly string AUTO_RUN_LEVEL_SAVE_NAME = "auto run level editor";

        public static bool AutoRunLevelInEditor
        {
            get { return EditorPrefs.GetBool(AUTO_RUN_LEVEL_SAVE_NAME, false); }
            set { EditorPrefs.SetBool(AUTO_RUN_LEVEL_SAVE_NAME, value); }
        }

        private void CheckIfNeedToAutoRunLevel()
        {
            if (AutoRunLevelInEditor)
                LoadLevel(LevelController.DisplayedLevelIndex);

            AutoRunLevelInEditor = false;
        }
#endif

        #endregion

    }
}