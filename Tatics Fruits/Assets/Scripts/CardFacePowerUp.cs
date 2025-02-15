using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardFacePowerUp : MonoBehaviour
{
    [Header("References")] 
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
            if (Camera.main != null)
            {
                var localPos = Camera.main.transform.InverseTransformPoint(transform.position) -
                               Camera.main.transform.InverseTransformPoint(_target.transform.position);
                _movement = Vector3.Lerp(_movement, localPos, 10 * Time.deltaTime);
            }

            var movementRotation = _movement;
            _rotation = Vector3.Lerp(_rotation, movementRotation, _rotationSpeed * Time.deltaTime);

            var clampedRotation = Mathf.Clamp(movementRotation.x, -_rotationAmount, _rotationAmount);
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
        if (_target == null)
        {
            return;
        }

        var cardComponent = _target.GetComponent<Card>();
        if (cardComponent == null)
        {
            return;
        }

        // Verifica se o CardPowerUpTypeSo está definido
        if (cardComponent._cardPowerUpTypeSo == null)
        {
            return;
        }

        // Certifica-se de que as referências do ícone e números estão configuradas
        if (_icon == null || _iconShadow == null || _rightNumber == null || _leftNumber == null)
        {
            return;
        }

        // Atualiza o ícone do power-up e os números (se necessário)
        _icon.sprite = cardComponent._cardPowerUpTypeSo._cardIcon;
        _iconShadow.sprite = cardComponent._cardPowerUpTypeSo._cardIcon;

        // Aqui você pode adicionar lógica para números se os power-ups tiverem números
        // _rightNumber.text = cardComponent._cardPowerUpTypeSo._cardNumber.ToString();
        // _leftNumber.text = cardComponent._cardPowerUpTypeSo._cardNumber.ToString();
    }
}
