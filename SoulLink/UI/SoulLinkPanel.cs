using RoR2.UI;
using SoulLink.Util;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SoulLink.UI
{
    internal class SoulLinkPanel : MonoBehaviour
    {

        private string lastSelectedOption;
        private List<Image> selectableImages = new List<Image>();

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

            CreateImageGrid(LoadExampleSprites());
        }

        private void CreateImageGrid(Sprite[] imageSprites)
        {
            if (imageSprites == null || imageSprites.Length == 0)
            {
                Debug.LogWarning("No images provided.");
                return;
            }

            GameObject gridContainer = new GameObject("ImageGrid");
            gridContainer.transform.SetParent(transform, false);

            RectTransform gridRect = gridContainer.AddComponent<RectTransform>();
            gridRect.sizeDelta = new Vector2(400, 300);

            GridLayoutGroup gridLayout = gridContainer.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(75, 75);
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = Mathf.CeilToInt(Mathf.Sqrt(imageSprites.Length));

            for (int i = 0; i < imageSprites.Length; i++)
            {
                CreateImage(gridContainer.transform, imageSprites[i], i);
            }
        }

        private void CreateImage(Transform parent, Sprite imageSprite, int index)
        {
            GameObject imgObj = new GameObject($"Image_{index}");
            imgObj.transform.SetParent(parent, false);

            RectTransform rectTransform = imgObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(75, 75);

            Image image = imgObj.AddComponent<Image>();
            image.sprite = imageSprite;
            image.color = Color.white;
            image.material = null;

            selectableImages.Add(image);
        }

        private void Update()
        {
            if (selectableImages.Count == 0) return;

            for (int i = 0; i < Mathf.Min(9, selectableImages.Count); i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
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
