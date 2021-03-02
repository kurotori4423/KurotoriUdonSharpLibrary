
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// ドロップ後一定時間でオブジェクトの位置がリセットされるPickup
/// </summary>

public class TimerRelocationPickup : UdonSharpBehaviour
{
    [SerializeField]
    bool isGlobalSync = true; // 同期を行うかどうか

    [SerializeField]
    float resetSeconds = 60.0f; // リセット時間[s]

    [SerializeField]
    float respawnHeight = -100.0f; // リスポーン処理のためVRCWorldで設定している最低値を入れる。
    
    bool isPickUp = false;
    float nowTime;
    Vector3 initPosition;
    Quaternion initRotation;
    Rigidbody rigidBody;


    void Start()
    {
        initPosition = transform.position;
        initRotation = transform.rotation;

        nowTime = resetSeconds;

        rigidBody = gameObject.GetComponent<Rigidbody>();

        if(rigidBody == null)
        {
            Debug.LogError(string.Format("{0} has not RigidBody", gameObject.name));
        }
    }

    public override void OnPickup()
    {
        isPickUp = true;
        nowTime = resetSeconds;
    }

    public override void OnDrop()
    {
        isPickUp = false;
    }

    private void TimerCount()
    {
        if (!isPickUp)
        {
            nowTime -= Time.deltaTime;

            if (nowTime < 0.0f)
            {
                if(isGlobalSync)
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetTransform");
                    ResetTransform();
                }
                else
                {
                    ResetTransform();
                }

                nowTime = resetSeconds;
            }
        }
    }

    public void ResetTransform()
    {
        if (isGlobalSync)
        {
            // グローバル同期設定の場合はRespawnHeightでリスポーンする方法を使用する。
            transform.position = new Vector3(transform.position.x, respawnHeight - 10.0f, transform.position.z);
        }
        else
        {
            // ローカルオブジェクトはそのまま場所を元に戻す。
            transform.position = initPosition;
            transform.rotation = initRotation;
        }

        if (rigidBody != null)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }

    private void Update()
    {
        if(isGlobalSync)
        {
            // オーナーだけでカウントダウン処理をしたい。
            if(Networking.IsOwner(gameObject))
            {
                TimerCount();
            }
        }
        else
        {
            TimerCount();
        }
    }
}
