using System.ComponentModel;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Terminal.Extensions;
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
        // [DefaultValue("/Users/martin/Source/SocialContractData/AuswahlZollrecht/02_BAZG-VG_DE_verabschiedet BR.pdf")]
        // [DefaultValue("/Users/martin/Source/SocialContractData/AuswahlZollrecht/Bundesgesetz/ZG.pdf")]
        public string? FileName { get; set; }

        [CommandOption("-d|--dir")]
        [DefaultValue("/Users/martin/Source/SocialContractData/AuswahlZollrecht")]
        public string? FolderName { get; set; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(FileName) && string.IsNullOrWhiteSpace(FolderName))
            {
                return ValidationResult.Error("either FileName or FolderName must be provided");    
            }
            
            if (!string.IsNullOrWhiteSpace(FileName) && !string.IsNullOrWhiteSpace(FolderName))
            {
                return ValidationResult.Error("either FileName or FolderName must be provided");    
            }

            if (!string.IsNullOrWhiteSpace(FileName) && !File.Exists(FileName))
            {
                return ValidationResult.Error($"file does not exist: {FileName}");
            }
            
            if (!string.IsNullOrWhiteSpace(FolderName) && !Directory.Exists(FolderName))
            {
                return ValidationResult.Error($"folder does not exist: {FolderName}");
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
            // add blob file extractor to the pipeline
            _pipeline.AddStep(new SerializeToBlobStorage(_extractorFileService, _log));
            if (!string.IsNullOrWhiteSpace(settings.FolderName))
            {
                _log.Write(EnumLogLevel.Info, $"Collecting pdf-files in directory subtree: {settings.FolderName}...");
                var filesInDirectorySubtree = settings.FolderName.GetFilesInAllDirectories();
                foreach (var file in filesInDirectorySubtree)
                {
                    LogFileName($"Parsing file: {file.FullName}");
                    var pipelineContext = new PipelineContext(file.FullName);
                    _pipeline.Execute(pipelineContext);
                }
            }
            else
            {
                LogFileName($"Parsing file: {settings.FileName}");
                var pipelineContext = new PipelineContext(settings.FileName!);
                _pipeline.Execute(pipelineContext);
            }
        }
        catch (Exception ex)
        {
            _log.WriteException(ex);
            return Task.FromResult(-1);
        }

        return Task.FromResult(0);
    }

    private void LogFileName(string msg)
    {
        var line = string.Empty.PadRight(msg.Length, '-');
        _log.Write(EnumLogLevel.Info, line);
        _log.Write(EnumLogLevel.Info, msg);
        _log.Write(EnumLogLevel.Info, line);
    }
}