using UnityEngine;
using Live2D.Cubism.Core;
using Photon.Pun;
using Photon.Realtime;
using Utils;
using Random = System.Random;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private CubismModel player;
    [SerializeField] private CubismParameter playerColor, playerItem, filled;
    [SerializeField] private string colorID ="ParamDrinkColor";
    [SerializeField] private string itemID = "ParamItem";
    [SerializeField] private string glassEmptiness = "ParamGlassEmptiness";
    [SerializeField] private float filledValue = 10;
    [SerializeField] private float percentage;

    private Animator _animator;
    private PlayerInputs _playerInputs;
    private bool _isRefilling, _isFull, _isPlayingAnim;
    private float _timer, _playerColorValue, _playerItemValue, _filledValue;

    private static readonly int Drop = Animator.StringToHash("Drop");
    private static readonly int Dance = Animator.StringToHash("Dance");
    private static readonly int Wave = Animator.StringToHash("Wave");
    private static readonly int Pour = Animator.StringToHash("Pour");

    private void Awake()
    {
        player = this.FindCubismModel();
        playerColor = player.Parameters.FindById(colorID);
        playerItem = player.Parameters.FindById(itemID);
        filled = player.Parameters.FindById(glassEmptiness);
        _animator = GetComponent<Animator>();
        _playerInputs = GetComponent<PlayerInputs>();
    }
    
    private void Start()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(RandomizeColor), RpcTarget.All);
            playerColor.Value = _playerColorValue;
            photonView.RPC(nameof(RandomizeItem), RpcTarget.All);
            playerItem.Value = _playerItemValue;
        }

        filled.Value = filledValue;
        _filledValue = filled.Value;
    }

    private void LateUpdate()
    {
        filled.Value = _filledValue;
        photonView.RPC(nameof(UpdateColorAndItem), RpcTarget.All);
        if (!photonView.IsMine) return;
        
        Check();

        if (_isPlayingAnim)
        {
            photonView.RPC(nameof(AnimationTimer), RpcTarget.All);
        }
        
        photonView.RPC(nameof(Emptying), RpcTarget.All);
    }
    
    [PunRPC]
    private void Emptying()
    {
        if (_isFull && _isPlayingAnim)
        {
            _filledValue -= Time.deltaTime * percentage / 2;
        }
        
        if (_filledValue <= 0)
        {
            _isFull = false;
        }

        if (_filledValue >= filledValue)
        {
            _isFull = true;
        }
    }

    [PunRPC]
    private void Filling()
    {
        _filledValue += Time.deltaTime * percentage;
    }
    
    [PunRPC]
    private void RandomizeColor()
    {
        var random = new Random();
        _playerColorValue = random.Next(0, 21);
    }
    [PunRPC]
    private void RandomizeItem()
    {
        var random = new Random();
        _playerItemValue = random.Next(0, 4);
    }
    [PunRPC]
    private void UpdateColorAndItem()
    {
        if (photonView.IsMine)
        {
            playerColor.Value = _playerColorValue;
            playerItem.Value = _playerItemValue;
        }
        else
        {
            playerColor.Value = _playerColorValue;
            playerItem.Value = _playerItemValue;
        }
    }

    private void Check()
    {
        if (_playerInputs.dropAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            photonView.RPC("PlayDrop", RpcTarget.All);
        }
        if (_playerInputs.danceAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            photonView.RPC("PlayDance", RpcTarget.All);
        }
        if (_playerInputs.waveAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            photonView.RPC("PlayWave", RpcTarget.All);
        }

        if (_playerInputs.pourAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            _animator.SetBool(Pour, true);
            _isPlayingAnim = true;
        }
        if (_playerInputs.pourAction.IsPressed() && _animator.GetBool(Pour).Equals(true))
        {
            photonView.RPC(nameof(Filling), RpcTarget.All);
        }
        if (_playerInputs.pourAction.WasReleasedThisFrame()  && photonView.IsMine)
        {
            _animator.SetBool(Pour, false);
            _isPlayingAnim = false;
        }
    }
    
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
    private void AnimationTimer()
    {
        _timer += Time.deltaTime;
        if (!(_timer >= _animator.GetCurrentAnimatorStateInfo(0).length)) return;
        _isPlayingAnim = false;
        _timer = 0;
    }

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
}