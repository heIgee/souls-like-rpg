using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int dropAmount;
    [SerializeField] private ItemData[] possibleDrop;

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()
    {
        // okay, this one is mine and this is better
        // (and I think statistically correct)
        while (dropAmount > 0)
        {
            ItemData randomDrop = possibleDrop[Random.Range(0, possibleDrop.Length)];

            if (Random.Range(0, 100) < randomDrop.dropChance)
            {
                DropItem(randomDrop);
                dropAmount--;
            }
        }
    }

    protected void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }

    protected void DropItem(ItemData itemData, int amount = 1) 
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

            var dropVelocity = new Vector2(Random.Range(-5f, 5f), Random.Range(10f, 20f));
            newDrop.GetComponent<ItemObject>().SetupItem(itemData, dropVelocity);
        }
    }
}
