using System.ComponentModel;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using Spectre.Console;
using Spectre.Console.Cli;

namespace affolterNET.TextExtractor.Terminal.Commands;

public class ExtractJsonCommand: AsyncCommand<ExtractJsonCommand.Settings>
{
    private readonly BasicPdfPipeline _pipeline;
    private readonly IExtractorFileService _extractorFileService;
    private readonly IOutput _log;

    public class Settings : CommandSettings
    {
        [CommandOption("-f|--file")]
        //[DefaultValue("/Users/martin/Source/SocialContractData/AuswahlZollrecht/02_BAZG-VG_DE_verabschiedet BR.pdf")]
        [DefaultValue("/Users/martin/Source/SocialContractData/AuswahlZollrecht/Bundesgesetz/ZG.pdf")]
        public string? Filename { get; set; }

        [CommandOption("-o|--output")]
        [DefaultValue("/Users/martin/Source/SocialContractData/Output/BAZG-VG")]
        public string? OutputFolder { get; set; }

        public override ValidationResult Validate()
        {
            if (!File.Exists(Filename))
            {
                return ValidationResult.Error($"file does not exist: {Filename}");
            }

            if (!Directory.Exists(OutputFolder))
            {
                try
                {
                    Directory.CreateDirectory(OutputFolder!);
                }
                catch (Exception ex)
                {
                    return ValidationResult.Error($"output folder does not exist and could not be created: {OutputFolder}");
                }
            }

            return base.Validate();
        }
    }

    public ExtractJsonCommand(BasicPdfPipeline pipeline, IExtractorFileService extractorFileService, IOutput log)
    {
        _pipeline = pipeline;
        _extractorFileService = extractorFileService;
        _log = log;
    }

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var pipelineContext = new PipelineContext(settings.Filename!);
            _pipeline.AddStep(new SerializeToBlobStorage(_extractorFileService, _log));
            _pipeline.Execute(pipelineContext);
        }
        catch (Exception ex)
        {
            _log.WriteException(ex);
            return Task.FromResult(-1);
        }

        return Task.FromResult(0);
    }
}