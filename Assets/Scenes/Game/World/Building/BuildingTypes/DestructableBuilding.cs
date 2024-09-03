using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
public class DestructableBuilding : BuildingBehaviour, IDamagable {
    public DestructableBuildingData destructableCastedData { get; private set; }
    public UnityEvent deathEvent = new UnityEvent();
    private int health;


    protected virtual void Awake() {
        destructableCastedData = (DestructableBuildingData)data;
        health = destructableCastedData.health;
    }

    public void Damage(int damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        } else {
            float initialScale = transform.localScale.x;
            transform.DOShakeScale(0.3f, initialScale * 0.1f).OnComplete(() => transform.DOScale(initialScale, 0.1f).SetLink(gameObject)).SetLink(gameObject);
        }
    }

    public virtual void Die() {
        deathEvent.Invoke();
        Destroy(gameObject);
    }

    public GameObject GetObject() {
        if (this == null) return null;
        return gameObject;
    }

    
}
