
public class WarriorUnit : PathedUnit {
    public IDamagable attackTarget { get; protected set; }
    public virtual void Attack(IDamagable target) { }

    public void ForceDie() {
        Die();
    }
}