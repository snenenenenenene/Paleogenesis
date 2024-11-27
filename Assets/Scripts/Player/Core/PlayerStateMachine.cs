public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }
    private PlayerController player;

    public PlayerStateMachine(PlayerController player)
    {
        this.player = player;
        SetState(new WalkState(player, this));
    }

    public void SetState(PlayerState newState)
    {
        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }

    public void UpdateState()
    {
        CurrentState?.OnUpdate();
    }
}
