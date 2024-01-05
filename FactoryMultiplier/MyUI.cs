using System;
using UnityEngine;
using UnityEngine.UI;

namespace FactoryMultiplier
{
    public class MyUI
    {
        public string Title;
        public GUI.WindowFunction DrawUi;
        public readonly int WindowId = 0;
        public Rect WinRect;
        public float _width;
        public float _height;
        private readonly RectTransform _rt;
        private readonly GameObject _canvasObj;
        private bool isShown = false;

        public MyUI(GUI.WindowFunction drawUi, float width, float height, string title = "", int windowId = 0)
        {
            DrawUi = drawUi;
            if (windowId == 0)
            {
                windowId = UnityEngine.Random.Range(268435456, 536870912);
            }
            WindowId = windowId;
            Title = title;
            _width = width;
            _height = height;
            _canvasObj = new GameObject("NextBlockerCanvas");
            _canvasObj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            _canvasObj.AddComponent<GraphicRaycaster>();
            GameObject gameObject = new GameObject("RayBlocker");
            _rt = gameObject.AddComponent<RectTransform>();
            _rt.SetParent(_canvasObj.transform);
            _rt.pivot = new Vector2(0f, 1f);
            Image image = gameObject.AddComponent<Image>();
            image.color = Color.clear;
            image.raycastTarget = true;
            CloseBlocker();
            UnityEngine.Object.DontDestroyOnLoad(_canvasObj);
        }

        public void OpenBlocker()
        {
            _canvasObj.SetActive(true);
        }

        public void CloseBlocker()
        {
            _canvasObj.SetActive(false);
        }

        public void Render(bool show)
        {
            if (!show)
            {
                isShown = false;
                CloseBlocker();
            }
            else
            {
                if (!isShown)
                {
                    isShown = true;
                    WinRect = new Rect(((float)Screen.width - _width) / 2f, ((float)Screen.height - _height) / 2f, _width, _height);
                    OpenBlocker();
                }
                WinRect = GUI.Window(WindowId, WinRect, DrawUi, Title);
                _rt.sizeDelta = WinRect.size;
                _rt.position = new Vector3(WinRect.position.x, (float)Screen.height - WinRect.position.y);
                Cursor.visible = true;
            }
        }
    }
}