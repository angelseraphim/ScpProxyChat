using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using VoiceChat.Networking;
using VoiceChat;
using System.Linq;
using System;

namespace ScpProxyChat
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "angelseraphim.";
        public override string Name => "ScpProxyChat";
        public override string Prefix => "ScpProxyChat";
        public override Version Version => new Version(1, 0, 0, 8);

        private readonly List<Player> ToggledPlayers = new List<Player>();

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;

            Exiled.Events.Handlers.Server.RestartingRound += OnRestartingRound;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;

            Exiled.Events.Handlers.Server.RestartingRound -= OnRestartingRound;
            base.OnDisabled();
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ToggledPlayers.Contains(ev.Player))
                ToggledPlayers.Remove(ev.Player);
        }

        private void OnRestartingRound() => ToggledPlayers.Clear();

        public void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (ev.Player.IsNoclipPermitted)
                return;

            if (!Config.AllowedRoles.Contains(ev.Player.Role.Type))
                return;

            if (ToggledPlayers.Contains(ev.Player))
            {
                ToggledPlayers.Remove(ev.Player);
                ev.Player.ShowHint(Config.ProxyChatDisabled, 4f);
            }
            else
            {
                ToggledPlayers.Add(ev.Player);
                ev.Player.ShowHint(Config.ProxyChatEnabled, 4f);
            }
        }

        public void OnVoiceChatting(VoiceChattingEventArgs ev)
        {
            if (ev.VoiceMessage.Channel != VoiceChatChannel.ScpChat)
                return;

            if (!ToggledPlayers.Contains(ev.Player))
                return;

            VoiceMessage voiceMessage = ev.VoiceMessage;
            foreach (Player player in Player.List.Where(p => !p.IsScp && !p.IsDead && Vector3.Distance(ev.Player.Position, p.Position) <= Config.MaxDistance))
            {
                voiceMessage.Channel = VoiceChatChannel.Proximity;
                player.Connection.Send(voiceMessage);
            }
            foreach (Player player in ev.Player.CurrentSpectatingPlayers)
            {
                voiceMessage.Channel = VoiceChatChannel.Proximity;
                player.Connection.Send(voiceMessage);
            }
        }
    }
}