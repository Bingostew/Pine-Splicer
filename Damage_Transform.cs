using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Damage_Transform : MonoBehaviour
{
    private Vector3 position;
    private Transform damageText;

    public void SetPosition(Vector3 targetPos, float text)
    {
        damageText = transform.GetChild(0);
        damageText.GetComponent<TextMeshProUGUI>().text = text.ToString();
        position = targetPos;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Instant_Reference.mainCamera.WorldToScreenPoint(position);
        Death();
    }


    public void Death()
    {
        if(!damageText.GetComponent<Animation>().isPlaying)
        Destroy(gameObject);
    }
}
