using System.Collections.Generic;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace ScpProxyChat
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string ProxyChatEnabled { get; set; } = "<b>Proxy чат <color=green>включен</color>!</b>";
        public string ProxyChatDisabled { get; set; } = "<b>Proxy чат <color=red>отключен</color>!</b>";
        public float MaxDistance { get; set; } = 10f;
        public List<RoleTypeId> AllowedRoles { get; set; } = new List<RoleTypeId>()
        {
            RoleTypeId.Scp049,
            RoleTypeId.Scp096,
            RoleTypeId.Scp106,
            RoleTypeId.Scp173,
            RoleTypeId.Scp0492,
            RoleTypeId.Scp939,
            RoleTypeId.Scp3114
        };
    }
}