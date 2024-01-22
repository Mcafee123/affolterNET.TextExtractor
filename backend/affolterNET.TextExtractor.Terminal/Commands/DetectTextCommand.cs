using System.ComponentModel;
using Spectre.Console.Cli;

namespace affolterNET.TextExtractor.Terminal.Commands;

public class DetectTextCommand: AsyncCommand<DetectTextCommand.Settings>
{
    public class Settings : CommandSettings
    {
        public Settings(string? connString = null)
        {
            ConnString = connString ?? string.Empty;
        }

        [CommandOption("-c|--connectionstring")]
        [DefaultValue("")]
        public string ConnString { get; set; }
    }

    public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        throw new NotImplementedException();
    }
}