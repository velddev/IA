﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using IA.SDK.Interfaces;

namespace IA.SDK
{
    public class RuntimeUser : DiscordUser, IProxy<IUser>
    {
        private IUser user;

        public RuntimeUser()
        {
        }
        public RuntimeUser(IUser author)
        {
            user = author;
        }

        public override string AvatarUrl
        {
            get
            {
                return user.AvatarUrl;
            }
        }

        public override ulong Id
        {
            get
            {
                return user.Id;
            }
        }

        public override bool IsBot
        {
            get
            {
                return user.IsBot;
            }
        }

        public override string Username
        {
            get
            {
                return user.Username;
            }
        }

        public override string Discriminator
        {
            get
            {
                return user.Discriminator;
            }
        }

        public override string Mention
        {
            get
            {
                return user.Mention;
            }
        }

        public override List<ulong> RoleIds
        {
            get
            {
                return (user as IGuildUser).RoleIds.ToList();
            }
        }

        public override IDiscordGuild Guild
        {
            get
            {
                IGuildUser u = user as IGuildUser;

                if(u == null)
                {
                    return null;
                }

                return new RuntimeGuild(u.Guild);
            }
        }

        public override async Task Kick()
        {
            await (user as IGuildUser).KickAsync();
        }

        public override async Task Ban(IDiscordGuild g)
        {
            await (g as IProxy<IGuild>).ToNativeObject().AddBanAsync(user);
        }

        public override async Task SendFile(string path)
        {
            IDMChannel c = await user.CreateDMChannelAsync();

            await c.SendFileAsync(path);
        }

        public override async Task<IDiscordMessage> SendMessage(string message)
        {
            IDMChannel c = await user.CreateDMChannelAsync();

            RuntimeMessage m = new RuntimeMessage(await c.SendMessageAsync(message));
            return m;
        }
        public override async Task<IDiscordMessage> SendMessage(IDiscordEmbedBuilder embed)
        {
            IDMChannel c = await user.CreateDMChannelAsync();
            IMessage m = await c.SendMessageAsync("", false, (embed as IProxy<EmbedBuilder>).ToNativeObject());

            return new RuntimeMessage(m);
        }

        public override bool HasPermissions(IDiscordChannel channel, params DiscordGuildPermission[] permissions)
        {
            foreach (DiscordGuildPermission p in permissions)
            {
                GuildPermission newP = (GuildPermission)Enum.Parse(typeof(DiscordGuildPermission), p.ToString());

                if (!(user as IGuildUser).GuildPermissions.Has(newP))
                {
                    return false;
                }
            }
            return true;
        }

        public override async Task AddRoleAsync(IDiscordRole role)
        {
            await (user as IGuildUser).AddRolesAsync((role as IProxy<IRole>).ToNativeObject());
        }

        public override async Task RemoveRoleAsync(IDiscordRole role)
        {
            IRole r = (role as IProxy<IRole>).ToNativeObject();
            IGuildUser u = (user as IGuildUser);

            await u.RemoveRolesAsync(r);
        }

        public IUser ToNativeObject()
        {
            return user;
        }
    }
}
