using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void OnEnter() { Debug.Log("Entering Crouch State"); }

    public override void OnUpdate()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        player.Move(moveDirection, player.playerAttributes.crouchSpeed);

        if (Input.GetKeyUp(KeyCode.C))
            stateMachine.SetState(new WalkState(player, stateMachine));
    }

    public override void OnExit() { Debug.Log("Exiting Crouch State"); }
}
