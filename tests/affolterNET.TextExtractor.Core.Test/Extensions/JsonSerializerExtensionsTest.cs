using affolterNET.TextExtractor.Core.Extensions;
using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services.Detectors;
using Xunit.Abstractions;

namespace affolterNET.TextExtractor.Core.Test.Extensions;

public class JsonSerializerExtensionsTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly TestOutput _log;

    public JsonSerializerExtensionsTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _log = new TestOutput(testOutputHelper);
    }

    [Fact]
    public void PdfDoc_ToJson_Test()
    {
        var wordExtractor = new PdfWordExtractor(_log);
        var lineDetector = new LineDetector(_log);
        var blockDetector = new BlockDetector(lineDetector, _log);
        var footnoteDetector = new FootnoteDetector(_log);
        var pageNumberService = new PageNumberService(_log);
        var headerService = new HeaderService(_log);
        var wordCleaner = new WordCleaner(_log);
        var fixSpacesService = new FixSpacesService(lineDetector, _log);
        var readStep = new ReadPagesStep(wordExtractor, _log);
        var cleanWordsStep = new CleanWordsStep(wordCleaner, _log);
        var analyzeLinesStep = new AnalyzeLineSpacingStep(_log);
        var blocksStep = new DetectTextBlocksStep(blockDetector, _log);
        var footnotesStep = new DetectFootnotesStep(footnoteDetector, _log);
        var detectPageNumberStep = new DetectPageNumberStep(pageNumberService, _log);
        var detectHeadersStep = new DetectHeadersStep(headerService, _log);
        var fixSpacesStep = new FixSpacesStep(fixSpacesService, _log);
        var extractTextStep = new ExtractTextStep(_log);
        var pipeline = new BasicPdfPipeline(
            readStep, 
            cleanWordsStep, 
            analyzeLinesStep, 
            blocksStep, 
            footnotesStep, 
            detectPageNumberStep, 
            detectHeadersStep, 
            fixSpacesStep, 
            extractTextStep, 
            _log);
        // var path = "/Users/martin/Downloads/Verfuegung_Nr_23-24_24846_3.pdf";
        var path = "/Users/martin/Downloads/nov-wingo-17463269.pdf";
        var context = new PipelineContext(path);
        pipeline.Execute(context);

        context.Document!.SerializePdfDoc("/Users/martin/Source/affolterNET.TextExtractor/frontend/src/assets/Verfuegung_Nr_23-24_24846_3.json", _log);
    }
}