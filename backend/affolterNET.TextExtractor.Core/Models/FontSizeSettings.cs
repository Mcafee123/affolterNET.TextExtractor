using affolterNET.TextExtractor.Core.Extensions;

namespace affolterNET.TextExtractor.Core.Models;

public class FontSizeSettings : List<FontSizeSettings.FontSizeSetting>
{
    private int _groupIdentifier = 0;

    public FontSizeSettings(List<IWordOnPage> words)
    {
        var selectedWords = words.Where(w => !string.IsNullOrWhiteSpace(w.Text)).ToList();
        AllFontNames = selectedWords.Select(w => w.FontName).Distinct().ToList();
        var fsSettings = selectedWords
            .FindCommonGroups<IWordOnPage>(0.5, w => w.FontSizeAvg);
        foreach (var foundGroup in fsSettings)
        {
            Add(new FontSizeSetting
            {
                GroupId = _groupIdentifier++,
                WordCount = foundGroup.Count,
                AvgFontSize = foundGroup.AvgValue,
                MinFontSize = foundGroup.MinValue,
                MaxFontSize = foundGroup.MaxValue,
                FontNames = foundGroup.Select(gi => gi.Obj.FontName).Distinct().ToList()
            });
        }
    }

    public new FontSizeSetting this[int groupId]
    {
        get => this.First(fs => fs.GroupId == groupId);
        set => throw new InvalidOperationException("Setting a FontSizeSetting by groupId is not allowed.");
    }

    public List<string> AllFontNames { get; set; }

    public FontSizeSetting GetGroup(double lineFontSize)
    {
        return this.OrderBy(fs => Math.Abs(fs.AvgFontSize - lineFontSize)).First();
    }
    
    public double GetTopDistanceDiff(double fontSize, double topDistance)
    {
        var fontSizeGroup = GetGroup(fontSize);
        var mostCommonLineSpacing = fontSizeGroup.MostCommonLineSpacing();
        var distanceDiff = Math.Abs(mostCommonLineSpacing - topDistance);
        return distanceDiff;
    }

    public class FontSizeSetting
    {
        private List<double> _lineSpacings = new();
        public int GroupId { get; set; }
        public double AvgFontSize { get; set; }
        public double MinFontSize { get; set; }
        public double MaxFontSize { get; set; }
        public int WordCount { get; set; }
        public List<string> FontNames { get; set; } = new();

        public void AddSpacing(double lineSpacing)
        {
            if (lineSpacing >= 2 * AvgFontSize)
            {
                return;
            }

            _lineSpacings.Add(lineSpacing);
        }

        public double MostCommonLineSpacing()
        {
            if (_lineSpacings.Count < 1)
            {
                // if there are no line spacings for a font-size, 
                // there are no consecutive lines of this size.
                // this can happen for titles - for titles we want
                // to create new blocks. therefore, return a big value.
                return LineOnPage.DefaultTopDistance;
            }

            var groups = _lineSpacings.FindCommonGroups(0.5, d => d);
            return groups.First().AvgValue;
        }
    }
}