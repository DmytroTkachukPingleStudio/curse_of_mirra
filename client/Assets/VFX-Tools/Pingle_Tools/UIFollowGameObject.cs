using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowGameObject : MonoBehaviour
{
    [SerializeField] private Transform following_root = null;
    [SerializeField] private float var1 = 1.0f;
    [SerializeField] private float var2 = 1.0f;

    private Transform local_following_root = null;
    private IEnumerator following_coroutine = null;
    void Start()
    {
        startFollow(following_root);
    }

    public void stopFollow()
    {
        if(following_coroutine != null)
            StopCoroutine(following_coroutine);
    }

    public void startFollow(Transform root_to_follow)
    {
        stopFollow();

        following_coroutine = impl();
        StartCoroutine(following_coroutine);

        IEnumerator impl()
        {
            Vector3 screen_pos = Vector3.zero;
            
            while(true)
            {
                yield return null;

                screen_pos = Camera.main.WorldToScreenPoint( root_to_follow.position );
                screen_pos.z = 0;
                transform.position = screen_pos;

                float distance = Vector3.Distance( Camera.main.transform.position, root_to_follow.position );

                distance = distance - var1;
                distance = distance / var2;
                distance = 1 - distance;
                distance = distance * 2;

                float scale = distance;

                Vector3 local_scale = new Vector3( scale, scale, 1.0f );
                transform.localScale = local_scale;
            }
        }
    }

}
