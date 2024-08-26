using System;
using System.Collections;
using UnityEngine;

public class FreezeScript : MonoBehaviour {
    void Start() {
        transform.localScale = new Vector3(2 * PowerupConstants.Freeze.radius, 2 * PowerupConstants.Freeze.radius, 1f);
    }

    void Awake() {
        StartCoroutine(Disappaer());
    }

    private IEnumerator Disappaer() {
        yield return new WaitForSeconds(PowerupConstants.Freeze.duration);
        Destroy(gameObject);
    }
}