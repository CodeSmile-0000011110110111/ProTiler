using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SBG.Toolbelt.UI
{
    [Serializable]
	public class ScreenWipe
	{
        public enum WipeDirection
        {
            RightToLeft,
            LeftToRight,
            TopToBottom,
            BottomToTop,
        }

        public enum EdgePresets
        {
            None,
            Triangular,
            Spikes,
            Orbs,
            Slant,
        }

        [SerializeField] private Image wipePanel;
        [Space]
        [SerializeField] private Image leftEdge;
        [SerializeField] private Image rightEdge;
        [SerializeField] private Image topEdge;
        [SerializeField] private Image bottomEdge;

        private float _edgeSize;

        private Sprite _leadingSprite;
        private Sprite _trailingSprite;


        public void Init()
        {
            DisableInstantly();

            SetEdgeSpritesLocal(null, null, 0f);
        }

        public void DisableInstantly()
        {
            if (!wipePanel.enabled) return;

            SetColor(Color.clear);
            wipePanel.raycastTarget = false;
            wipePanel.enabled = false;
        }

        public void SetColor(Color col)
        {
            wipePanel.color = col;
            rightEdge.color = col;
            leftEdge.color = col;
            topEdge.color = col;
            bottomEdge.color = col;
        }

        public void SetEdgeSpritesLocal(Sprite leading, Sprite trailing, float edgeSize)
        {
            _edgeSize = edgeSize;
            _leadingSprite = leading;
            _trailingSprite = trailing;
        }

        public void SetEdgeSpritesLocal(EdgePresets preset)
        {
            string leadName = "";
            string trailName = "";

            //Get the Preset Resource Names
            switch (preset)
            {
                case EdgePresets.Triangular:
                    leadName = "TransitionEdge_Triangular_Leading";
                    trailName = "TransitionEdge_Triangular_Trailing";
                    break;
                case EdgePresets.Spikes:
                    leadName = "TransitionEdge_Spikes_Leading";
                    trailName = "TransitionEdge_Spikes_Trailing";
                    break;
                case EdgePresets.Orbs:
                    leadName = "TransitionEdge_Orbs_Leading";
                    trailName = "TransitionEdge_Orbs_Trailing";
                    break;
                case EdgePresets.Slant:
                    leadName = "TransitionEdge_Slant_Leading";
                    trailName = "TransitionEdge_Slant_Trailing";
                    break;
                case EdgePresets.None:
                    SetEdgeSpritesLocal(null, null, 0f);
                    return;
            }

            //Load Presets from Resources
            Sprite leading = Resources.Load<Sprite>($"Sprites/{leadName}");
            Sprite trailing = Resources.Load<Sprite>($"Sprites/{trailName}");

            //Apply Presets
            _edgeSize = 200;
            _leadingSprite = leading;
            _trailingSprite = trailing;
        }

        private void RefreshEdges(WipeDirection direction)
        {
            Vector2 screenSize = wipePanel.rectTransform.rect.size;
            Vector2 screenHalfSize = screenSize / 2;

            //Set the Leading/Trailing Sprites to the relevant edges
            switch (direction)
            {
                case WipeDirection.RightToLeft:
                    ConfigureEdge(this.leftEdge, _leadingSprite, 0, 90, new Vector2(-screenHalfSize.x,0), new Vector2(screenSize.y, _edgeSize));
                    ConfigureEdge(this.rightEdge, _trailingSprite, 1, 90, new Vector2(screenHalfSize.x, 0), new Vector2(screenSize.y, _edgeSize));
                    break;
                case WipeDirection.LeftToRight:
                    ConfigureEdge(this.rightEdge, _leadingSprite, 0, -90, new Vector2(screenHalfSize.x, 0), new Vector2(screenSize.y, _edgeSize));
                    ConfigureEdge(this.leftEdge, _trailingSprite, 1, -90, new Vector2(-screenHalfSize.x, 0), new Vector2(screenSize.y, _edgeSize));
                    break;
                case WipeDirection.TopToBottom:
                    ConfigureEdge(this.bottomEdge, _leadingSprite, 0, 180, new Vector2(0, -screenHalfSize.y), new Vector2(screenSize.x, _edgeSize));
                    ConfigureEdge(this.topEdge, _trailingSprite, 1, 180, new Vector2(0, screenHalfSize.y), new Vector2(screenSize.x, _edgeSize));
                    break;
                case WipeDirection.BottomToTop:
                    ConfigureEdge(this.topEdge, _leadingSprite, 0, 0, new Vector2(0, screenHalfSize.y), new Vector2(screenSize.x, _edgeSize));
                    ConfigureEdge(this.bottomEdge, _trailingSprite, 1, 0, new Vector2(0, -screenHalfSize.y), new Vector2(screenSize.x, _edgeSize));
                    break;
                default:
                    break;
            }
        }

        private void ConfigureEdge(Image edge, Sprite newSprite, float yPivot, float angle, Vector2 position, Vector2 size)
        {
            edge.sprite = newSprite;
            edge.rectTransform.pivot = new Vector2(0.5f, yPivot);
            edge.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            edge.rectTransform.anchoredPosition = position;
            edge.rectTransform.sizeDelta = size;

            edge.enabled = newSprite != null && size != Vector2.zero;
        }

        private Vector2 GetPosFromDirection(RectTransform rectTransform, WipeDirection dir)
        {
            switch (dir)
            {
                case WipeDirection.RightToLeft: return new Vector2(wipePanel.rectTransform.rect.width + _edgeSize, 0);
                case WipeDirection.LeftToRight: return new Vector2(-(wipePanel.rectTransform.rect.width + _edgeSize), 0);
                case WipeDirection.TopToBottom: return new Vector2(0, wipePanel.rectTransform.rect.height + _edgeSize);
                case WipeDirection.BottomToTop: return new Vector2(0, -(wipePanel.rectTransform.rect.height + _edgeSize));
            }

            return Vector2.zero;
        }

        public IEnumerator CO_ExecuteWipe(bool transitionToColor, WipeDirection direction, float duration, float startDelay, Action callback, Action<Color> internalCallback)
        {
            float time = -startDelay;
            Vector2 startPos = transitionToColor ? GetPosFromDirection(wipePanel.rectTransform, direction) : Vector2.zero;
            Vector2 targetPos = transitionToColor ? Vector2.zero : -GetPosFromDirection(wipePanel.rectTransform, direction);

            //Enable Color Screen for Lerp
            wipePanel.enabled = true;
            wipePanel.raycastTarget = true;
            wipePanel.rectTransform.anchoredPosition = startPos;
            RefreshEdges(direction);
            yield return null; //Somehow waiting a frame helps the the time to work properly

            //EXECUTE WIPE LERP
            if (duration > 0)
            {
                while (time < duration)
                {
                    wipePanel.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, time / duration);

                    yield return null;
                    time += Time.deltaTime;
                }
            }

            //Clamp to Target Value
            wipePanel.rectTransform.anchoredPosition = targetPos;

            //Disable Screen if game is visible
            if (!transitionToColor)
            {
                wipePanel.enabled = false;
                wipePanel.raycastTarget = false;
            }

            //Invoke Callbacks
            internalCallback?.Invoke(transitionToColor ? wipePanel.color : Color.clear);
            callback?.Invoke();
        }

    }
}