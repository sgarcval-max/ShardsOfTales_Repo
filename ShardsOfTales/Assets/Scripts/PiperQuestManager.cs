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
        Puzzle1Completed
    }

    public Step CurrentStep { get; set; } = Step.TalkToPiper;

    private void Awake()
    {
        Instance = this;
    }
}