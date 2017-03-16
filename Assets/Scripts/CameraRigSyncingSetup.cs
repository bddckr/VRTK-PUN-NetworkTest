namespace NetworkTest
{
    using UnityEngine;
    using VRTK;

    public sealed class CameraRigSyncingSetup : MonoBehaviour
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
        }
    }
}
