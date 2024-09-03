using SFH;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee Warrior Data", menuName = "Game/Melee Warrior Data")]
public class MeleeWarriorData : WarriorUnitData {
    [field: SerializeField] public AudioQuerySO fightSound { get; private set; }
    [field: SerializeField] public int damage { get; private set; }
    [field: SerializeField] public float cooldown { get; private set; }
    [field: SerializeField] public float attackDistance { get; private set; }
}