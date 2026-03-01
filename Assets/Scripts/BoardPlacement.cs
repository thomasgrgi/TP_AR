using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class BoardPlacement : MonoBehaviour
{
    [SerializeField] GameObject gameBoardPrefab;
    [SerializeField] bool hidePlanesOnPlacement = false;
    [SerializeField] bool showLogs = false;

    GameObject gameBoardInstance;
    ARRaycastManager raycastManager;
    ARPlaneManager arPlaneManager;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        if (showLogs)
            Debug.Log("BoardPlacement Awake – raycastManager=" + raycastManager + " arPlaneManager=" + arPlaneManager);
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        if (showLogs)
            Debug.Log("EnhancedTouchSupport enabled");
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if (raycastManager == null)
        {
            if (showLogs)
                Debug.LogWarning("BoardPlacement: no ARRaycastManager on the same object");
            return;
        }

        var touches = Touch.activeTouches;
        if (showLogs)
            Debug.Log("BoardPlacement.Update touches.Count=" + touches.Count);

        if (touches.Count == 0)
            return;

        var first = touches[0];
        if (showLogs)
            Debug.Log($"first touch phase={first.phase} pos={first.screenPosition}");

        if (first.phase != UnityEngine.InputSystem.TouchPhase.Began)
            return;

        if (raycastManager.Raycast(first.screenPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            if (showLogs)
                Debug.Log("raycast hit " + s_Hits[0].pose);
            var hitPose = s_Hits[0].pose;

            if (gameBoardInstance == null)
            {
                if (gameBoardPrefab == null)
                {
                    if (showLogs)
                        Debug.LogWarning("BoardPlacement: GameBoard prefab is not assigned.");
                    return;
                }

                gameBoardInstance = Instantiate(gameBoardPrefab, hitPose.position, hitPose.rotation);

                if (hidePlanesOnPlacement)
                    HideAllPlanes();
            }
            else
            {
                gameBoardInstance.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }
        }
        else
        {
            if (showLogs)
                Debug.Log("raycast did not hit any plane");
        }
    }

    void HideAllPlanes()
    {
        if (arPlaneManager == null) return;
        var planes = new List<ARPlane>();
        foreach (var plane in arPlaneManager.trackables)
            planes.Add(plane);

        foreach (var p in planes)
            if (p != null && p.gameObject != null)
                Destroy(p.gameObject);

        arPlaneManager.planePrefab = null;
        arPlaneManager.enabled = false;
    }
}
