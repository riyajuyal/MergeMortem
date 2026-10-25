using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MergeMortem
{
    /// <summary>
    /// Builds the entire uGUI canvas and every UI element from code at runtime,
    /// then wires GameState to GameUI.
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        void Awake()
        {
            EnsureEventSystem();

            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(480, 854);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            GameObject uiRootGO = new GameObject("GameUI", typeof(RectTransform));
            uiRootGO.transform.SetParent(canvasGO.transform, false);
            RectTransform uiRootRect = uiRootGO.GetComponent<RectTransform>();
            uiRootRect.anchorMin = Vector2.zero;
            uiRootRect.anchorMax = Vector2.one;
            uiRootRect.offsetMin = Vector2.zero;
            uiRootRect.offsetMax = Vector2.zero;

            GameUI ui = uiRootGO.AddComponent<GameUI>();
            GameState state = new GameState();
            ui.Init(state);
        }

        void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
    }
}
