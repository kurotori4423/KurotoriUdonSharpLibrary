
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SimpleToggleLowHighMirror : UdonSharpBehaviour
{

    bool hightMirror = false;
    bool lowMirror = false;

    [SerializeField]
    Animator mirrorAnimator;
    [SerializeField]
    Animator LQButtonAnimator;
    [SerializeField]
    Animator HQButtonAnimator;

    void Start()
    {

    }

    private void ChangeState()
    {
        mirrorAnimator.SetBool("LQMirror", lowMirror);
        mirrorAnimator.SetBool("HQMirror", hightMirror);

        LQButtonAnimator.SetBool("LQMirror", lowMirror);
        LQButtonAnimator.SetBool("HQMirror", hightMirror);

        HQButtonAnimator.SetBool("LQMirror", lowMirror);
        HQButtonAnimator.SetBool("HQMirror", hightMirror);
    }

    public void HighMirrorToggle()
    {
        if(hightMirror)
        {
            hightMirror = false;
        }
        else
        {
            hightMirror = true;

            if (lowMirror)
            {
                lowMirror = false;
            }
        }

        ChangeState();
    }

    public void LowMirrorToggle()
    {
        if(lowMirror)
        {
            lowMirror = false;
        }
        else
        {
            lowMirror = true;

            if (hightMirror)
            {
                hightMirror = false;
            }
        }

        ChangeState();
    }
}
