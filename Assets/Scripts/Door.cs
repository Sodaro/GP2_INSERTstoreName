using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool _moveDoor;
    [SerializeField] private bool _rotateDoor;
    [SerializeField] private Vector3 _openPosition;
    [SerializeField] private Quaternion _openRotation;

    private bool _isOpen = false;

    private Vector3 _closedPosition;

    private Coroutine _rotationRoutine;
    private Coroutine _moveRoutine;

    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        _closedPosition = transform.localPosition;
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        PlaySound(0);
        float f = 0;


        Vector3 startPos = transform.localPosition;

        float time = 1f;

        while (f < time)
        {
            f += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, targetPosition, f / time);
            yield return null;
        }
        transform.localPosition = targetPosition;
        _moveRoutine = null;

    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        float f = 0;
        

        Quaternion startRot = transform.localRotation;

        float angle = Quaternion.Angle(startRot, targetRotation);
        float time = angle / 90;

        while (f < time)
        {
            f += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startRot, targetRotation, f / time);
            yield return null;
        }
        transform.localRotation = targetRotation;
        _rotationRoutine = null;


        yield return new WaitForSeconds(0.08f);
        if(transform.localRotation == Quaternion.Euler(0,0,0))
            PlaySound(0);

    }

    public void ToggleOpen()
    {
        if (_rotationRoutine != null || _moveRoutine != null)
            return;
        if(!_isOpen)
            PlaySound(0);

        _isOpen = !_isOpen;
        
        if (_rotateDoor)
        {
            Quaternion rot = _isOpen ? _openRotation : Quaternion.identity;
            _rotationRoutine = StartCoroutine(RotateDoor(rot));
        }

        if (_moveDoor)
        {
            Vector3 pos = _isOpen ? _openPosition : _closedPosition;
            _moveRoutine = StartCoroutine(MoveDoor(pos));
        }

    }

    public void Open()
    {
        if (_isOpen)
            return;

        _isOpen = true;

        if (_rotateDoor)
        {
            _rotationRoutine = StartCoroutine(RotateDoor(_openRotation));
        }

        if (_moveDoor)
        {
            _moveRoutine = StartCoroutine(MoveDoor(_openPosition));
        }
    }

    private void PlaySound(int i)
    {
        if(i == 0)
        {
            audioSource.clip = sounds[i];
        }
        audioSource.Play();
    }
}
