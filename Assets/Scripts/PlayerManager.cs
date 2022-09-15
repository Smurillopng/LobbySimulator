/*
 * Created by: Sérgio Murillo da Costa Faria
 */

using UnityEngine;
using Live2D.Cubism.Core;
using Photon.Pun;
using Utils;
using Random = System.Random;

// Esse script é responsável por controlar as animações do personagem,
// seus valores e por fazer a sincronização entre os players.
public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Variáveis do Inspector
    
    [Tooltip("Nome do parâmetro de animação de cor para o Lived2D")]
    [SerializeField] private string colorID ="ParamDrinkColor";
    [Tooltip("Nome do parâmetro do item para o Lived2D")]
    [SerializeField] private string itemID = "ParamItem";
    [Tooltip("Nome do parâmetro do quão cheio o copo ta para o Lived2D")]
    [SerializeField] private string glassEmptiness = "ParamGlassEmptiness";
    [Tooltip("O valor inicial do parâmetro de quão cheio o copo ta")]
    [SerializeField] private float filledValue = 10;
    [Tooltip("A porcentagem de quanto o copo vai diminuir a cada vez que o player tocar uma animação")]
    [SerializeField] private float percentage;

    #endregion

    #region Variáveis Privadas 

    private CubismModel _player; // O modelo do personagem [Live2d]
    private CubismParameter _playerColor, _playerItem, _filled; // Os parâmetros do modelo [Live2D]
    private Animator _animator;
    private PlayerInputs _playerInputs;
    private bool _isRefilling, _isFull, _isPlayingAnim;
    private float _timer, _playerColorValue, _playerItemValue, _filledValue;

    // Essas variáveis guardar referências as animações do player
    private static readonly int Drop = Animator.StringToHash("Drop");
    private static readonly int Dance = Animator.StringToHash("Dance");
    private static readonly int Wave = Animator.StringToHash("Wave");
    private static readonly int Pour = Animator.StringToHash("Pour");

    #endregion

    #region Métodos Unity

    private void Awake()
    {
        _player = this.FindCubismModel();
        _playerColor = _player.Parameters.FindById(colorID);
        _playerItem = _player.Parameters.FindById(itemID);
        _filled = _player.Parameters.FindById(glassEmptiness);
        _animator = GetComponent<Animator>();
        _playerInputs = GetComponent<PlayerInputs>();
    }
    
    private void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(RandomizeColor), RpcTarget.All);
            _playerColor.Value = _playerColorValue;
            photonView.RPC(nameof(RandomizeItem), RpcTarget.All);
            _playerItem.Value = _playerItemValue;
        }

        // Enche o copo no começo
        _filled.Value = filledValue;
        _filledValue = _filled.Value;
    }

    private void LateUpdate()
    {
        _filled.Value = _filledValue; // Atualiza o valor do parâmetro de quão cheio o copo ta
        photonView.RPC(nameof(UpdateColorAndItem), RpcTarget.All);
        if (!photonView.IsMine) return;
        
        Check(); // Checa se as animações estão sendo acionadas ou se estão sendo executadas

        if (_isPlayingAnim)
        {
            photonView.RPC(nameof(AnimationTimer), RpcTarget.All);
        }
        
        photonView.RPC(nameof(Emptying), RpcTarget.All);
    }

    #endregion

    #region Métodos

    [PunRPC]
    private void Emptying() // Método responsável por esvaziar o copo
    {
        if (_isFull && _isPlayingAnim) // Começa a esvaziar se ele estiver cheio e tocando uma animação
        {
            _filledValue -= Time.deltaTime * percentage / 2;
        }
        
        if (_filledValue <= 0) // Checa se o copo está vazio
        {
            _isFull = false;
        }

        if (_filledValue >= filledValue) // Checa se o copo está cheio
        {
            _isFull = true;
        }
    }

    [PunRPC]
    private void Filling() => _filledValue += Time.deltaTime * percentage; // Enche o copo

    [PunRPC]
    private void RandomizeColor() // Escolhe uma cor aleatória para o player
    {
        var random = new Random();
        _playerColorValue = random.Next(1, 22);
    }
    [PunRPC]
    private void RandomizeItem() // Escolhe um item aleatório para o player
    {
        var random = new Random();
        _playerItemValue = random.Next(1, 5);
    }
    [PunRPC]
    private void UpdateColorAndItem() // Atualiza os valores dos parâmetros de cor e item
    {
        if (photonView.IsMine)
        {
            _playerColor.Value = _playerColorValue;
            _playerItem.Value = _playerItemValue;
        }
        else
        {
            _playerColor.Value = _playerColorValue;
            _playerItem.Value = _playerItemValue;
        }
    }

    private void Check()
    {
        // Animações de 1 a 3
        if (_playerInputs.dropAction.WasPressedThisFrame() && !_isPlayingAnim)
            photonView.RPC("PlayDrop", RpcTarget.All);
        if (_playerInputs.danceAction.WasPressedThisFrame() && !_isPlayingAnim)
            photonView.RPC("PlayDance", RpcTarget.All);
        if (_playerInputs.waveAction.WasPressedThisFrame() && !_isPlayingAnim)
            photonView.RPC("PlayWave", RpcTarget.All);

        // Animação do [espaço] que enche o copo
        if (_playerInputs.pourAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            _animator.SetBool(Pour, true);
            _isPlayingAnim = true;
        }
        if (_playerInputs.pourAction.IsPressed() && _animator.GetBool(Pour).Equals(true))
            photonView.RPC(nameof(Filling), RpcTarget.All);
        if (_playerInputs.pourAction.WasReleasedThisFrame()  && photonView.IsMine)
        {
            _animator.SetBool(Pour, false);
            _isPlayingAnim = false;
        }
    }

    #region Métodos responsáveis pelas animações

    [PunRPC]
    private void PlayDrop()
    {
        _isPlayingAnim = true;
        _animator.SetTrigger(Drop);
    }
    
    [PunRPC]
    private void PlayDance()
    {
        _isPlayingAnim = true;
        _animator.SetTrigger(Dance);
    }
    
    [PunRPC]
    private void PlayWave()
    {
        _isPlayingAnim = true;
        _animator.SetTrigger(Wave);
    }

    [PunRPC]
    private void AnimationTimer() // Verifica se a animação acabou de ser executada
    {
        _timer += Time.deltaTime;
        if (!(_timer >= _animator.GetCurrentAnimatorStateInfo(0).length)) return;
        _isPlayingAnim = false;
        _timer = 0;
    }

    #endregion
    

    // Método que envia e recebe alguns dados do player para sincronizar com os outros players
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && photonView.IsMine)
        {
            stream.SendNext(_filledValue);
            stream.SendNext(_playerColorValue);
            stream.SendNext(_playerItemValue);
            this.Log($"is sending {_playerColorValue}");
        }
        else
        {
            _filledValue = (float) stream.ReceiveNext();
            _playerColorValue = (float) stream.ReceiveNext();
            _playerItemValue = (float) stream.ReceiveNext();
            this.Log($"is receiving {_playerColorValue}");
        }
    }

    #endregion
}