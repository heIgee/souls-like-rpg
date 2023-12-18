using TMPro;
using UnityEngine;

public class BlackHoleHotkeyController : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode hotkey;
    private TextMeshProUGUI keyText;

    private Transform enemy;
    private BlackHoleController controller;

    public void SetupKey(BlackHoleController controller, KeyCode hotkey, Transform enemy)
    {
        this.controller = controller;
        this.hotkey = hotkey;
        this.enemy = enemy;

        sr = GetComponent<SpriteRenderer>();

        keyText = GetComponentInChildren<TextMeshProUGUI>();  
        keyText.text = hotkey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(hotkey))// && !controller.targets.Contains(enemy))
        {
            controller.targets.Add(enemy);
            keyText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}

