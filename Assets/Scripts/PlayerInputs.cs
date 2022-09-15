/*
 * Created by: Sérgio Murillo da Costa Faria
 */

using Photon.Pun;
using UnityEngine.InputSystem;

// Esse script é responsável pelos inputs do jogador
// só ativa e desativa o Input System e atribui os valores dos inputs
public class PlayerInputs : MonoBehaviourPunCallbacks
{
    #region Variáveis Privadas
    
    private PlayerControls _playerActions;
    public InputAction dropAction, danceAction, waveAction , pourAction;
    
    #endregion

    #region Métodos

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

    #endregion
}