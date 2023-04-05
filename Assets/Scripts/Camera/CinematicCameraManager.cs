using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CinematicCameraManager : MonoBehaviour
{
    CinemachineDollyCart cart;

    public static CinematicCameraManager instance;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    //Cinemachine Paths are dolly tracks for the camera. This is used during the turn selection mode for a "news helicopter" view of the battle;
    [SerializeField] CinemachineSmoothPath startPath;
    [SerializeField] CinemachineSmoothPath[] paths;

    [SerializeField] CinemachineSmoothPath playerFocusPath;


    [SerializeField] Transform sceneCenter; //An empty game object placed at the center of the scene, this is where the camera will focus on during the "move selection" phase of the player's turn

    public enum CameraMode
    {
        STATIC, //Camera Mode is switched to STATIC when focusing on a specific object, IE the crab when the crab attacks, or the army when the player has selected their move
        DYNAMIC //Camera Mode is switched to DYNAMIC when playing the cinematic scene during the players turn (move selection).
    }

    [SerializeField] CameraMode cameraMode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        cart = GetComponent<CinemachineDollyCart>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        Reset();
    }

    //NOTE: FOR MATT. After the attack scene of either side is over, you need to call this function.
    public void Reset()
    {
        StopAllCoroutines(); //End all camera cinematics
        cart.m_Path = startPath; //Reset camera to starting path
        virtualCamera.LookAt = sceneCenter; //Focus camera on scene center

        switch (cameraMode)
        {
            case CameraMode.STATIC:
                break;

            case CameraMode.DYNAMIC:
                StartCoroutine(ChangeTrack());
                break;

            default:
                Debug.LogWarning("We should never get here! (Invalid CameraMode)");
                break;
        }
    }

    //NOTE: FOR MATT. You also need to call this one and pass in CameraMode.DYNAMIC
    public void ChangeCameraMode(CameraMode cM)
    {
        cameraMode = cM;
    }

    IEnumerator ChangeTrack() //Changes the camera path to a random one for 6-8 seconds
    {
        yield return new WaitForSeconds(Random.Range(6, 8));
        var path = paths[Random.Range(0, paths.Length)];
        cart.m_Path = path;

        StartCoroutine(ChangeTrack());
    }

    IEnumerator PlayerFocus()
    {
        var path = playerFocusPath;
        cart.m_Path = path;
        StartCoroutine(ChangeTrack());
        yield return new WaitForSeconds(3f); //TODO: TALK TO MATT, THIS IS JUST TEMPORARY
    }

    public void CameraStaticFocus(Transform focusObj)
    {
        StopAllCoroutines();


    }

    


}
