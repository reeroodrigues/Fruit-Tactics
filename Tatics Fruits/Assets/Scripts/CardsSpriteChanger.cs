using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class CardsSpriteChanger : MonoBehaviour
{
    public Image _targetImage;
    public List<Sprite> _sprites = new List<Sprite>();
    public float _timeBetweenSwitches;
    public bool _randomize;
    public int _imageInt;

    private void Start()
    {
        StartCoroutine(SpriteChangeLoop());
    }

    private void Update()
    {
        _targetImage.sprite = _sprites[_imageInt];
    }

    IEnumerator SpriteChangeLoop()
    {
        yield return new WaitForSeconds(_timeBetweenSwitches);
        if (_randomize)
        {
            int targetImage = Random.Range(0, _sprites.Count - 1);
            _imageInt = targetImage;
        }
        else
        {
            if (_imageInt == _sprites.Count - 1)
            {
                _imageInt = 0;
            }
            else
            {
                _imageInt += 1;
            }
        }

        StartCoroutine(SpriteChangeLoop());
    }
}
