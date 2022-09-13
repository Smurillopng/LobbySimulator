using System.Collections;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Photon.Pun;
using Photon.Realtime;
using Random = System.Random;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private CubismModel player;
    [SerializeField] private CubismParameter playerColor, playerItem, filled;
    [SerializeField] private string colorID ="ParamDrinkColor";
    [SerializeField] private string itemID = "ParamItem";
    [SerializeField] private string glassEmptiness = "ParamGlassEmptiness";
    [SerializeField] private int filledValue = 10;
    
    private Animator _animator;
    private PlayerInputs _playerInputs;
    private bool _isRefilling, _isFull, _isPlayingAnim;
    
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
        RandomizeColor();
        RandomizeItem();

        filled.Value = filledValue;
    }

    private void Update() => Check();
    
    private void LateUpdate()
    {
        switch (filled.Value)
        {
            case >= 9.8f:
                _isFull = true;
                _isRefilling = false;
                break;
            case <= 1f:
                _isFull = false;
                _isRefilling = true;
                break;
        }

        if (_isFull && _isPlayingAnim)
        {
            EmptyGlass();
        }

        if (!_isFull)
        {
            FillGlass();
        }

        /*
        switch (_isRefilling)
        {
            case true:
                _animator.SetBool(Pour, true);
                break;
            case false:
                _animator.SetBool(Pour, false);
                break;
        }
        */
    }
    
    private void RandomizeColor()
    {
        var random = new Random();
        playerColor.Value = random.Next(0, 21);
    }
    private void RandomizeItem()
    {
        var random = new Random();
        playerItem.Value = random.Next(0, 4);
    }
    
    [PunRPC]
    public float GetColor()
    {
        return playerColor.Value;
    }
    
    private void EmptyGlass() => filled.Value -= Time.deltaTime;
    private void FillGlass() => filled.BlendToValue(CubismParameterBlendMode.Override, 10, Time.deltaTime);

    private void Check()
    {
        if (_playerInputs.dropAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            StartCoroutine(PlayDrop());
        }
        if (_playerInputs.danceAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            StartCoroutine(PlayDance());
        }
        if (_playerInputs.waveAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            StartCoroutine(PlayWave());
        }

        if (_playerInputs.pourAction.WasPressedThisFrame() && !_isPlayingAnim)
        {
            _animator.SetBool(Pour, true);
            _isPlayingAnim = true;
        }
        if (_playerInputs.pourAction.IsPressed())
        {
            FillGlass();
        }
        if (_playerInputs.pourAction.WasReleasedThisFrame())
        {
            _animator.SetBool(Pour, false);
            _isPlayingAnim = false;
        }
    }
    
    private IEnumerator PlayDrop()
    {
        _isPlayingAnim = true;
        _animator.SetTrigger(Drop);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _isPlayingAnim = false;
    }
    
    private IEnumerator PlayDance()
    {
        _isPlayingAnim = true;
        _animator.SetTrigger(Dance);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _isPlayingAnim = false;
    }
    
    private IEnumerator PlayWave()
    {
        _isPlayingAnim = true;
        _animator.SetTrigger(Wave);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _isPlayingAnim = false;
    }
}