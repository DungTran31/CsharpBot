using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace CsharpBot.Commands.Slash
{
    public class BasicSL : ApplicationCommandModule
    {
        [SlashCommand("test", "This is my first slash command")]
        public async Task MyFirstSlashCommand(InteractionContext ctx)
        {
            // await ctx.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Hello World"));
            // Solution below better
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Blue,
                Title = "Hello World"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }

        [SlashCommand("parameters", "This slash command allows parameters")]
        public async Task SlashCommandParameters(InteractionContext ctx, [Option("testoption", "Type in anything")] string stringParameter, [Option("numberoption", "Type in a number")] long numberParameter)
        {
            await ctx.DeferAsync();

            var embedMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Brown,
                Title = "Test Embed",
                Description = $"{stringParameter} {numberParameter}"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }

        [SlashCommand("discordParameters", "This slash command allows passing of DiscordParameters")]
        public async Task DiscordParameters(InteractionContext ctx, [Option("user", "Pass in a Discord User")] DiscordUser user, [Option("file", "Upload a file here")] DiscordAttachment file)
        {
            await ctx.DeferAsync();

            var member = (DiscordMember)user;

            var embedMessage = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Blue,
                Title = "Test Embed",
                Description = $"{member.Nickname} {file.FileName} {file.FileSize}"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedMessage));
        }
    }
}
