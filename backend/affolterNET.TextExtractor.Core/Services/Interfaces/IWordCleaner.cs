using affolterNET.TextExtractor.Core.Pipeline.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Steps;

namespace affolterNET.TextExtractor.Core.Services.Interfaces;

public interface IWordCleaner
{
    void RemoveBigSpaces(IPipelineContext context, CleanWordsStep.CleanWordsStepSettings settings);
    void FixMixedBaselineWords(IPipelineContext context, double minBaseLineDiff);
    void FixMixedLetterTypeWords(IPipelineContext context);
    void SeparatePunctuation(IPipelineContext context, char[] startPunctuationChars, char[] endPunctuationChars);
}