using Unity.Netcode;
using UnityEngine;

namespace NobleConnect.Examples.NetCodeForGameObjects
{
    // A super simple example player. Use arrow keys to move
    public class NetCodeForGameObjectsExamplePlayer : NetworkBehaviour
    {
        void Update()
        {
            if (!IsLocalPlayer) return;

            Vector3 dir = Vector3.zero;

            if (Input.GetKey(KeyCode.UpArrow)) dir = Vector3.up;
            else if (Input.GetKey(KeyCode.DownArrow)) dir = Vector3.down;
            else if (Input.GetKey(KeyCode.LeftArrow)) dir = Vector3.left;
            else if (Input.GetKey(KeyCode.RightArrow)) dir = Vector3.right;

            transform.position += dir * Time.deltaTime * 5;

            SetRemotePosServerRPC(transform.position);
        }

        [ServerRpc]
        void SetRemotePosServerRPC(Vector3 pos)
        {
            transform.position = pos;
        }
    }
}