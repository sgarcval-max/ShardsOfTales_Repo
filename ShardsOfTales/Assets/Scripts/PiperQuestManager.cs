using System.Collections.Generic;
using UnityEngine;

public class PiperQuestManager : MonoBehaviour
{
    public static PiperQuestManager Instance { get; private set; }

    public enum Step
    {
        TalkToPiper,
        FindSheetMusic,
        PlayOcarina,
        HerdRatsToForest,
        ReturnToPiper,
        // --- PUZZLE 2: BUSCA A LOS NIÑOS ---
        TalkToFather,
        FindDeafChild,
        FindPencil,
        TalkDeafWithPencil,
        FindBlindChild,
        RiverMinigame,
        CheckFootprints,
        FindCrippledKids,
        ClearCaveRocks,
        EnterCave
    }

    public Step CurrentStep { get; set; } = Step.TalkToPiper;

    [Header("Progreso de Ratas (Puzzle 1)")]
    public int totalRats = 5;
    private List<RatFollower> followingRats = new List<RatFollower>();
    private int deliveredRatsCount = 0;

    [Header("Objetos de Transición (Puzzle 1 -> Puzzle 2)")]
    public GameObject fatherNPC;
    public GameObject townGate;

    private void Awake()
    {
        Instance = this;
        if (fatherNPC != null) fatherNPC.SetActive(false);
    }

    public void RegisterFollowingRat(RatFollower rat)
    {
        if (!followingRats.Contains(rat))
        {
            followingRats.Add(rat);
        }
    }

    public int GetFollowIndex(RatFollower rat)
    {
        return followingRats.IndexOf(rat);
    }

    public void DeliverFollowingRatsToForest()
    {
        if (CurrentStep != Step.HerdRatsToForest || followingRats.Count == 0) return;

        int deliveredNow = followingRats.Count;
        foreach (RatFollower rat in followingRats)
        {
            if (rat != null) rat.EnterForest();
        }
        followingRats.Clear();

        deliveredRatsCount += deliveredNow;

        if (deliveredRatsCount >= totalRats)
        {
            CurrentStep = Step.ReturnToPiper;
            if (SimpleDialogueUI.Instance != null)
            {
                SimpleDialogueUI.Instance.ShowDialogue("Bibliotecario", new string[]
                {
                    $"¡Has llevado a todas las ratas ({deliveredRatsCount}/{totalRats}) al bosque!",
                    "Vuelve a hablar con el Flautista para avisarle."
                });
            }
        }
        else
        {
            if (SimpleDialogueUI.Instance != null)
            {
                SimpleDialogueUI.Instance.ShowDialogue("Bibliotecario", new string[]
                {
                    $"¡Las ratas han entrado en el bosque! ({deliveredRatsCount}/{totalRats}).",
                    "Aún quedan ratas por el pueblo, ve a buscarlas."
                });
            }
        }
    }

    public void StartPuzzle2AfterPiperLeaves(GameObject piperObj)
    {
        if (piperObj != null) piperObj.SetActive(false);
        if (fatherNPC != null) fatherNPC.SetActive(true);
        CurrentStep = Step.TalkToFather;
    }

    public void OpenTownGate()
    {
        if (townGate != null) townGate.SetActive(false);
    }
}