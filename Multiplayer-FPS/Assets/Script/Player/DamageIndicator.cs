using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public Vector3 damageLocation;
    [SerializeField] private Transform playerObject;
    [SerializeField] private Transform damageImagePivot;

    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeStartTim, fadeTime;
    private float maxFadeTime;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        maxFadeTime = fadeTime;
    }

    private void Update()
    {
        if (fadeStartTim > 0)
        {
            fadeStartTim -= Time.deltaTime;
        }
        else
        {
            fadeTime -= Time.deltaTime;
            canvasGroup.alpha = fadeTime / maxFadeTime;

            if(fadeTime <= 0)
            {
                Destroy(gameObject);
            }
        }

        damageLocation.y = playerObject.position.y;

        Vector3 Direction = (damageLocation - playerObject.position).normalized;
        float angle = (Vector3.SignedAngle(Direction, -playerObject.forward, Vector3.up));

        damageImagePivot.localEulerAngles = new Vector3(0, 0, angle);
    }
}
