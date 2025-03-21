using HarmonyLib;
using RoR2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SoulLink.Util
{
    public static class AssetUtil
    {
        public static AssetBundle bundle;
        public const string bundleName = "soullinkassets";

        public static Sprite defaultSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
        public static GameObject defaultModel = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

        public static string AssetBundlePath
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(SoulLink.SavedInfo.Location), bundleName);
            }
        }

        public static void Init()
        {
            bundle = AssetBundle.LoadFromFile(AssetBundlePath);
        }

        /// <summary>
        /// Loads the requested asset from the AssetBundle defined in AssetUtil. If the given filename is invalid, it falls back to the game's default Mystery icon.
        /// </summary>
        /// <param name="fileName">The filename of the asset as it appears in the AssetBundle. You must include the file extenstion.</param>
        /// <returns>The loaded asset, if it exists within the AssetBundle. Otherwise, returns null.</returns>
        private static T SafeLoad<T>(string fileName) where T : UnityEngine.Object
        {
            try
            {
                //Log.Debug($"Loading Template asset...");
                var loaded = AssetUtil.bundle.LoadAsset<T>(fileName);

                if (loaded == null)
                {
                    Log.Debug($"Asset {fileName} not found in the AssetBundle. Using fallback asset.");
                }
                return loaded;
                //Log.Debug($"Template asset loaded!");
            }
            catch (Exception e)
            {
                Log.Error($"Error loading asset {fileName}: {e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Loads the requested Sprite from the AssetBundle.
        /// </summary>
        /// <param name="filename">The filename of the asset as it appears in the AssetBundle.</param>
        /// <returns>The loaded Sprite, if it exists within the AssetBundle. Otherwise, returns the game's default Mystery icon.</returns>
        public static Sprite LoadSprite(string filename)
        {
            if (!filename.Contains("."))
            {
                filename += ".png"; // Default handling if sent with no extension.
            }
            return SafeLoad<Sprite>(filename) ?? defaultSprite;
        }

        /// <summary>
        /// Loads the requested GameObject (3D model) from the AssetBundle.
        /// </summary>
        /// <param name="filename">The filename of the asset as it appears in the AssetBundle.</param>
        /// <returns>The loaded GameObject, if it exists within the AssetBundle. Otherwise, returns the game's default Mystery icon.</returns>
        public static GameObject LoadModel(string filename)
        {
            if (!filename.Contains("."))
            {
                filename += ".prefab"; // Default handling if sent with no extension.
            }
            return AddLogbookComponents(SafeLoad<GameObject>(filename)) ?? defaultModel;
        }

        /// <summary>
        /// Loads a Sprite from the base game Risk of Rain 2 libraries. You must have the full path for the sprite.
        /// </summary>
        /// <param name="path">The path to the sprite in the game's files. Ex. "RoR2/Base/Common/MiscIcons/texMysteryIcon.png"</param>
        /// <returns>The loaded Sprite from the game.</returns>
        public static Sprite LoadBaseGameSprite(string path)
        {
            return Addressables.LoadAssetAsync<Sprite>(path).WaitForCompletion();
        //public static GameObject defaultModel = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();
        }

        /// <summary>
        /// Loads a Texture2D from the base game Risk of Rain 2 libraries. You must have the full path for the sprite.
        /// </summary>
        /// <param name="path">The path to the sprite in the game's files. Ex. "RoR2/Base/Brother/texBrotherIcon.png"</param>
        /// <returns>The loaded Sprite from the game.</returns>
        public static Texture2D LoadBaseGameTexture(string path)
        {
            return Addressables.LoadAssetAsync<Texture2D>(path).WaitForCompletion();
        //public static GameObject defaultModel = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();
        }

        /// <summary>
        /// Loads a GameObject (3D Model) from the base game Risk of Rain 2 libraries. You must have the full path for the sprite.
        /// </summary>
        /// <param name="path">The path to the model in the game's files. Ex. "RoR2/Base/Mystery/PickupMystery.prefab"</param>
        /// <returns>The loaded GameObject from the game.</returns>
        public static GameObject LoadBaseGameModel(string path)
        {
            return Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
        }

        // Shoutout to the Sandswept Team for inspiring this solution.
        public static GameObject AddLogbookComponents(GameObject itemModel)
        {
            if (itemModel.GetComponent<ModelPanelParameters>() != null) return itemModel;

            // Add focus component
            GameObject focus = new GameObject("Focus");
            focus.transform.parent = itemModel.transform;
            MeshRenderer biggestRenderer = itemModel.GetComponentsInChildren<MeshRenderer>().ToList().OrderByDescending(x => SumVectorDims(x.bounds.size)).First();
            focus.transform.parent = itemModel.transform;
            focus.transform.position = biggestRenderer.bounds.center;

            // Add camera component
            GameObject camera = new GameObject("Camera");
            camera.transform.parent = itemModel.transform;
            camera.transform.parent = itemModel.transform;
            camera.transform.localPosition = focus.transform.position;

            // Add model panel parameters component
            var modelPanelParameters = itemModel.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = focus.transform;
            modelPanelParameters.cameraPositionTransform = camera.transform;
            modelPanelParameters.minDistance = .1f * SumVectorDims(biggestRenderer.bounds.size);
            modelPanelParameters.maxDistance = 1f * SumVectorDims(biggestRenderer.bounds.size);

            // Add components for item display
            List<Renderer> renderers = itemModel.GetComponentsInChildren<MeshRenderer>().ToList<Renderer>();
            renderers.AddRange(itemModel.GetComponentsInChildren<SkinnedMeshRenderer>());

            CharacterModel.RendererInfo[] renderInfos = new CharacterModel.RendererInfo[renderers.Count];

            for (int i = 0; i < renderers.Count; i++)
            {
                renderInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = renderers[i] is SkinnedMeshRenderer ? renderers[i].sharedMaterial : renderers[i].material,
                    renderer = renderers[i],
                    ignoreOverlays = false,
                    hideOnDeath = true
                };
            }

            var displayComp = itemModel.AddComponent<ItemDisplay>();
            displayComp.rendererInfos = renderInfos;

            return itemModel;
        }

        private static float SumVectorDims(Vector3 vector)
        {
            return Mathf.Abs(vector.x) +Mathf.Abs(vector.y) + Mathf.Abs(vector.z);
        }
    }

}
