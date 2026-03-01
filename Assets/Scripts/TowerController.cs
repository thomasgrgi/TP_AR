using UnityEngine;
using System.Collections;

public class TowerController : MonoBehaviour
{
    public float range = 0.05f;  // Portée de tir 
    public float fireRate = 1f;
    public float lineWidth = 0.005f;  // epaisseur du tir visuel
    public bool showLogs = false;
    float nextFireTime = 0f;
    
    private LineRenderer shootLine;

    void Start()
    {
        // Create LineRenderer for shooting effect
        shootLine = gameObject.AddComponent<LineRenderer>();
        shootLine.material = new Material(Shader.Find("Sprites/Default"));
        shootLine.startColor = Color.yellow;
        shootLine.endColor = Color.red;
        shootLine.startWidth = lineWidth;
        shootLine.endWidth = lineWidth;
        shootLine.enabled = false;
    }

    void Update()
    {
        if (Time.time < nextFireTime) return;

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if (showLogs)
                Debug.Log($"[Tower] Ennemi détecté à distance: {distance:F3}, Range: {range:F6}");
            
            if (distance < range)
            {
                var ec = e.GetComponent<EnemyController>();
                if (ec != null)
                {
                    if (showLogs)
                        Debug.Log($"[Tower] TIR! Distance {distance:F3} < Range {range:F6}");
                    // Visual feedback: show shooting line
                    ShowShootEffect(e.transform.position);
                    ec.TakeDamage(1f);
                }
                nextFireTime = Time.time + fireRate;
                break;
            }
        }
    }

    void ShowShootEffect(Vector3 targetPosition)
    {
        // Show a line from tower to target
        shootLine.SetPosition(0, transform.position);
        shootLine.SetPosition(1, targetPosition);
        shootLine.enabled = true;
        
        // Fade out the line after a short duration
        StartCoroutine(FadeOutLine());
    }

    IEnumerator FadeOutLine()
    {
        float fadeTime = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            
            Color startColor = shootLine.startColor;
            Color endColor = shootLine.endColor;
            startColor.a = alpha;
            endColor.a = alpha;
            
            shootLine.startColor = startColor;
            shootLine.endColor = endColor;
            
            yield return null;
        }

        shootLine.enabled = false;
    }
}
