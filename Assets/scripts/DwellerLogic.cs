using System.Collections.Generic;
using UnityEngine;

public class DwellerLogic : MonoBehaviour
{
    public void drink(GameObject target)
    {
        int change = 0;
        switch (dweller.alcholTollerance)
        {
            case 1:
                change = -10 * dweller.hostile;
                break;
            case 2:
                change = -5 * dweller.hostile;
                break;
            case 3:
                change = 0;
                break;
            case 4:
                change = 5 * dweller.friendly;
                break;
            case 5:
                change = 10 * dweller.friendly;
                break;

            default:
                break;
        }
        dweller.UpdateRelationship(target, change);  // Use the serialized change value
    }

    public void work(GameObject target)
    {
        int change = 0;
        switch (dweller.hardWorking)
        {
            case 1:
                change = -10 * dweller.hostile;
                break;
            case 2:
                change = -5 * dweller.hostile;
                break;
            case 3:
                change = 0;
                break;
            case 4:
                change = 5 * dweller.friendly;
                break;
            case 5:
                change = 10 * dweller.friendly;
                break;

            default:
                break;
        }
        dweller.UpdateRelationship(target, change);  // Use the serialized change value
    }


    [System.Serializable]
    public class RelationshipEntry
    {
        [Tooltip("The target Dweller GameObject.")]
        public GameObject Target; // Drag and drop a GameObject with a Dweller script
        public int Score;

        public RelationshipEntry(GameObject target, int score)
        {
            Target = target;
            Score = score;
        }
    }

    [System.Serializable]
    public class Dweller
    {
        [Header("Dweller Information")]
        public string Name;
        public bool IsAlive = true;
        public int alcholTollerance = 1;
        public int hardWorking = 1;
        public int friendly = 1;
        public int hostile = 1;

        [Header("Relationships")]
        [SerializeField]
        [Tooltip("List of relationships with other dwellers.")]
        private List<RelationshipEntry> relationshipEntries = new List<RelationshipEntry>();


        public void UpdateRelationship(GameObject target, int change)
        {
            if (target == null) return;

            // Find the existing relationship entry
            RelationshipEntry entry = relationshipEntries.Find(e => e.Target == target);

            if (entry != null)
            {
                // Update the score if the entry exists
                entry.Score += change;
            }
            else
            {
                // Add a new entry if none exists
                relationshipEntries.Add(new RelationshipEntry(target, change));
            }
        }

        public int GetRelationship(GameObject target)
        {
            if (target == null) return 0;

            // Find the relationship entry
            RelationshipEntry entry = relationshipEntries.Find(e => e.Target == target);

            // Return the score or 0 if no entry exists
            return entry != null ? entry.Score : 0;
        }

        public GameObject Vote()
        {
            GameObject worstDweller = null;
            int worstScore = int.MaxValue;

            foreach (var entry in relationshipEntries)
            {
                if (entry.Target != null)
                {
                    Dweller targetDweller = entry.Target.GetComponent<DwellerLogic>().GetDweller();
                    if (targetDweller != null && targetDweller.IsAlive && entry.Score < worstScore)
                    {
                        worstScore = entry.Score;
                        worstDweller = entry.Target;
                    }
                }
            }

            return worstDweller;
        }
    }

    [SerializeField] Dweller dweller;

    public Dweller GetDweller()
    {
        return dweller;
    }


}

