using RoR2.UI;
using SoulLink.Util;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.ComponentModel;

namespace SoulLink.UI
{
    internal class SoulLinkPanel : MonoBehaviour
    {

        private string lastSelectedOption;
        private List<Image> selectableImages = new List<Image>();
        private static KeyCode[] optionSelectKeybinds = new KeyCode[]
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
            KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6,
            KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
        };

        public static void Toggle()
        {
            GameObject panel = (GameObject.Find("HUDSimple(Clone)") ?? GameObject.Find("RiskUI(Clone)")).transform
                .Find("MainContainer")
                .Find("MainUIArea")
                .Find("SpringCanvas")
                .Find("SoulLinkPanel")
                ?.gameObject;

            if (panel)
                Destroy(panel);
            else
                Show();
        }

        public static void Show()
        {
            var container = (GameObject.Find("HUDSimple(Clone)") ?? GameObject.Find("RiskUI(Clone)")).transform
                .Find("MainContainer")
                .Find("MainUIArea")
                .Find("SpringCanvas");

            SoulLinkPanel soulLinkPanel = CreateUI(container);
            soulLinkPanel.Render();
        }

        public void Render()
        {
            GameObject labelObj = AssetUtil.LoadBaseGameModel("RoR2/Base/UI/DefaultLabel.prefab");
            var soulLinkLabel = Instantiate(labelObj, transform);

            RectTransform labelRect = (RectTransform)soulLinkLabel.transform;
            labelRect.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            labelRect.pivot = new Vector2(0f, -.1f);
            labelRect.SetAsFirstSibling();

            HGTextMeshProUGUI textMesh = soulLinkLabel.GetComponent<HGTextMeshProUGUI>();
            textMesh.text = "Forge Your Bond";
            textMesh.color = Color.white;
            textMesh.fontSize = 25;

            Vector2 contentsDimensions = CreateImageGrid(LoadExampleSprites());
            RectTransform myBG = GetComponent<RectTransform>();
            myBG.sizeDelta = new Vector2(contentsDimensions.x + 20, contentsDimensions.y + 20);
        }

        private Vector2 CreateImageGrid(Sprite[] imageSprites)
        {
            if (imageSprites == null || imageSprites.Length == 0)
            {
                Debug.LogWarning("No images provided.");
                return new Vector2(0, 0); // Bad grid, no dimensions
            }
            GameObject windowContainer = new GameObject("WindowContainer", typeof(RectTransform));
            windowContainer.transform.SetParent(transform, false);

            RectTransform fullContainerRect = windowContainer.GetComponent<RectTransform>();
            fullContainerRect.anchorMin = new Vector2(0.5f, 0.5f);
            fullContainerRect.anchorMax = new Vector2(0.5f, 0.5f);
            fullContainerRect.pivot = new Vector2(0.5f, 0.5f);
            fullContainerRect.sizeDelta = Vector2.zero; // Start at zero, expand as needed

            GameObject gridContainer = new GameObject("ImageGrid", typeof(RectTransform));
            gridContainer.transform.SetParent(windowContainer.transform, false);

            //RectTransform gridRect = gridContainer.AddComponent<RectTransform>();
            //gridRect.sizeDelta = new Vector2(400, 300);

            GridLayoutGroup gridLayout = gridContainer.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(90, 110); // Image size + padding
            gridLayout.spacing = new Vector2(10, 10); // Space between images
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 3; // 3 columns per row
            gridLayout.childAlignment = TextAnchor.MiddleCenter;

            RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.5f, 0.5f);
            gridRect.anchorMax = new Vector2(0.5f, 0.5f);
            gridRect.pivot = new Vector2(0.5f, 0.5f);

            for (int i = 0; i < imageSprites.Length; i++)
            {
                CreateImage(gridContainer.transform, imageSprites[i], i);
            }

            int rows = Mathf.Min(3, Mathf.CeilToInt(imageSprites.Length / 3f)); // 3 images per row
            float totalHeight = rows * (gridLayout.cellSize.y + gridLayout.spacing.y) + 20; // Padding
            float totalWidth = 3 * (gridLayout.cellSize.x + gridLayout.spacing.x) + 20;

            fullContainerRect.sizeDelta = new Vector2(totalWidth, totalHeight);
            return new Vector2(totalWidth + 20, totalHeight + 20);
        }

        private void CreateImage(Transform parent, Sprite imageSprite, int index)
        {
            GameObject fullContainer = new GameObject("ImageContainer", typeof(RectTransform));
            fullContainer.transform.SetParent(parent, false);

            VerticalLayoutGroup layout = fullContainer.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 5;
            layout.padding = new RectOffset(0, 0, 5, 5);

            GameObject imgObj = new GameObject($"Image_{index}");
            imgObj.transform.SetParent(fullContainer.transform, false);

            RectTransform rectTransform = imgObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(75, 75);

            Image image = imgObj.AddComponent<Image>();
            image.sprite = imageSprite;
            image.color = Color.white;
            image.material = null;
            image.preserveAspect = true;

            GameObject keybindLabel = new GameObject($"KeybindLabel_{index}");
            keybindLabel.transform.SetParent(fullContainer.transform, false);

            TextMeshProUGUI textComponent = keybindLabel.AddComponent<TextMeshProUGUI>();
            textComponent.text = optionSelectKeybinds[index].ToString().Replace("Alpha", "");
            textComponent.fontSize = 20;
            textComponent.alignment = TextAlignmentOptions.Center;

            RectTransform textRect = keybindLabel.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(75, 30);
            textRect.pivot = new Vector2(0.5f, 0);
            textRect.anchoredPosition = Vector2.zero;

            selectableImages.Add(image);
        }

        private void Update()
        {
            if (selectableImages.Count == 0) return;

            for (int i = 0; i < Mathf.Min(9, selectableImages.Count); i++)
            {
                if (Input.GetKeyDown(optionSelectKeybinds[i]))
                {
                    OnImageSelected(i);
                }
            }
        }

        private void OnImageSelected(int index)
        {
            if (index < 0 || index >= selectableImages.Count) return;

            lastSelectedOption = $"Option {index}";
            Debug.Log($"Selected: {lastSelectedOption}");

            Toggle(); // Close UI after selection
        }

        public static SoulLinkPanel CreateUI(Transform parent)
        {
            GameObject panelObject = new GameObject("SoulLinkPanel");
            panelObject.transform.SetParent(parent, false);

            RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400, 300); // Adjust as needed

            Image background = panelObject.AddComponent<Image>();
            background.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black

            SoulLinkPanel panel = panelObject.AddComponent<SoulLinkPanel>();
            return panel;
        }

        // Method just for debugging the UI Grid
        private Sprite[] LoadExampleSprites()
        {
            // Replace with actual loading logic
            return new Sprite[]
            {
                ConvertTextureToSprite(AssetUtil.LoadBaseGameTexture("RoR2/Base/Bandit2/texBanditIcon.png")),
                ConvertTextureToSprite(AssetUtil.LoadBaseGameTexture("RoR2/Base/Brother/texBrotherIcon.png")),
                ConvertTextureToSprite(AssetUtil.LoadBaseGameTexture("RoR2/Base/Captain/texCaptainIcon.png")),
                ConvertTextureToSprite(AssetUtil.LoadBaseGameTexture("RoR2/Base/Engi/texEngiIcon.png")),
                ConvertTextureToSprite(AssetUtil.LoadBaseGameTexture("RoR2/Base/Heretic/texHereticIcon.png")),
                AssetUtil.LoadBaseGameSprite("RoR2/Base/Common/texSurvivorBGIcon.png"),
                AssetUtil.LoadBaseGameSprite("RoR2/Base/Bear/texBearIcon.png"),
                ConvertTextureToSprite(AssetUtil.LoadBaseGameTexture("RoR2/Base/ArtifactShell/texUnidentifiedKillerIcon.png")),
            };
        }

        public static Sprite ConvertTextureToSprite(Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogError("ConvertTextureToSprite received a null texture!");
                return null;
            }

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }


    }
}
