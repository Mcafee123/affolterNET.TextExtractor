using System.ComponentModel;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Models;
using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace affolterNET.TextExtractor.Terminal.Commands;

public class DetectTextCommand: AsyncCommand<DetectTextCommand.Settings>
{
    private readonly BasicPdfPipeline _pipeline;
    private readonly IOutput _log;

    public class Settings : CommandSettings
    {
        [CommandOption("-f|--file")]
        [DefaultValue("/Users/martin/Downloads/Verfuegung_Nr_23-24_24846_3.pdf")]
        public string? Filename { get; set; }

        public override ValidationResult Validate()
        {
            if (!File.Exists(Filename))
            {
                return ValidationResult.Error($"file does not exist: {Filename}");
            }

            return base.Validate();
        }
    }

    public DetectTextCommand(BasicPdfPipeline pipeline, IOutput log)
    {
        _pipeline = pipeline;
        _log = log;
    }

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var pipelineContext = new PipelineContext(settings.Filename!);
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