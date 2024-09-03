using System.Collections;
using System.Collections.Generic;
using SFH;
using UnityEngine;
[CreateAssetMenu(fileName = "AudioHelper", menuName = "Game/Audio Helper")]
public class AudioHelperSO : ScriptableObject
{
    public void PlaySound(AudioQuerySO query)
    {
        AudioManager.inst.PlayQuery(query);
    }
    public void PlaySetSound(AudioQuerySetSO set)
    {
        AudioManager.inst.PlayRandomFromSet(set);
    }
}
