using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingHutPlaces : MonoBehaviour
{
    [SerializeField] private List<Transform> pointToStand = new List<Transform>(); 
    
    public void TryPutUnit(UnitBehaviour instance)
    {
        for(int i = 0; i < pointToStand.Count; i++)
        {
            if(pointToStand[i].childCount == 0)
            {
                instance.transform.SetParent(pointToStand[i]);
                instance.transform.position = pointToStand[i].position;
                break;
            }
        }
    }
}
