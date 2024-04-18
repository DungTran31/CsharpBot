using System;
using System.Threading.Tasks;
using CsharpBot.commands;
using CsharpBot.config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace CsharpBot
{
    //Make sure the Program class is sealed so that nothing can inherit it
    public sealed class Program
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        static async Task Main(string[] args)
        {
            //1. Retrieve Token/Prefix from config.json
            var configJsonFile = new JSONReader();
            await configJsonFile.ReadJSON();

            // 2. Setting up the Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            // 3. Apply this config to our DiscordClient
            Client = new DiscordClient(discordConfig);

            // 4. Set the default timeout for Commands that use interactivity
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            // 5. Set up the Ready Event-Handler
            // Event-Handlers for the Client go here
            Client.Ready += Client_Ready;
            // Client.MessageCreated += MessageCreatedHandler;
            Client.VoiceStateUpdated += VoiceChannelHandler;

            // 6. Set up the Commands Configuration
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJsonFile.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.CommandErrored += CommandHandler;

            // 7. Register your commands
            Commands.RegisterCommands<Basic>();

            // 8. Connect to get the Bot online
            await Client.ConnectAsync();
            //Make sure you delay by -1 to keep the bot running forever
            await Task.Delay(-1);
        }

        private static async Task CommandHandler(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            if (e.Exception is ChecksFailedException exception)
            {
                string timeLeft = string.Empty;

                foreach (var check in exception.FailedChecks)
                {
                    var coolDown = (CooldownAttribute)check;
                    timeLeft = coolDown.GetRemainingCooldown(e.Context).ToString(@"hh\:mm\:ss");
                }

                var coolDownMessage = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Title = "Please wait for the cooldown to end",
                    Description = $"Time: {timeLeft}"
                };

                await e.Context.Channel.SendMessageAsync(embed: coolDownMessage);
            }
        }

        private static async Task VoiceChannelHandler(DiscordClient sender, VoiceStateUpdateEventArgs e)
        {
            if (e.Before == null && e.Channel.Name == "Chung")
            {
                await e.Channel.SendMessageAsync($"{e.User.Username} joined the Voice channel");
            }
        }

        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            await e.Channel.SendMessageAsync("This event handler was triggered");
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
