
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Kurotori
{
    /// <summary>
    /// SendCustomEventで位置がリセットできるPickup
    /// </summary>

    public class RelocatablePickup : UdonSharpBehaviour
    {
        [SerializeField]
        bool isGlobalSync = true; // 同期を行うかどうか

        [SerializeField]
        float respawnHeight = -100.0f; // リスポーン処理のためVRCWorldで設定している最低値を入れる。

        Vector3 initPosition;
        Quaternion initRotation;
        Rigidbody rigidBody;


        void Start()
        {
            initPosition = transform.position;
            initRotation = transform.rotation;

            rigidBody = gameObject.GetComponent<Rigidbody>();

            if (rigidBody == null)
            {
                Debug.LogError(string.Format("{0} has not RigidBody", gameObject.name));
            }
        }

        public void Relocate()
        {
            if (isGlobalSync)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "RelocateGlobal");
                RelocateLocal();
            }
            else
            {
                RelocateLocal();
            }
        }

        public void RelocateGlobal()
        {
            // グローバル同期設定の場合はRespawnHeightでリスポーンする方法を使用する。
            transform.position = new Vector3(transform.position.x, respawnHeight - 10.0f, transform.position.z);

            if (rigidBody != null)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
        }

        private void RelocateLocal()
        {
            // ローカルオブジェクトはそのまま場所を元に戻す。
            transform.position = initPosition;
            transform.rotation = initRotation;

            if (rigidBody != null)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
        }
    }
}
