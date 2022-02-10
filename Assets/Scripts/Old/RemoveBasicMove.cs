using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBasicMove : MonoBehaviour
{    
    [SerializeField] private List<ItemData> _objectives = new List<ItemData>();
    public float speed;

    public List<ItemData> Objectives => _objectives;
    private void OnEnable()
    {
        ItemManager.onItemPickedUp += CheckObjective;
    }

    private void OnDisable()
    {
        ItemManager.onItemPickedUp -= CheckObjective;
    }

    private void CheckObjective(ItemData item)
    {
        if (_objectives.Contains(item))
        {
            _objectives.Remove(item);
        }
        if (_objectives.Count == 0)
        {
            DisplayWinMessage();
        }
    }

    private void DisplayWinMessage()
    {
        Debug.Log($"You win!");
    }

    void Update()
    {
        PlayerMovement();
    }

    void PlayerMovement()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        Vector3 playerMovement = new Vector3(hor, 0f, ver) * speed * Time.deltaTime;
        transform.Translate(playerMovement, Space.Self);

        //Comment adding comments cuz its fun
    }
}
