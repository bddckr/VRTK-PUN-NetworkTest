namespace NetworkTest
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using UnityEngine.VR;
    using VRTK;

    public sealed class SDKChooser : MonoBehaviour
    {
        public VRTK_SDKManager SDKManager;
        public List<SDKSetup> Setups;
        public Transform ButtonList;

        public UnityEvent SDKWasChosen = new UnityEvent();

        private void OnEnable()
        {
            Func<VRTK_SDKInfo, string> selector = info => info.description.prettyName;
            var availableSDKInfoPrettyNames = VRTK_SDKManager.AvailableSystemSDKInfos.Select(selector)
                                                             .Concat(VRTK_SDKManager.AvailableBoundariesSDKInfos.Select(selector))
                                                             .Concat(VRTK_SDKManager.AvailableHeadsetSDKInfos.Select(selector))
                                                             .Concat(VRTK_SDKManager.AvailableControllerSDKInfos.Select(selector))
                                                             .Distinct();
            var installedSDKInfoPrettyNames = VRTK_SDKManager.InstalledSystemSDKInfos.Select(selector)
                                                             .Concat(VRTK_SDKManager.InstalledBoundariesSDKInfos.Select(selector))
                                                             .Concat(VRTK_SDKManager.InstalledHeadsetSDKInfos.Select(selector))
                                                             .Concat(VRTK_SDKManager.InstalledControllerSDKInfos.Select(selector))
                                                             .Distinct()
                                                             .ToList();

            foreach (var prettyName in availableSDKInfoPrettyNames)
            {
                var isFallback = prettyName == SDK_DescriptionAttribute.Fallback.prettyName;
                if (isFallback)
                {
                    continue;
                }

                var isInstalled = installedSDKInfoPrettyNames.Contains(prettyName);
                var setup = Setups.FirstOrDefault(scene => scene.PrettyName == prettyName);

                if (!isInstalled)
                {
                    AddButton(string.Format("{0}\n(not installed)", prettyName), null);
                }
                else if (setup == null)
                {
                    AddButton(string.Format("{0}\n(no matching Scene in the setups)", prettyName), null);
                }
                else if (!Application.CanStreamedLevelBeLoaded(setup.SceneToLoad))
                {
                    AddButton(string.Format("{0}\n(Scene '{1}' not found in build)", prettyName, setup.SceneToLoad), null);
                }
                else if (!VRSettings.supportedDevices.Contains(setup.VRDeviceNameToLoad))
                {
                    AddButton(string.Format("{0}\n(VR Device '{1}' not included in build)", prettyName, setup.VRDeviceNameToLoad), null);
                }
                else
                {
                    AddButton(prettyName, () => HandleButtonClick(setup));
                }
            }

            Destroy(ButtonList.GetChild(0).gameObject);
        }

        private void AddButton(string sdkName, UnityAction onClickAction)
        {
            var newButton = Instantiate(ButtonList.GetChild(0).gameObject);
            var newButtonTransform = newButton.transform;

            newButtonTransform.GetComponentInChildren<Text>().text = sdkName;
            newButtonTransform.SetParent(ButtonList, false);

            if (onClickAction == null)
            {
                newButton.GetComponentInChildren<Button>().interactable = false;
            }
            else
            {
                newButtonTransform.GetComponentInChildren<Button>().onClick.AddListener(onClickAction);
            }
        }

        private void HandleButtonClick(SDKSetup setup)
        {
            UnityAction<Scene, LoadSceneMode> onSceneLoaded = null;
            onSceneLoaded = (scene, loadMode) =>
            {
                if (scene.name != setup.SceneToLoad)
                {
                    return;
                }

                SceneManager.sceneLoaded -= onSceneLoaded;

                SDKManager.PopulateObjectReferences(true);
                SDKManager.gameObject.SetActive(true);

                Destroy(gameObject);

                SDKWasChosen.Invoke();
            };
            SceneManager.sceneLoaded += onSceneLoaded;

            SDKManager.autoPopulateObjectReferences = false;

            Func<VRTK_SDKInfo, bool> predicate = info => info.description.prettyName == setup.PrettyName;
            SDKManager.systemSDKInfo = VRTK_SDKManager.InstalledSystemSDKInfos.First(predicate);
            SDKManager.boundariesSDKInfo = VRTK_SDKManager.InstalledBoundariesSDKInfos.First(predicate);
            SDKManager.headsetSDKInfo = VRTK_SDKManager.InstalledHeadsetSDKInfos.First(predicate);
            SDKManager.controllerSDKInfo = VRTK_SDKManager.InstalledControllerSDKInfos.First(predicate);

            VRSettings.LoadDeviceByName(setup.VRDeviceNameToLoad);
            StartCoroutine(LoadSceneAfterFrameDelay(setup));
        }

        private static IEnumerator LoadSceneAfterFrameDelay(SDKSetup setup)
        {
            yield return null;

            SceneManager.LoadScene(
                setup.SceneToLoad,
                LoadSceneMode.Additive
            );
        }

        [Serializable]
        public sealed class SDKSetup
        {
            public string PrettyName;
            public string SceneToLoad;
            public string VRDeviceNameToLoad;
        }
    }
}
