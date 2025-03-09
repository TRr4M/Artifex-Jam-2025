using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    void Start()
    {
        List<Transform> children = new();
        foreach (Transform child in transform) {
            children.Add(child);
            child.gameObject.SetActive(false);
        }
        int idx = Random.Range(0, children.Count);
        children[idx].gameObject.SetActive(true);
    }
}
