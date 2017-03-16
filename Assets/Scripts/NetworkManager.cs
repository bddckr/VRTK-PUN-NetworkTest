namespace NetworkTest
{
    using Photon;
    using UnityEngine;

    public sealed class NetworkManager : PunBehaviour
    {
        private const string GameNetworkVersion = "0.0.1";

        private void OnEnable()
        {
            PhotonNetwork.ConnectUsingSettings(GameNetworkVersion);
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            PhotonNetwork.CreateRoom("Test Room");
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.Instantiate("VRPlayerNetworkRepresentation", Vector3.zero, Quaternion.identity, 0);
        }
    }
}
