using RoR2.UI;
using SoulLink.Util;
using UnityEngine;
using UnityEngine.UI;

namespace SoulLink.UI
{
    internal class SoulLinkPanel : MonoBehaviour
    {
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

    }
}
