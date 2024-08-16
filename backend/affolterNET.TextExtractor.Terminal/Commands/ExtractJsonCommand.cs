using System.ComponentModel;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace affolterNET.TextExtractor.Terminal.Commands;

public class ExtractJsonCommand : AsyncCommand<ExtractJsonCommand.Settings>
{
    private readonly ServiceFactory _serviceFactory;
    private readonly IOutput _log;

    public class Settings : CommandSettings
    {
        [CommandOption("-f|--file")]
        // [DefaultValue("/Users/martin/Source/SocialContractData/AuswahlZollrecht/02_BAZG-VG_DE_verabschiedet BR.pdf")]
        [DefaultValue("/Users/martin/Source/SocialContractData/AuswahlZollrecht/Bundesgesetz/ZG.pdf")]
        public string? FileName { get; set; }

        public override ValidationResult Validate()
        {
            if (!string.IsNullOrWhiteSpace(FileName) && !File.Exists(FileName))
            {
                return ValidationResult.Error($"file does not exist: {FileName}");
            }

            return base.Validate();
        }
    }

    public ExtractJsonCommand(ServiceFactory serviceFactory, IOutput log)
    {
        _serviceFactory = serviceFactory;
        _log = log;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            LogFileName($"Parsing file: {settings.FileName}");
            var pipeline = _serviceFactory.CreatePipeline(settings.FileName!);
            var serializeToBlobStorageStep = _serviceFactory.GetService<SerializeToBlobStorageStep>();
            pipeline.AddStep(serializeToBlobStorageStep);
            var pipelineContext = new PipelineContext(settings.FileName!);
            await pipeline.Execute(pipelineContext);
        }
        catch (Exception ex)
        {
            _log.WriteException(ex);
            return -1;
        }

        return 0;
    }

    private void LogFileName(string msg)
    {
        var line = string.Empty.PadRight(msg.Length, '-');
        _log.Write(EnumLogLevel.Info, line);
        _log.Write(EnumLogLevel.Info, msg);
        _log.Write(EnumLogLevel.Info, line);
    }
}