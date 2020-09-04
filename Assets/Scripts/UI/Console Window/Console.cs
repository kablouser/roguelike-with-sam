using System.Text;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Console : Singleton<Console>
{
    public struct Log
    {
        public int turnNumber;
        public string message;

        public Log(int turnNumber, string message)
        {
            this.turnNumber = turnNumber;
            this.message = message;
        }
    }

    private struct LogFade
    {
        public Log log;
        public float addedTime;

        public LogFade(Log log, float addedTime)
        {
            this.log = log;
            this.addedTime = addedTime;
        }
    }

    public List<Log> logs;
    [SerializeField]
    private bool clearFadesOnDisable = true;
    [SerializeField]
    private float visibleDuration = 5.0f;
    [SerializeField]
    private float fadeDuration = 1.0f;
    [SerializeField]
    private bool writeFadesDownwards = true;
    [SerializeField]
    private TextMeshProUGUI fadesText;
    [SerializeField]
    private int maxLines = 5;
    
    private TurnManager turnManager;
    private List<LogFade> logFades;
    private List<string> chronologicalLines;
    private StringBuilder stringBuilder;

    public override void Awake()
    {
        base.Awake();
        if (this == Current)
        {
            logs = new List<Log>(1000);
            turnManager = TurnManager.Current;
            logFades = new List<LogFade>();
            chronologicalLines = new List<string>();
            stringBuilder = new StringBuilder();
        }
    }

    private void OnDisable()
    {
        if (clearFadesOnDisable)
            logFades.Clear();
    }

    public void AddLog(string message)
    {
        logs.Add(new Log(turnManager.turnNumber, message));
        if (gameObject.activeInHierarchy)
        {
            logFades.Add(new LogFade(logs[logs.Count - 1], Time.unscaledTime));

            if (maxLines < logFades.Count)
                logFades.RemoveRange(0, logFades.Count - maxLines);
        }
    }

    private void Update()
    {
        chronologicalLines.Clear();
        stringBuilder.Clear();

        int previousTabLength = 0;
        int previousTurnNumber = 0;
        bool firstLine = true;

        for (int i = 0; i < logFades.Count; i++)
        {
            string colourTagHead = string.Empty,
                colourTagTail = string.Empty;

            if (logFades[i].addedTime + visibleDuration < Time.unscaledTime)
            {
                if (logFades[i].addedTime + visibleDuration + fadeDuration < Time.unscaledTime)
                {                    
                    logFades.RemoveAt(i);
                    i--;
                    continue;
                }

                //start fading
                Color currentColor = fadesText.color;
                currentColor.a = (logFades[i].addedTime + visibleDuration + fadeDuration - Time.unscaledTime) / fadeDuration;                
                colourTagHead = string.Format("<color=#{0}>", ColorUtility.ToHtmlStringRGBA(currentColor));
                colourTagTail = "</color>";
            }

            stringBuilder.Append(colourTagHead);

            if (firstLine || previousTurnNumber != logFades[i].log.turnNumber)
            {
                // turn label
                string turnLabel = string.Format("turn {0}> ", logFades[i].log.turnNumber);
                stringBuilder.Append(turnLabel);
                previousTabLength = turnLabel.Length;
                previousTurnNumber = logFades[i].log.turnNumber;
                firstLine = false;
            }
            else
            {
                // tab some spaces
                stringBuilder.Append(' ', previousTabLength);
            }

            stringBuilder.Append(logFades[i].log.message);
            stringBuilder.Append(colourTagTail);

            chronologicalLines.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }
        
        if (writeFadesDownwards)
        {
            for (int j = 0; j < chronologicalLines.Count; j++)
                stringBuilder.AppendLine(chronologicalLines[j]);
        }
        else
        {
            for (int j = chronologicalLines.Count - 1; 0 <= j; j--)
                stringBuilder.AppendLine(chronologicalLines[j]);
        }

        fadesText.SetText(stringBuilder.ToString());
    }
}
