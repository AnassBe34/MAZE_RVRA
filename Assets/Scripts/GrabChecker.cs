using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabChecker : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    [Header("Références WFC")]
    public OverlapWFC wfc;
    public Training training2;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // On s'abonne aux events XR Interaction
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("Element grabbed ! Changement de training...");

        if (wfc != null && training2 != null)
        {
            wfc.SetTraining(training2);
            wfc.Generate();
        }
        else
        {
            Debug.LogWarning("WFC ou Training2 non assigné !");
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("Element relâchée");
    }
}