using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tap : MonoBehaviour
{
    bool isHold = false;
    Vector2 beginPosition;
    Vector2 rootPosition;


    public Vector2 DeltaPosition
    {
        get; set;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor && Input.GetMouseButtonUp(0))
        {
            isHold = false;
            beginPosition = Vector2.zero;
            DeltaPosition = Vector3.zero;
        }
        if (!Application.isEditor && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled))
        {
            isHold = false;
            beginPosition = Vector2.zero;
            DeltaPosition = Vector3.zero;
        }
    }

    public Vector2 TapPosition
    {
        get
        {
            if (!Application.isEditor && Input.touchCount > 0)
                return Input.GetTouch(0).position;
            return Input.mousePosition;
        }
    }
    public Vector3 WorldPosition
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        }
    }

    public bool DetectTap
    {
        get
        {
            return Input.touchCount > 0 || Application.isEditor;
        }
    }
    public Vector2 deltaFromBegin
    {
        get {
            if ((Application.isEditor && isHold) || (!Application.isEditor && Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                //Debug.Log("World " + WorldPosition.x + ";" + WorldPosition.y + "  Root " + rootPosition.x + ";" + rootPosition.y);
                return (Vector2)WorldPosition - rootPosition;
            }
            return Vector2.zero;
        }
    }
    public bool TapBegin()
    {
        if (Application.isEditor && Input.GetMouseButtonDown(0))
        {
            isHold = true;
            beginPosition = WorldPosition;
            rootPosition = WorldPosition;
            return true;
        }
        if (!Application.isEditor && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isHold = true;
            beginPosition = WorldPosition;
            rootPosition = WorldPosition;
            return true;
        }
        return false;
    }
    public bool TapMove()
    {
        if (Application.isEditor && isHold)
        {
            DeltaPosition = (Vector2)WorldPosition - beginPosition;
            beginPosition = WorldPosition;
            return true;
        }
        if (!Application.isEditor && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            DeltaPosition = (Vector2)WorldPosition - beginPosition; //Input.GetTouch(0).deltaPosition;
            beginPosition = WorldPosition;

            return true;
        }
        return false;
    }
    public bool TapEnd()
    {
        if (Application.isEditor && Input.GetMouseButtonUp(0))
        {
            isHold = false;
            beginPosition = Vector2.zero;
            DeltaPosition = Vector3.zero;
            return true;
        }
        if (!Application.isEditor && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled))
        {
            isHold = false;
            beginPosition = Vector2.zero;
            DeltaPosition = Vector3.zero;
            return true;
        }
        return false;
    }
}
