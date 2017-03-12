using UnityEngine;
using VRTK;

public sealed class SetUpCameraRigSyncing : MonoBehaviour
{
    public GameObject HeadsetNetworkRepresentation;
    public GameObject LeftControllerNetworkRepresentation;
    public GameObject RightControllerNetworkRepresentation;

    private void OnEnable()
    {
        SetUpTransformFollow(HeadsetNetworkRepresentation, VRTK_DeviceFinder.Devices.Headset);
        SetUpTransformFollow(LeftControllerNetworkRepresentation, VRTK_DeviceFinder.Devices.LeftController);
        SetUpTransformFollow(RightControllerNetworkRepresentation, VRTK_DeviceFinder.Devices.RightController);
    }

    private static void SetUpTransformFollow(GameObject networkRepresentation, VRTK_DeviceFinder.Devices device)
    {
        var photonView = networkRepresentation.GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError(string.Format("The network representation '{0}' has no {1} component on it.", networkRepresentation.name, typeof(PhotonView).Name));
            return;
        }

        if (!photonView.isMine)
        {
            return;
        }

        var transformFollow = networkRepresentation.AddComponent<VRTK_TransformFollow>();
        transformFollow.gameObjectToFollow = VRTK_DeviceFinder.DeviceTransform(device).gameObject;
        transformFollow.followsScale = false;

        /*
         * Work around stupid bug in VRTK_TransformFollow:
         * VRTK_TransformFollow only caches the transforms in OnEnable but we can't set
         * gameObjectToFollow before OnEnable is run, so we trigger it again here.
         * 
         * This will be fixed in the future by @bddckr so the following isn't needed anymore.
         */
        transformFollow.enabled = false;
        transformFollow.enabled = true;
        // Bug fix end.
    }
}
