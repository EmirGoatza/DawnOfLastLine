using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Statistiques de Base")]
    [SerializeField] protected string enemyName;
    [SerializeField] protected float health;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int damage;

    protected Transform player;

    protected virtual void Awake()
    {
        // On trouve le joueur automatiquement au démarrage
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{enemyName} a reçu {amount} de dégâts. Vie restante : {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    protected abstract void Move();

    protected virtual void Die()
    {
        Debug.Log($"{enemyName} est mort !");
        Destroy(gameObject);
    }

    protected virtual void Update()
    {

        Move();

    }
}
