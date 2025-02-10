using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardFace : MonoBehaviour
{
    [Header("Refences")] 
    public GameObject _target;
    public GameObject _visual;
    public Image _icon;
    public Image _iconShadow;
    public TextMeshProUGUI _rightNumber;
    public TextMeshProUGUI _leftNumber;

    [Header("Settings")]
    public float _rotationSpeed;
    public float _movementSpeed;
    public float _rotationAmount;
    public Vector3 _offset;

    private Vector3 _rotation;
    private Vector3 _movement;
    private float _randomRot;
    private bool _hovering;

    private void Start()
    {
        _randomRot = Random.Range(-_rotationAmount, _rotationAmount);
    }

    private void Update()
    {
        if (_target == null)
            return;
        
        _hovering = _target.GetComponent<Card>()._hovering && _target.GetComponent<Card>()._cardState != Card.CardState.Played;
        
        transform.position = Vector3.Lerp(transform.position, _target.transform.position, Time.deltaTime * _movementSpeed);
        _visual.transform.position = Vector3.Lerp(_visual.transform.position, (_hovering || _target.GetComponent<Card>()._cardManager._selectedCard == _target) ? _target.transform.position + _offset : _target.transform.position, Time.deltaTime * _movementSpeed);

        if (_target.GetComponent<Card>()._cardState != Card.CardState.Played)
        {
            Vector3 localPos = Camera.main.transform.InverseTransformPoint(transform.position) -
                               Camera.main.transform.InverseTransformPoint(_target.transform.position);
            _movement = Vector3.Lerp(_movement, localPos, 10 * Time.deltaTime);

            Vector3 movementRotation = _movement;
            _rotation = Vector3.Lerp(_rotation, movementRotation, _rotationSpeed * Time.deltaTime);

            float clampedRotation = Mathf.Clamp(movementRotation.x, -_rotationAmount, _rotationAmount);
            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, clampedRotation);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _randomRot);
        }

        UpdateCardInfo();
    }

    public void UpdateCardInfo()
    {
        CardType info = _target.GetComponent<Card>()._cardType;

        _icon.sprite = info._cardIcon;
        _iconShadow.sprite = info._cardIcon;
        _rightNumber.text = _target.GetComponent<Card>()._cardNumber.ToString();
        _leftNumber.text = _target.GetComponent<Card>()._cardNumber.ToString();
    }
}
