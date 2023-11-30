using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using Unity.Netcode;

using UnityEngine;

namespace Bonkers.Lobby
{
    using System;

    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private Transform spawnedObjectPrefab;
        private                  Transform spawnedObjectTransform;

        private NetworkVariable<MyCustomData> randomNumber = new
            (new MyCustomData { _int = 56, _bool = false, }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public struct MyCustomData : INetworkSerializable
        {
            public Int32   _int;
            public Boolean _bool;
            public String  _message;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _int);
                serializer.SerializeValue(ref _bool);
            }
        }

        public override void OnNetworkSpawn()
        {
            randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
            {
                Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool);
            };
        }

        private void Update()
        {
            if (!IsOwner) { return; }

            if (Input.GetKeyDown(KeyCode.T))
            {
                spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
                spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
                //TestClientRpc();
                //randomNumber.Value = new MyCustomData
                //{
                //    _int = 10,
                //    _bool = false,
                //};
            }

            if (Input.GetKeyDown(KeyCode.Y)) { Destroy(spawnedObjectTransform); }

            Vector3 moveDir = new (0, 0, 0);

            if (Input.GetKey(KeyCode.W)) { moveDir.y = +1f; }

            if (Input.GetKey(KeyCode.A)) { moveDir.x = -1f; }

            if (Input.GetKey(KeyCode.S)) { moveDir.y = -1f; }

            if (Input.GetKey(KeyCode.D)) { moveDir.x = +1f; }

            Single moveSpeed = 3f;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        [ServerRpc]
        private void TestServerRpc() { Debug.Log("TestServerRpc" + OwnerClientId); }

        [ClientRpc]
        private void TestClientRpc() { Debug.Log("test client rpc"); }
    }
}