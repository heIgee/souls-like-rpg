using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

public class BlackHoleController : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodes;

    private float maxSize; 
    private float growSpeed;
    private float shrinkSpeed;

    public bool canGrow = true;
    public bool canShrink;

    private int attacksAmount = 4;
    private float cloneAttackCooldown = 0.3f;
    private float cloneAttackTimer;
    private bool cloneAttackReleased;

    private float blackHoleTimer;

    public List<Transform> targets = new();
    public List<GameObject> activeKeys = new();

    private bool canCreateKeys = true;
    public bool canExitState;

    // I don't like this skill-controller pattern with such bulky setup methods
    public void SetupBlackHole(float maxSize, float growSpeed, float shrinkSpeed, 
        int attacksAmount, float cloneAttackCooldown, float blackHoleDuration)
    {
        this.maxSize = maxSize;
        this.growSpeed = growSpeed;
        this.shrinkSpeed = shrinkSpeed;
        this.attacksAmount = attacksAmount;
        this.cloneAttackCooldown = cloneAttackCooldown;
        blackHoleTimer = blackHoleDuration;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackHoleTimer -= Time.deltaTime;

        if (blackHoleTimer < 0)
        {
            blackHoleTimer = Mathf.Infinity;

            if (targets.Count > 0) 
                ReleaseCloneAttack();
            else
                FinishBlackHole();
        }

        if (Input.GetKeyDown(KeyCode.R))
            ReleaseCloneAttack();

        CloneAttackBehaviour();
        ResizeBehaviour();
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count <= 0)
            return; 

        ClearKeys();
        cloneAttackReleased = true;
        canCreateKeys = false;

        if (!canExitState)
        PlayerManager.instance.player.SetTransparency(true);
    }

    private void CloneAttackBehaviour()
    {
        if (attacksAmount <= 0)
        {
            Invoke(nameof(FinishBlackHole), 1f);
            return;
        }

        if (cloneAttackTimer < 0 && cloneAttackReleased && targets.Count > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            float xOffset = (Random.Range(0, 1) == 0)
                ? -2
                : 2;

            //Debug.LogWarning("Targets.Count: " + targets.Count);
            int randomIndex = Random.Range(0, targets.Count);

            SkillManager.instance.Clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));

            attacksAmount--;
        }
    }

    private void FinishBlackHole()
    {
        ClearKeys();
        canExitState = true;
        cloneAttackReleased = false;

        canGrow = false;
        canShrink = true;
    }

    private void ResizeBehaviour()
    {
        if (canGrow && !canShrink)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize),
                growSpeed * Time.deltaTime);

        if (canShrink && !canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1),
                shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ClearKeys()
    {
        if (activeKeys.Count <= 0)
            return;

        foreach (GameObject key in activeKeys)
            Destroy(key);
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;

        collision.GetComponent<Enemy>().SetFreezeTime(true);

        SetupKey(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
            collision.GetComponent<Enemy>().SetFreezeTime(false);
    }

    private void SetupKey(Collider2D collision)
    {
        if (keyCodes.Count <= 0 || keyCodes == null)
        {
            Debug.LogWarning("Ran out of hotkeys to assign!");
            return;
        }

        if (!canCreateKeys)
            return;

        GameObject hotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2),
            Quaternion.identity);
        activeKeys.Add(hotKey); 

        KeyCode chosenKey = keyCodes[Random.Range(0, keyCodes.Count)];
        keyCodes.Remove(chosenKey);

        BlackHoleHotkeyController controller = hotKey.GetComponent<BlackHoleHotkeyController>();
        controller.SetupKey(this, chosenKey, collision.transform);
    }
} 
