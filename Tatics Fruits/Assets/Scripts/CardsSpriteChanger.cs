using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Threading.Tasks;

public class CardsSpriteChanger : MonoBehaviour
{
    public Image _targetImage;
    public List<Sprite> _sprites = new List<Sprite>();
    public float _timeBetweenSwitches;
    public bool _randomize;
    public int _imageInt;

    private async void Start()
    {
        await SpriteChangeLoop();
    }

    private void Update()
    {
        _targetImage.sprite = _sprites[_imageInt];
    }

    public async Task SpriteChangeLoop()
    {
        while (true)
        {
            await Task.Delay(System.TimeSpan.FromSeconds(_timeBetweenSwitches));

            if (_randomize)
            {
                _imageInt = Random.Range(0, _sprites.Count);
            }
            else
            {
                _imageInt = (_imageInt + 1) % _sprites.Count;
            }
        }
    }
}