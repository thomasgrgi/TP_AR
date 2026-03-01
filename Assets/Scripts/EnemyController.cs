using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private Transform[] waypoints;
    public float speed = 0.1f;
    public float health = 2f;
    private float maxHealth = 2f;
    public float healthBarScale = 0.1f; // Ajustez cette valeur pour changer la taille de la health bar
    int index = 0;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private GameObject healthBarUI;
    private RectTransform healthBarFill;

    void Start()
    {
        maxHealth = health;
        CreateHealthBar();
        
        // Get sprite renderer for damage flash effect
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (index >= waypoints.Length)
        {
            index = 0;
        }

        // Verify waypoint exists before accessing
        if (index < 0 || index >= waypoints.Length)
        {
            Debug.LogError($"[EnemyController] Index {index} out of bounds for waypoints array of length {waypoints.Length}");
            return;
        }

        Transform targetWaypoint = waypoints[index];
        if (targetWaypoint == null)
        {
            Debug.LogError($"[EnemyController] Waypoint at index {index} is null!");
            index++;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.05f)
            index++;
        
        // Update health bar position
        UpdateHealthBarPosition();
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        
        // Flash red effect
        StartCoroutine(DamageFlash());
        
        // Update health bar
        if (healthBarFill != null)
        {
            float healthPercent = health / maxHealth;
            healthBarFill.localScale = new Vector3(healthPercent, 1f, 1f);
        }
        
        if (health <= 0f)
        {
            GameManager.Instance.EnemyKilled();
            Destroy(gameObject);
            if (healthBarUI != null)
                Destroy(healthBarUI);
        }
    }

    IEnumerator DamageFlash()
    {
        if (spriteRenderer == null) yield break;
        
        // Flash red
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        // Return to original color
        spriteRenderer.color = originalColor;
    }

    void CreateHealthBar()
    {
        // Create a Canvas for the health bar UI (world space)
        GameObject canvasObj = new GameObject("HealthBarCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, 0.4f, 0);
        canvasObj.transform.localRotation = Quaternion.identity;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1, 0.2f) * healthBarScale;
        canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f); // Scale down for world space
        
        // Create background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform);
        bgObj.transform.localPosition = Vector3.zero;
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(1, 0.2f) * healthBarScale;
        bgRect.anchoredPosition = Vector2.zero;
        
        // Create fill bar
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(bgObj.transform);
        fillObj.transform.localPosition = Vector3.zero;
        
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = new Color(0, 1, 0, 0.8f);
        healthBarFill = fillObj.GetComponent<RectTransform>();
        healthBarFill.sizeDelta = new Vector2(1, 0.2f) * healthBarScale;
        healthBarFill.pivot = new Vector2(0, 0.5f);
        healthBarFill.anchoredPosition = new Vector2(-0.5f * healthBarScale, 0);
        
        healthBarUI = canvasObj;
    }

    void UpdateHealthBarPosition()
    {
        // Health bar is now part of the transform hierarchy, so it moves with the enemy automatically
        // No additional positioning needed
    }
}
