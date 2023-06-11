using NetDiscordRpc.Core.Logger;
using NetDiscordRpc.RPC;
using NetDiscordRpc;
using Button = NetDiscordRpc.RPC.Button;

namespace OTFA
{
    class RPCDiscord
    {
        public static DiscordRPC DiscordRpc;
        internal static Timestamps RPCTimestamp = Timestamps.Now;

        public static void Init()
        {
            DiscordRpc = new DiscordRPC("1110137150919430155");   
            DiscordRpc.Initialize();
            DiscordRpc.SetPresence(new RichPresence()
            {
                Details = "Modifying my Windows Experience",
                State = "Browsing Modifications",
                Timestamps = RPCTimestamp,
                Assets = new Assets()
                {
                    LargeImageKey = "textlogo",
                    LargeImageText = "One Tool For All"
                },
                Buttons = new Button[]
                {
                    new Button() { Label = "Download", Url = "https://github.com/bonsall2004/One-Tool-For-All/releases" }
                }
            });
            DiscordRpc.Invoke();
            RPCDiscord.changeState("General");
            return;
        }

        public static void changeState(string state)
        {
            DiscordRpc.UpdateState("Editing "+state+" Settings");
            DiscordRpc.Invoke();
            return;
        }
    }
}
