using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void OnEnter() { Debug.Log("Entering Run State"); }

    public override void OnUpdate()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        player.Move(moveDirection, player.playerAttributes.runSpeed);

        if (!Input.GetKey(KeyCode.LeftShift) || !player.HasStamina())
            stateMachine.SetState(new WalkState(player, stateMachine));
    }

    public override void OnExit() { Debug.Log("Exiting Run State"); }
}
