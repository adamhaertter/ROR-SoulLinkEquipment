using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.UI;

namespace SoulLink.Util
{

    public class UIPopup : MonoBehaviour
    {
        private GameObject uiPanel;

        void Start()
        {
            CreateUI();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                ToggleUI();
            }
        }

        void CreateUI()
        {
            // Create a new GameObject for the UI panel
            uiPanel = new GameObject("CustomUIPanel");
            Canvas canvas = uiPanel.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Add a background panel
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(uiPanel.transform);
            RectTransform panelTransform = panel.AddComponent<RectTransform>();
            panelTransform.sizeDelta = new Vector2(400, 200);
            panelTransform.anchoredPosition = new Vector2(0, 0);

            // Add an Image component for background
            UnityEngine.UI.Image panelImage = panel.AddComponent<UnityEngine.UI.Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black

            // Add a Text component
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(panel.transform);
            RectTransform textTransform = textObj.AddComponent<RectTransform>();
            textTransform.sizeDelta = new Vector2(380, 100);
            textTransform.anchoredPosition = new Vector2(0, 0);

            UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
            text.text = "This is a custom UI popup!";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            // Initially hide the panel
            uiPanel.SetActive(false);
        }

        void ToggleUI()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(!uiPanel.activeSelf);
            }
        }
    }
}

