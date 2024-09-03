using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class UnitBehaviour : MonoBehaviour, IDamagable {
    [field: SerializeField] public UnitData data { get; private set; }
    private int health;
    protected SpriteRenderer spriteRenderer;

    public void Damage(int damage) {
        health -= damage;
        if (health <= 0) {
            Die();
        }
    }

    public GameObject GetObject() {
        if (this == null) return null;
        return gameObject;
    }

    protected virtual void Die() {
        //Play death animation/sound
        UnitManager.inst.DestroyUnit(this);
    }

    protected virtual void Awake() {
        health = data.maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sortingOrder = 10;
    }

    protected virtual void Update() {
        if (spriteRenderer != null) {
            float y = Mathf.Sin(Time.time * 15f) * 0.0333f;
            spriteRenderer.transform.localScale = new Vector3(1, 1 + y, 1);
        }
    }
}
