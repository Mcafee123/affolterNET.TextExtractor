using System.ComponentModel;
using affolterNET.TextExtractor.Core.Helpers;
using affolterNET.TextExtractor.Core.Interfaces;
using affolterNET.TextExtractor.Core.Pipeline.Core;
using affolterNET.TextExtractor.Core.Pipeline.Steps;
using affolterNET.TextExtractor.Core.Services;
using affolterNET.TextExtractor.Terminal.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace affolterNET.TextExtractor.Terminal.Commands;

public class ExtractJsonParallelCommand : AsyncCommand<ExtractJsonParallelCommand.Settings>
{
    private readonly ServiceFactory _serviceFactory;
    private IOutput _log;

    public class Settings : CommandSettings
    {
        [CommandOption("-d|--dir")]
        [DefaultValue("/Users/martin/Source/SocialContractData/AuswahlZollrecht")]
        public string? FolderName { get; set; }

        [CommandOption("-b |--batch-size")]
        [DefaultValue(2)]
        public int BatchSize { get; set; } = 10;

        public override ValidationResult Validate()
        {
            if (!string.IsNullOrWhiteSpace(FolderName) && !Directory.Exists(FolderName))
            {
                return ValidationResult.Error($"folder does not exist: {FolderName}");
            }

            return base.Validate();
        }
    }

    public ExtractJsonParallelCommand(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            var tasks = new List<Task>();
            var fileEnumerable = settings.FolderName!.GetFilesInAllDirectories().ToList();
            foreach (var fi in fileEnumerable)
            {
                var pipeline = _serviceFactory.CreatePipeline(fi.FullName);
                _log = _serviceFactory.GetService<IOutput>();
                tasks.Add(Task.Run(async () =>
                {
                    var serializeToBlobStorageStep = _serviceFactory.GetService<SerializeToBlobStorageStep>();
                    pipeline.AddStep(serializeToBlobStorageStep);
                    var pipelineContext = new PipelineContext(fi.FullName);
                    LogFileName($"Parsing file: {fi.FullName}");
                    await pipeline.Execute(pipelineContext);
                }));
                if (tasks.Count >= settings.BatchSize)
                {
                    await ExecuteTasks(tasks);
                }
            }
            
            await ExecuteTasks(tasks);
        }
        catch (Exception ex)
        {
            _log.WriteException(ex);
            return -1;
        }

        return 0;
    }

    private async Task ExecuteTasks(List<Task> tasks)
    {
        _log.Write(EnumLogLevel.Debug, $"Waiting for {tasks.Count} tasks to finish...");
        await Task.WhenAll(tasks);
        _log.Write(EnumLogLevel.Debug, $"{tasks.Count} tasks finished");
        tasks.Clear();
    }

    private void LogFileName(string msg)
    {
        var line = string.Empty.PadRight(msg.Length, '-');
        _log.Write(EnumLogLevel.Info, line);
        _log.Write(EnumLogLevel.Info, msg);
        _log.Write(EnumLogLevel.Info, line);
    }
}