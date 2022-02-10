using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalChecker : MonoBehaviour
{
    [SerializeField] private bool _isNightLevel = false;

    private bool _isListComplete = false;

    private void OnEnable()
    {
        ObjectiveManager.onObjectivesCompleted += OnObjectivesComplete;
    }

    private void OnDisable()
    {
        ObjectiveManager.onObjectivesCompleted -= OnObjectivesComplete;
    }

    private void OnObjectivesComplete()
    {
        _isListComplete = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {

            ObjectiveManager.Instance.Win();
            string soundToPlay;

            if (_isNightLevel)
            {
                soundToPlay = _isListComplete ? "Night_LevelCompList" : "Night_LevelComp";
            }
            else
            {
                soundToPlay = _isListComplete ? "Day_LevelCompList" : "Day_LevelComp";
            }

            
            AudioManager.Instance.PlayVoiceLine(name: soundToPlay, interrupt: true);
        }
    }
}
