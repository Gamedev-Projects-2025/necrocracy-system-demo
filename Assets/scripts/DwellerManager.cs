using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static DwellerLogic;
using TMPro;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.SceneManagement;

public class DwellerManager : MonoBehaviour
{
    [SerializeField] private GameObject execution;
    [SerializeField] private List<GameObject> dwellers;   // List of all dwellers
    [SerializeField] private GameObject playerDweller;    // The player's dweller
    private GameObject playerVote;                       // To store the player's vote
    private bool hasPlayerVoted = false;                 // Flag to ensure the player votes first

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Optional: Initialize dwellers or perform setup
    }

    // Update is called once per frame
    void Update()
    {
        // Update gameplay logic if needed
    }

    public void SetPlayerVote(GameObject target)
    {
        // Ensure the target is valid and not the player themselves
        if (target != playerDweller)
        {
            playerVote = target;
            hasPlayerVoted = true;
        }
        else
        {
            Debug.LogWarning("Player cannot vote for themselves!");
        }
    }

    public void VoteSession()
    {
        if (!hasPlayerVoted)
        {
            Debug.LogWarning("Player has not voted yet. Voting session cannot proceed.");
            return;
        }

        if (dwellers == null || dwellers.Count == 0)
        {
            Debug.LogWarning("No dwellers available for voting.");
        }

        // Dictionary to store vote counts
        Dictionary<GameObject, int> voteCounts = new Dictionary<GameObject, int>();

        // Add the player's vote to the tally
        if (playerVote != null)
        {
            if (voteCounts.ContainsKey(playerVote))
            {
                voteCounts[playerVote]++;
            }
            else
            {
                voteCounts[playerVote] = 1;
            }
        }

        // Gather votes from all other dwellers except the player
        foreach (GameObject dwellerObject in dwellers)
        {
            if (dwellerObject == playerDweller || dwellerObject == null) continue;

            Dweller dweller = dwellerObject.GetComponent<DwellerLogic>().GetDweller();
            if (dweller != null && dweller.IsAlive)
            {
                GameObject votedAgainst = dweller.Vote();
                if (votedAgainst != null)
                {
                    if (voteCounts.ContainsKey(votedAgainst))
                    {
                        voteCounts[votedAgainst]++;
                    }
                    else
                    {
                        voteCounts[votedAgainst] = 1;
                    }
                }
            }
        }

        // Determine the most voted person
        GameObject mostVoted = voteCounts
            .OrderByDescending(vote => vote.Value)
            .FirstOrDefault().Key;

        string verdict = "";
        // Log the results for debugging
        Debug.Log("Voting results:");
        foreach (var vote in voteCounts)
        {
            verdict += $"{vote.Key.name} received {vote.Value} votes.\n";
        }

        if (mostVoted != null)
        {
            verdict += $"{mostVoted.name} is the most voted person.\n";
        }
        else
        {
            Debug.Log("No votes were cast.");
        }

        execution.GetComponent<TextMeshProUGUI>().text = verdict;
        mostVoted.GetComponent<DwellerLogic>().GetDweller().IsAlive = false;
        dwellers.Remove(mostVoted);
        Destroy(mostVoted);

        if (mostVoted == playerDweller)
        {
            SceneManager.LoadScene("gameover");
        }
        if (dwellers.Count == 2)
        {
            SceneManager.LoadScene("victory");
        }
    }

    public void bar()
    {
        foreach (GameObject dwellerObject in dwellers)
        {
            if (dwellerObject == playerDweller || dwellerObject == null) continue;

            if (dwellerObject != null)
            {
                dwellerObject.GetComponent<DwellerLogic>().drink(playerDweller);
            }
        }
    }

    public void work()
    {
        foreach (GameObject dwellerObject in dwellers)
        {
            if (dwellerObject == playerDweller || dwellerObject == null) continue;

            if (dwellerObject != null)
            {
                dwellerObject.GetComponent<DwellerLogic>().work(playerDweller);
            }
        }
    }

    public void opinon(GameObject dweller)
    {
        execution.GetComponent<TextMeshProUGUI>().text = dweller.GetComponent<DwellerLogic>().GetDweller().GetRelationship(playerDweller) >= 50 ? "They like you" : "The dislike you";
    }

    public void beFriend(GameObject friend)
    {
        friend.GetComponent<DwellerLogic>().GetDweller().friendly++;
        friend.GetComponent<DwellerLogic>().GetDweller().UpdateRelationship(playerDweller, 10);

        foreach (GameObject dwellerObject in dwellers)
        {
            if (dwellerObject == playerDweller || dwellerObject == friend || dwellerObject == null) continue;

            Dweller dweller = dwellerObject.GetComponent<DwellerLogic>().GetDweller();

            if (dweller != null)
            {
                if (dweller.GetRelationship(friend) < 50)
                {
                    dweller.UpdateRelationship(playerDweller, -10);
                }
                else
                {
                    dweller.UpdateRelationship(playerDweller, 10);
                }
            }
        }
    }

    public void rumor(GameObject target)
    {
        foreach (GameObject dwellerObject in dwellers)
        {
            if (dwellerObject == playerDweller || dwellerObject == target || dwellerObject == null) continue;

            Dweller dweller = dwellerObject.GetComponent<DwellerLogic>().GetDweller();

            if (dweller != null)
            {
                dweller.UpdateRelationship(target, -10);
            }
        }
    }
}
