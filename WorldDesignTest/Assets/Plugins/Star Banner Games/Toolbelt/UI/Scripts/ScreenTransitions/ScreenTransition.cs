using System;
using UnityEngine;

namespace SBG.Toolbelt.UI
{
    /// <summary>
    /// Use this class to invoke ScreenTransitions with a range of effects.
    /// All GameObjects are handled internally, using an Overlay Canvas with Sort Order 1000.
    /// </summary>
	public class ScreenTransition : MonoBehaviour
	{
        private enum TransitionType
        {
            None,
            Blackscreen,
            Fade,
            Cutout,
            Wipe,
        }

        private static ScreenTransition Instance
        {
            get
            {
                if (_instanceValue == null) GetInstance();
                return _instanceValue;
            }
        }

        private static ScreenTransition _instanceValue;


        [SerializeField] private BlackScreen blackscreen;
        [SerializeField] private ScreenFade screenFade;
        [SerializeField] private ScreenCutout screenCutout;
        [SerializeField] private ScreenWipe screenWipe;

        private Color _lastColor = Color.black;
        private Coroutine _transitionRoutine;


        private static void GetInstance()
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/ScreenTransitionCanvas");

            GameObject go = Instantiate(prefab);
            go.name = prefab.name; //Removes "Clone" from the Instance Name

            _instanceValue = go.GetComponent<ScreenTransition>();
            DontDestroyOnLoad(go);
        }


        private void Awake()
        {
            if (_instanceValue != null && _instanceValue != this)
            {
                Destroy(gameObject);
                return;
            }

            blackscreen.Init();
            screenFade.Init();
            screenCutout.Init();
            screenWipe.Init();
        }

        /// <summary>
        /// Disables all Transition Objects except the inteded one
        /// </summary>
        private void SetFocus(TransitionType transitionType, bool transitionToColor)
        {
            switch (transitionType)
            {
                case TransitionType.Blackscreen:
                    screenFade.DisableInstantly();
                    screenCutout.DisableInstantly();
                    screenWipe.DisableInstantly();
                    break;
                case TransitionType.Fade:
                    if (!transitionToColor) blackscreen.Disable();
                    screenCutout.DisableInstantly();
                    screenWipe.DisableInstantly();
                    break;
                case TransitionType.Cutout:
                    if (!transitionToColor) blackscreen.Disable();
                    screenFade.DisableInstantly();
                    screenWipe.DisableInstantly();
                    break;
                case TransitionType.Wipe:
                    if (!transitionToColor) blackscreen.Disable();
                    screenFade.DisableInstantly();
                    screenCutout.DisableInstantly();
                    break;
                case TransitionType.None:
                    blackscreen.Disable();
                    screenFade.DisableInstantly();
                    screenCutout.DisableInstantly();
                    screenWipe.DisableInstantly();
                    break;
            }
        }

        private void OnTransitionComplete(Color endColor)
        {
            ///If we transitioned to a color, turn on the blackscreen.
            ///This way we can follow up with whatever other transition we want
            if (endColor != Color.clear)
            {
                _lastColor = endColor;
                ShowBlackscreen(_lastColor);
            }
            else
            {
                HideBlackscreen();
            }
        }

        #region BLACKSCREEN

        public static void ShowBlackscreen(Color screenColor)
        {
            Instance.SetFocus(TransitionType.Blackscreen, true);
            Instance.blackscreen.ShowBlackscreen(screenColor);
            Instance._lastColor = screenColor;
        }

        public static void HideBlackscreen()
        {
            Instance.SetFocus(TransitionType.None, false);
        }

        #endregion

        #region SCREEN_FADE

        public static void FadeToBlack(float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null) => FadeToColor(Color.black, duration, startDelay, fadeCompleteCallback);
        public static void FadeToWhite(float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null) => FadeToColor(Color.white, duration, startDelay, fadeCompleteCallback);

        public static void FadeToColor(Color color, float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null)
        {
            Instance.StartFade(true, color, duration, startDelay, fadeCompleteCallback);
        }

        public static void FadeToGame(float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null)
        {   
            Instance.screenFade.SetColor(Instance._lastColor);
            Instance.StartFade(false, Color.clear, duration, startDelay, fadeCompleteCallback);
        }

        public void StartFade(bool transitionToColor, Color color, float duration, float startDelay, Action fadeCompleteCallback)
        {
            color = THelper.SnapColorToOpaque(color, true);

            if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);

            SetFocus(TransitionType.Fade, transitionToColor);

            _transitionRoutine = StartCoroutine(screenFade.CO_ExecuteFade(transitionToColor, color, duration, startDelay, fadeCompleteCallback, OnTransitionComplete));
        }

        #endregion

        #region SCREEN_CUTOUT

        public static void SetCutoutShape(Sprite cutoutShape)
        {
            Instance.screenCutout.SetCutoutShape(cutoutShape);
        }

        public static void CutoutToBlack(float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null) => CutoutToColor(Color.black, duration, startDelay, fadeCompleteCallback);

        public static void CutoutToWhite(float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null) => CutoutToColor(Color.white, duration, startDelay, fadeCompleteCallback);

        public static void CutoutToColor(Color color, float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null)
        {
            Instance.StartCutout(true, color, duration, startDelay, fadeCompleteCallback);
        }

        public static void CutoutToGame(float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null)
        {
            Instance.StartCutout(false, duration, startDelay, fadeCompleteCallback);
        }

        private void StartCutout(bool transitionToColor, float duration, float startDelay, Action fadeCompleteCallback)
        {
            StartCutout(transitionToColor, _lastColor, duration, startDelay, fadeCompleteCallback);
        }

        private void StartCutout(bool transitionToColor, Color color, float duration, float startDelay, Action fadeCompleteCallback)
        {
            color = THelper.SnapColorToOpaque(color, true);

            if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);

            screenCutout.SetColor(color);
            SetFocus(TransitionType.Cutout, transitionToColor);

            _transitionRoutine = StartCoroutine(screenCutout.CO_ExecuteCutout(transitionToColor, duration, startDelay, fadeCompleteCallback, OnTransitionComplete));
        }

        #endregion

        #region SCREEN_WIPE

        public static void WipeToBlack(ScreenWipe.WipeDirection direction, float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null) => WipeToColor(direction, Color.black, duration, startDelay, fadeCompleteCallback);

        public static void WipeToWhite(ScreenWipe.WipeDirection direction, float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null) => WipeToColor(direction, Color.white, duration, startDelay, fadeCompleteCallback);

        public static void WipeToColor(ScreenWipe.WipeDirection direction, Color color, float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null)
        {
            Instance.StartWipe(true, direction, color, duration, startDelay, fadeCompleteCallback);
        }

        public static void WipeToGame(ScreenWipe.WipeDirection direction, float duration = 1, float startDelay = 0, Action fadeCompleteCallback = null)
        {
            Instance.StartWipe(false, direction, duration, startDelay, fadeCompleteCallback);
        }


        public static void SetEdgeSprites(Sprite leadingEdge, Sprite trailingEdge, float edgeSize = 200)
        {
            Instance.screenWipe.SetEdgeSpritesLocal(leadingEdge, trailingEdge, edgeSize);
        }

        public static void SetEdgeSprites(ScreenWipe.EdgePresets preset)
        {
            Instance.screenWipe.SetEdgeSpritesLocal(preset);
        }

        private void StartWipe(bool transitionToColor, ScreenWipe.WipeDirection direction, float duration, float startDelay, Action fadeCompleteCallback)
        {
            StartWipe(transitionToColor, direction, _lastColor, duration, startDelay, fadeCompleteCallback);
        }

        private void StartWipe(bool transitionToColor, ScreenWipe.WipeDirection direction, Color color, float duration, float startDelay, Action fadeCompleteCallback)
        {
            color = THelper.SnapColorToOpaque(color, true);

            if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);

            screenWipe.SetColor(color);
            SetFocus(TransitionType.Wipe, transitionToColor);

            _transitionRoutine = StartCoroutine(screenWipe.CO_ExecuteWipe(transitionToColor, direction, duration, startDelay, fadeCompleteCallback, OnTransitionComplete));
        }


        #endregion
    }
}