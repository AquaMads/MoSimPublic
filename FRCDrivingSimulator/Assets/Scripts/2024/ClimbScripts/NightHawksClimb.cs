using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NightHawksClimb : MonoBehaviour, IResettable
{
    [SerializeField] private ConfigurableJoint climber;

       private RobotAlignToSpeaker shooter;

    private int startingLayer;

    private bool climb;

    private bool hang;
    private bool raisedArm = false;
    private bool prepped = false;
    private bool isClimbing = false;

    private Vector3 climberStartingPos;
    private Quaternion climberStartingRot;

    private void Start() 
    {
        climberStartingPos = climber.gameObject.transform.localPosition;
        climberStartingRot = climber.gameObject.transform.localRotation;

        startingLayer = climber.gameObject.layer;

 
        shooter = GetComponent<RobotAlignToSpeaker>();
    }

    private void Update()
    {
        if (climb && !isClimbing)
        {
            isClimbing = true;
            StartCoroutine(ClimbSequence());
        }
        else if (climb && prepped && raisedArm)
        {
            prepped = false;
            HangSequence();
        }
    }


    private IEnumerator ClimbSequence() 
    {
        StartCoroutine(shooter.ShooterToPosition(Quaternion.Euler(40, 0, 0), false));
        climber.targetRotation = Quaternion.Euler(-100f, 0f, 0f);
        yield return new WaitForSeconds(0.5f);
        prepped = true;
    }

    private void HangSequence() 
    {
        //climber.targetRotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(shooter.ShooterToPosition(Quaternion.Euler(-60, 0, 0), false));
    }

    public void OnClimb(InputAction.CallbackContext ctx)
    {
        climb = ctx.action.triggered;
    }

    public void OnHang(InputAction.CallbackContext ctx)
    {
        hang= ctx.action.triggered;
        StartCoroutine(shooter.ShooterToPosition(Quaternion.Euler(-60, 0, 0), false));
    }
    private IEnumerator WaitToEnable() 
    {
        yield return new WaitForSeconds(0.01f);
        climber.gameObject.layer = startingLayer;
    }

    public void Reset() 
    {
        climber.gameObject.layer = 17;

        prepped = false;
        raisedArm = false;
        isClimbing = false;

        //Reset joints pos and rot and targetPos
        climber.gameObject.transform.localPosition = climberStartingPos;
        climber.gameObject.transform.localRotation = climberStartingRot;

        climber.targetRotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(WaitToEnable());
    }
}