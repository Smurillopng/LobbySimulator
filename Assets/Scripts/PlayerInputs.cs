using Photon.Pun;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviourPunCallbacks
{
    private PlayerControls _playerActions;
    public InputAction dropAction, danceAction, waveAction , pourAction;

    public override void OnEnable()
    {
        _playerActions = new PlayerControls();
        dropAction = _playerActions.Player.Drop;
        danceAction = _playerActions.Player.Dance;
        waveAction = _playerActions.Player.Wave;
        pourAction = _playerActions.Player.Pour;

        _playerActions.Enable();
    }

    public override void OnDisable()
    {
        dropAction.Disable();
        danceAction.Disable();
        waveAction.Disable();
        pourAction.Disable();
        _playerActions.Disable();
    }
}