using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System.Threading.Tasks;

namespace CsharpBot.commands
{
    public class Basic : BaseCommandModule
    {
        [Command("test")]
        [Cooldown(5, 10, CooldownBucketType.User)]
        public async Task TestCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Hello {ctx.User.Username}");
        }
    }
}
