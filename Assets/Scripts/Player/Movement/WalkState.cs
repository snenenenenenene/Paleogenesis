using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void OnEnter()
    {
        player.SetSpeed(player.playerAttributes.walkSpeed);
        Debug.Log("Entering Walk State");
    }

    public override void OnUpdate()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        player.ApplyMovement(moveDirection, player.playerAttributes.walkSpeed);

        if (Input.GetKey(KeyCode.LeftShift) && player.HasStamina())
            stateMachine.SetState(new RunState(player, stateMachine));
        else if (Input.GetKey(KeyCode.C))
            stateMachine.SetState(new CrouchState(player, stateMachine));
    }

    public override void OnExit()
    {
        Debug.Log("Exiting Walk State");
    }
}
