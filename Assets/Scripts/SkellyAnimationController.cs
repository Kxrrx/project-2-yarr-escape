using UnityEngine.Events;
using UnityEngine;

public class SkellyAnimationController : MonoBehaviour
{
    public UnityEvent skellyHitsPlayer = new UnityEvent();
    public void Hit()
    {
        skellyHitsPlayer.Invoke();
        AudioManager.singleton.Play("SkellyAttack");
        Debug.Log("SkellyHitsPlayer");
    }
}
