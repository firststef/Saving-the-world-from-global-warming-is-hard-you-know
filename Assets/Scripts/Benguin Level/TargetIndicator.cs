using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    public Transform Target;
    public float HideDistance;
    private Vector3 dir;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (dir.magnitude < HideDistance)
            SetChildrenActive(false);
        else
            SetChildrenActive(true);
        if (Target != null)
        {
            dir = Target.position - transform.parent.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetChildrenActive(bool value)
    {
        foreach(Transform child in this.transform)
        {
            child.gameObject.SetActive(value);
        }
    }

}
