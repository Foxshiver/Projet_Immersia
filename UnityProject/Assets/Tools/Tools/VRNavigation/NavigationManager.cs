using UnityEngine;
using System.Collections;

public enum NavigationMode 
{
    CharacterController,
    NavMesh,
    Fly,
    None
}

/// <summary>
/// Change navigation mode on runtime.
/// </summary>
public class NavigationManager : MonoBehaviour 
{
    public NavigationMode navigationMode = NavigationMode.CharacterController;

    public KeyCode changeNavigationModeKey = KeyCode.N;
    public KeyCode modifierChangeNavigationModeKey = KeyCode.None;

    public JoystickNavigationController joystickNavigationController;

    public CharacterController characterController;
    public NavMeshAgent navMeshAgent;

    public GameObject camera;

    int navigationModeNumber;

    void Reset()
    {
        characterController = GetComponentInChildren<CharacterController>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        joystickNavigationController = GetComponent<JoystickNavigationController>();
        camera = GetComponentInChildren<Camera>().gameObject;
    }

    void OnEnable()
    {
        navigationModeNumber = System.Enum.GetNames(typeof(NavigationMode)).Length;
        changeMode(navigationMode);
    }
	
	void OnValidate() 
    {
        if (Application.isPlaying)
            changeMode(navigationMode);
	}

    void Update()
    {
        if(VRTools.GetKeyDown(changeNavigationModeKey) && (modifierChangeNavigationModeKey == KeyCode.None || VRTools.GetKeyPressed(modifierChangeNavigationModeKey)))
        {
            int newMode = (int) navigationMode;
            newMode = (newMode + 1) % navigationModeNumber;
            navigationMode = (NavigationMode)newMode;
            changeMode(navigationMode);
        }

    }

    void changeMode(NavigationMode mode)
    {
        Debug.Log("[VRNavigation] Change navigation mode to " + mode.ToString());

        switch(mode)
        {
            case NavigationMode.CharacterController :
                characterController.enabled = true;
                joystickNavigationController.directTranslateMode = false;
                joystickNavigationController.fixedHeight = true;
                navMeshAgent.enabled = false;
                break;

            case NavigationMode.Fly :
                characterController.enabled = false;
                joystickNavigationController.directTranslateMode = true;
                joystickNavigationController.fixedHeight = false;
                navMeshAgent.enabled = false;
                break;

            case NavigationMode.NavMesh:
                characterController.enabled = false;
                joystickNavigationController.directTranslateMode = true;
                joystickNavigationController.fixedHeight = true;
                NavMeshHit navMeshHit;
                NavMesh.SamplePosition(characterController.transform.position, out navMeshHit, 2000, NavMesh.AllAreas);
                //Add same offset to the camera to stay sync.
                camera.transform.position += navMeshHit.position - characterController.transform.position;
                characterController.transform.position = navMeshHit.position;
                navMeshAgent.enabled = true;
                break;

            case NavigationMode.None:
                characterController.enabled = false;
                joystickNavigationController.directTranslateMode = false;
                joystickNavigationController.fixedHeight = false;
                characterController.enabled = false;
                navMeshAgent.enabled = false;
                break;
        }
    }
}
