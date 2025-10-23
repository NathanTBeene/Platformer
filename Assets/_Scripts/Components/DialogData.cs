using UnityEngine;

[System.Serializable]
public class DialogData
{
    public enum DialogType
    {
        Single, // One-time display
        Sequence, // Sequential display (stops at the end and shows last line for all future calls)
        Repeat, // Loops through the lines
        Random // Random line each time
    }

    [System.Serializable]
    public class DialogLine
    {
        [TextArea(5, 3)]
        public string text;
        public float duration;

        public DialogLine(string text, float duration)
        {
            this.text = text;
            this.duration = duration;
        }
    }

    public DialogLine[] lines;
    public DialogType type;

    private int currentIndex = 0;
    private string currentText;

    public DialogData() { }
    public DialogData(string text, float duration, DialogType type)
    {
        this.lines = new DialogLine[] { new DialogLine(text, duration) };
        this.type = type;
    }

    public string GetNextDialogue()
    {
        Debug.Log("Getting next dialogue of type: " + type.ToString());
        if (type == DialogType.Single)
        {
            currentText = lines[0].text;
            return currentText;
        }
        else if (type == DialogType.Sequence)
        {
            if (lines.Length == 0) return "";

            string line = lines[currentIndex].text;
            if (currentIndex < lines.Length - 1)
            {
                currentIndex++;
            }
            currentText = line;
            return line;
        }
        else if (type == DialogType.Repeat)
        {
            if (lines.Length == 0) return "";

            string line = lines[currentIndex].text;
            currentIndex = (currentIndex + 1) % lines.Length;
            currentText = line;
            return line;
        }
        else if (type == DialogType.Random)
        {
            if (lines.Length == 0) return "";

            int randomIndex = Random.Range(0, lines.Length);
            currentText = lines[randomIndex].text;
            return currentText;
        }
        return "";
    }

    public string GetCurrentDialogue()
    {
        return currentText;
    }
}
