﻿// 
// SoundSense C# Port aka SoundCenSe
// 
// Solution: SoundCenSe
// Project: SoundCenSe
// File: SoundPanel.cs
// 
// Last modified: 2016-07-30 19:37

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SoundCenSe.Configuration;
using SoundCenSe.Events;
using SoundCenSe.Utility;

namespace SoundCenSe.GUI
{
    public partial class SoundPanel : UserControl
    {
        #region Fields and Constants

        internal List<string> channelSort = new List<string>();

        public EventHandler<ChannelFastForwardEventArgs> FastForward;
        public EventHandler<ChannelMuteEventArgs> Muting;
        public EventHandler<DisableSoundEventArgs> SoundDisabled;
        public EventHandler<ChannelVolumeEventArgs> VolumeChanged;

        #endregion

        public SoundPanel()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            this.InvokeIfRequired(() => tablePanel.Controls.Clear());
        }

        private void FastForwardInternal(object sender, ChannelFastForwardEventArgs channelFastForwardEventArgs)
        {
            var handler = FastForward;
            if (handler != null)
            {
                handler(sender, channelFastForwardEventArgs);
            }
        }


        public void FillEntries(List<string> channelNames)
        {
            Clear();
            this.InvokeIfRequired(() =>
            {
                foreach (string s in channelNames)
                {
                    SoundPanelEntry spe = new SoundPanelEntry();
                    spe.ChannelName = s.Capitalize();
                    spe.IsSFXPanel = (string.Equals(s, "sfx", StringComparison.OrdinalIgnoreCase));

                    ChannelData cd = Config.Instance.Channels.FirstOrDefault(x => string.Equals(x.Channel, s, StringComparison.OrdinalIgnoreCase));
                    if (cd != null)
                    {
                        spe.VolumeBar.Value = (int) (cd.Volume*100);
                        spe.btnMute.Checked = cd.Mute;
                    }


                    spe.FastForward += FastForwardInternal;
                    spe.Muting += MutingInternal;
                    spe.VolumeChanged += VolumeChangedInternal;
                    spe.SoundDisabled += SoundDisabledInternal;
                    
                    tablePanel.Controls.Add(spe);
                    
                     //spe.Width = this.tablePanel.Width;
                     //spe.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                   spe.Dock = DockStyle.Fill;

                }
            });
        }


        private void MutingInternal(object sender, ChannelMuteEventArgs channelMuteEventArgs)
        {
            var handler = Muting;
            if (handler != null)
            {
                handler(sender, channelMuteEventArgs);
            }
        }


        public void SetValues(string channel, string file, uint length, bool mute, float volume, bool looping)
        {
            SoundPanelEntry spe =
                tablePanel.Controls.OfType<SoundPanelEntry>().FirstOrDefault(x => x.ChannelName == channel);
            if (spe == null)
            {
                throw new Exception("Channel not initialized yet? (" + channel + ")");
            }
            spe.VolumeBar.Value = (int) (volume*100);
            spe.btnMute.Checked = mute;
            spe.btnFastForward.Visible = looping;
            if (spe.Filename != file)
            {
                spe.ChannelName = channel;
                spe.Filename = file;
                spe.Length = length;
            }
            spe.AddEntry(file);
        }

        private void SoundDisabledInternal(object sender, DisableSoundEventArgs disableSoundEventArgs)
        {
            var handler = SoundDisabled;
            if (handler != null)
            {
                handler(this, disableSoundEventArgs);
            }
        }

        public void Tick()
        {
            foreach (SoundPanelEntry s in tablePanel.Controls.OfType<SoundPanelEntry>())
            {
                s.Tick();
            }
        }

        private void VolumeChangedInternal(object sender, ChannelVolumeEventArgs channelVolumeEventArgs)
        {
            var handler = VolumeChanged;
            if (handler != null)
            {
                handler(sender, channelVolumeEventArgs);
            }
        }
    }
}