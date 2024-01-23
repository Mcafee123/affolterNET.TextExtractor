using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services;
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
        var readStep = new ReadWordsStep(wordExtractor, _log);
        var linesStep = new DetectLinesStep(lineDetector, _log);
        var blocksStep = new DetectTextBlocksStep(blockDetector, _log);
        var pipeline = new BasicProcessingPipeline(readStep, linesStep, blocksStep, _log);

        var path = "/Users/martin/Downloads/Verfuegung_Nr_23-24_24846_3.pdf";
        var context = new PipelineContext(path);
        pipeline.Execute(context);

        context.Document!.ToJson("/Users/martin/Source/affolterNET.TextExtractor/frontend/src/assets/Verfuegung_Nr_23-24_24846_3.json");
    }
}