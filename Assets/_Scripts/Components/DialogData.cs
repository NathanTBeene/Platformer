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
        public bool autoNext = false; // If true, automatically proceed to next line after delay
        public float autoDelay = 0f; // Delay before proceeding to next line

        public DialogLine(string text, float duration)
        {
            this.text = text;
            this.duration = duration;
        }
    }

    public DialogLine[] lines;
    public DialogType type;

    private int currentIndex = 0;
    public int CurrentIndex => currentIndex;
    private int displayedIndex = -1;

    private DialogLine currentLine = null;
    public DialogLine CurrentLine => currentLine;

    private string currentText;

    public DialogData() { }
    public DialogData(string text, float duration, DialogType type)
    {
        this.lines = new DialogLine[] { new DialogLine(text, duration) };
        this.type = type;
    }

    public bool HasNextDialogue()
    {
        switch (type)
        {
            case DialogType.Single:
                return false;
            case DialogType.Sequence:
                return displayedIndex < lines.Length - 1;
            case DialogType.Repeat:
                return lines.Length > 1;
            case DialogType.Random:
                return lines.Length > 1;
            default:
                return false;
        }
    }

    public string GetCurrentDialogue()
    {
        if (lines.Length == 0) return "";
        if (currentLine == null)
        {
            _setCurrentLine();
        }
        return currentText ?? "";
    }

    public string GetNextDialogue()
    {
        Debug.Log("Getting next dialogue of type: " + type.ToString());

        if (lines.Length == 0) return "";

        switch (type)
        {
            case DialogType.Single:
                _setCurrentToIndex(0);
                break;
            case DialogType.Sequence:
                _setCurrentToIndex(currentIndex);
                // Only advance index if not at the end
                if (currentIndex < lines.Length - 1)
                    displayedIndex = currentIndex;
                    currentIndex++;
                break;
            case DialogType.Repeat:
                _setCurrentToIndex(currentIndex);
                displayedIndex = currentIndex;
                currentIndex = (currentIndex + 1) % lines.Length;
                break;
            case DialogType.Random:
                displayedIndex = currentIndex;
                int randomIndex = Random.Range(0, lines.Length);
                _setCurrentToIndex(randomIndex);
                break;
        }

        return currentText;
    }

    private void _setCurrentToIndex(int index)
    {
        if (index >= 0 && index < lines.Length)
        {
            currentLine = lines[index];
            currentText = currentLine.text;
        }
    }

    private void _setCurrentLine()
    {
        _setCurrentToIndex(currentIndex);
    }

    // Advance without getting text
    public void AdvanceToNext()
    {
        if (!HasNextDialogue()) return;

        switch (type)
        {
            case DialogType.Sequence:
                if (currentIndex < lines.Length - 1)
                    displayedIndex = currentIndex;
                    currentIndex++;
                    _setCurrentToIndex(currentIndex);
                break;
            case DialogType.Repeat:
                displayedIndex = currentIndex;
                currentIndex = (currentIndex + 1) % lines.Length;
                _setCurrentToIndex(currentIndex);
                break;
            case DialogType.Random:
                displayedIndex = currentIndex;
                currentIndex = Random.Range(0, lines.Length);
                _setCurrentToIndex(currentIndex);
                break;
        }
    }
}
