﻿// 
// SoundSense C# Port aka SoundCenSe
// 
// Solution: SoundCenSe
// Project: SoundCenSe
// File: SoundCenSeForm.cs
// 
// Last modified: 2016-07-30 19:37

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using FMOD;
using NLog;
using SoundCenSe.Configuration.Sounds;
using SoundCenSe.Enums;
using SoundCenSe.Events;
using SoundCenSe.fmodInternal;
using SoundCenSe.Input;
using SoundCenSe.Output;
using SoundCenSe.Utility;
using SoundCenSe.Utility.Updater;
using Sound = SoundCenSe.Configuration.Sounds.Sound;

namespace SoundCenSe
{
    public partial class SoundCenSeForm : Form
    {
        #region Fields and Constants

        private SoundsXML allSounds;

        private bool closing;

        readonly List<string> DebugGamelog = new List<string>();
        private int DebugGamelogIndex;
        private readonly DwarfFortressAware DF;
        private LogFileListener LL;

        private readonly Queue<string> PackDownloaderMessageQueue = new Queue<string>();

        private SoundProcessor SP;

        private readonly bool startingUp = true;

        private bool stopLongtimeDebug;

        readonly List<Control> wereEnabled = new List<Control>();

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        #endregion

        public SoundCenSeForm()
        {
            InitializeComponent();

            // Load Configuration
            Config.Load(@"Configuration.json");

            DF = new DwarfFortressAware();
            DF.DwarfFortressRunning += DFRunning;
            DF.DwarfFortressStopped += DFStopped;
            fmodPlayer.Instance.SoundPlaying += Playing;
            DF.Start();

            soundPanel.Muting += Muting;
            soundPanel.FastForward += FastForward;
            soundPanel.VolumeChanged += VolumeChanged;
            soundPanel.SoundDisabled += SoundDisabled;
            FillDisabledSounds();

            FillThresholdCombo();
#if __MonoCS__
#else
            //lbGamelogpath.Visible = false;
            //btnSelectGamelogPath.Visible = false;
            //tbGamelogPath.Visible = false;
#endif

            cbDeleteFiles.Checked = Config.Instance.deleteFiles;
            cbOverwriteFiles.Checked = Config.Instance.replaceFiles;
            tbSoundPackPath.Text = Config.Instance.soundpacksPath;

            this.Text = "SoundCenSe " +
                        string.Join(".",
                            FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)
                                .FileVersion.Split('.')
                                .Take(3)
                                .ToArray());

            Tabs.TabPages.Remove(tabDebug);
            startingUp = false;
        }


        private void AddProgress(long expectedSize)
        {
            toolStripProgressBar1.Value += (int) expectedSize;
        }

        private void btnDebug1_Click(object sender, EventArgs e)
        {
            DF.Stop();
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Config.Instance.gamelogPath);
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                GC.Collect();
                MessageBox.Show("Starting now");
                allSounds = new SoundsXML(Config.Instance.soundpacksPath);
                SP = new SoundProcessor(allSounds);
                FillChannelEntries();

                string line = "";
                TextReader tr = new StreamReader(openFileDialog1.FileName);
                while ((line = tr.ReadLine()) != null)
                {
                    DebugGamelog.Add(line);
                }
                tr.Close();
                while (!stopLongtimeDebug)
                {
                    SP.ProcessLine(this, new GamelogEventArgs(DebugGamelog[DebugGamelogIndex++]));
                    return;
                    if (DebugGamelogIndex == DebugGamelog.Count)
                    {
                        DebugGamelogIndex = 0;
                    }
                    button1.Text = DebugGamelogIndex.ToString();
                    Application.DoEvents();
                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                    Application.DoEvents();
                    Thread.Sleep(50);
                }
                SP = null;
                allSounds.Dispose();
                GC.Collect();
                MessageBox.Show("Stopping now");
            }
        }

        private void btnUpdateClick(object sender, EventArgs e)
        {
            DFStopped(this, new DwarfFortressStoppedEventArgs());
            DF.Stop();
            listBoxUpdateMessages.Items.Clear();
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = 1;
            EnableControls(false);

            PackDownloader PD = new PackDownloader();
            PD.FinishedFile += FinishedSingleUpdateFile;
            PD.UpdateFinished += UpdateFinished;
            PD.DownloadStarted += DownloadStarted;
            PD.UpdateSoundPack();

            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = PD.Count();
            PD.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DF.Stop();
            DFStopped(this, new DwarfFortressStoppedEventArgs());
            Application.DoEvents();
            browseSoundpackPath.SelectedPath = Path.GetFullPath(Config.Instance.soundpacksPath);
            if (browseSoundpackPath.ShowDialog() == DialogResult.OK)
            {
                if (browseSoundpackPath.SelectedPath != Config.Instance.soundpacksPath)
                {
                    Config.Instance.soundpacksPath = browseSoundpackPath.SelectedPath;
                    tbSoundPackPath.Text = Config.Instance.soundpacksPath;
                }
            }
            DF.Start();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            stopLongtimeDebug = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(openFileDialog1.FileName))
                {
                    if (Path.GetFileName(openFileDialog1.FileName) == "gamelog.txt")
                    {
                        tbGamelogPath.Text = openFileDialog1.FileName;
                        Config.Instance.gamelogPath = openFileDialog1.FileName;
                        DF.Start();
                    }
                }
            }
        }

        private void cbDeleteFilesCheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.deleteFiles = cbDeleteFiles.Checked;
        }

        private void cbOverwriteFilesCheckedChanged(object sender, EventArgs e)
        {
            Config.Instance.replaceFiles = cbOverwriteFiles.Checked;
        }

        private void comboBoxThresholdSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!startingUp)
            {
                Threshold selected;
                Enum.TryParse(comboBoxThreshold.SelectedValue.ToString(), out selected);
                Config.Instance.playbackThreshold = selected;
                fmodPlayer.Instance.Threshold = selected;
            }
        }


        public void DFRunning(object sender, DwarfFortressRunningEventArgs e)
        {
            if (closing)
            {
                return;
            }
            allSounds = new SoundsXML(Config.Instance.soundpacksPath);
            SetDisabledSounds();
            FillChannelEntries();

            if (LL != null)
            {
                LL.Stop();
                LL.Dispose();
            }

            Config.Instance.gamelogPath = DF.GameLogPath;
#if __MonoCS__
            if (!File.Exists(Config.Instance.gamelogPath))
            {
                Status("Dwarf Fortress gamelog not found.");
                Tabs.SelectedTab = tabPageConfiguration;
                return;
            }
#endif
            Dictionary<string, Sound> oldMusics = GetOldMusic(allSounds);
            allSounds = new SoundsXML(Config.Instance.soundpacksPath);
            SP = new SoundProcessor(allSounds);
            SetDisabledSounds();
            if (oldMusics.ContainsKey("music"))
            {
                fmodPlayer.Instance.Play(oldMusics["music"], 0, 0, 0);
            }
            if (oldMusics.ContainsKey("weather"))
            {
                fmodPlayer.Instance.Play(oldMusics["weather"], 0, 0, 0);
            }

            LL = new LogFileListener(DF.GameLogPath, true);
            LL.GamelogEvent += SP.ProcessLine;
            LL.GamelogEvent += ShowLogInStatus;
            //LL.BeginAtEnd();
            LL.Start();
            this.InvokeIfRequired(() =>
            {
                toolStripSignal1.Signal = true;
                Status("Connected to Dwarf Fortress in " + Path.GetDirectoryName(DF.GameLogPath));
                btnUpdate.Enabled = false;
            });
        }

        public void DFStopped(object sender, DwarfFortressStoppedEventArgs e)
        {
            if (closing)
            {
                return;
            }
            if (LL != null)
            {
                LL.Dispose();
            }
            LL = null;
            SP = null;
            fmodPlayer.Instance.StopAll();
            this.InvokeIfRequired(() =>
            {
                toolStripSignal1.Signal = false;
                btnUpdate.Enabled = true;
                soundPanel.Clear();
            });
        }

        private void DownloadStarted(object sender, StartDownloadEventArgs e)
        {
            lock (PackDownloaderMessageQueue)
            {
                PackDownloaderMessageQueue.Enqueue("Download of " + Path.GetFileName(e.File.SourceURL) + " started");
            }
        }

        /// <summary>
        ///     Enable or disable all Controls
        /// </summary>
        /// <param name="enable">Whether enable (true) or disable (false)</param>
        private void EnableControls(bool enable)
        {
            if (enable == false)
            {
                wereEnabled.Clear();
            }
            this.InvokeIfRequired(() =>
            {
                foreach (Control c in this.Controls)
                {
                    if (!enable && c.Enabled)
                    {
                        wereEnabled.Add(c);
                        c.Enabled = enable;
                    }
                    else
                    {
                        if (wereEnabled.Contains(c))
                        {
                            c.Enabled = enable;
                        }
                    }
                }
            });
        }

        private void FastForward(object sender, ChannelFastForwardEventArgs channelFastForwardEventArgs)
        {
            if (closing)
            {
                return;
            }
            fmodPlayer.Instance.FastForward(channelFastForwardEventArgs.Channel);
        }

        private void FillChannelEntries()
        {
            List<string> channelNames = new List<string>();
            channelNames.Add("SFX");
            foreach (Sound s in allSounds.Sounds)
            {
                if (channelNames.Contains(s.Channel))
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(s.Channel))
                {
                    channelNames.Add(s.Channel);
                }
            }

            this.InvokeIfRequired(() =>
            {
                soundPanel.Clear();
                soundPanel.FillEntries(channelNames);
            });
            Tabs.InvokeIfRequired(() => { Tabs.SelectedTab = tabPageAudioControl; });
        }

        private void FillDisabledSounds()
        {
            foreach (string disabled in Config.Instance.disabledSounds)
            {
                lbDisabledSounds.Items.Add(disabled);
            }
        }

        private void FillThresholdCombo()
        {
            comboBoxThreshold.DataSource = Enum.GetValues(typeof(Threshold));
            comboBoxThreshold.SelectedItem = Config.Instance.playbackThreshold;
        }


        private void FinishedSingleUpdateFile(object sender, DownloadFinishedEventArgs downloadFinishedEventArgs)
        {
            lock (PackDownloaderMessageQueue)
            {
                PackDownloaderMessageQueue.Enqueue("Downloaded " +
                                                   Path.GetFileName(downloadFinishedEventArgs.File.DestinationPath));
            }
            this.InvokeIfRequired(
                () =>
                {
                    Status("Downloaded " + Path.GetFileName(downloadFinishedEventArgs.File.DestinationPath));
                    AddProgress(downloadFinishedEventArgs.File.ExpectedSize);
                });
        }

        private Dictionary<string, Sound> GetOldMusic(SoundsXML allSounds)
        {
            DummyPlayerManager dpm = new DummyPlayerManager();
            DummySoundProcessor sp = new DummySoundProcessor(allSounds);
            sp.DummyPlayerManager = dpm;

            FileStream fs = new FileStream(Config.Instance.gamelogPath, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
            TextReader tr = new StreamReader(fs);

            List<string> lines = new List<string>();

            string line = "";
            while ((line = tr.ReadLine()) != null)
            {
                if (line == "*** STARTING NEW GAME ***")
                {
                    lines.Clear();
                }
                lines.Add(line);
            }

            foreach (string l in lines)
            {
                sp.ProcessLine(this, new GamelogEventArgs(l));
            }
            return dpm.Channels;
        }

        private void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var index = lbDisabledSounds.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    lbDisabledSounds.SelectedIndex = index;
                }
            }
        }

        private void Muting(object sender, ChannelMuteEventArgs channelMuteEventArgs)
        {
            if (closing)
            {
                return;
            }
            string ch = channelMuteEventArgs.Channel.ToLower().StartsWith("sfx")
                ? "sfx"
                : channelMuteEventArgs.Channel.ToLower();
            fmodPlayer.Instance.MuteChannel(ch, channelMuteEventArgs.Mute);
            Config.Instance.MuteChannel(ch, channelMuteEventArgs.Mute);
        }

        private void Playing(object sender, SoundPlayingEventArgs soundPlayingEventArgs)
        {
            if (closing)
            {
                return;
            }
            string channel = soundPlayingEventArgs.Sound.Channel.Capitalize();
            if (string.IsNullOrEmpty(channel))
            {
                channel = "SFX";
            }

            string soundFile = soundPlayingEventArgs.SoundFile.Filename;
            this.InvokeIfRequired(
                () =>
                {
                    soundPanel.SetValues(channel, soundFile, soundPlayingEventArgs.SoundFile.Length,
                        soundPlayingEventArgs.Mute,
                        soundPlayingEventArgs.Volume, soundPlayingEventArgs.Sound.loop == Loop.Start_Looping);
                });
        }

        private void reenableSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file = lbDisabledSounds.SelectedItem.ToString();
            if (Config.Instance.disabledSounds.Contains(file))
            {
                lbDisabledSounds.Items.Remove(file);
                Config.Instance.disabledSounds.Remove(file);
                if (allSounds != null)
                {
                    foreach (Sound s in allSounds.Sounds)
                    {
                        foreach (SoundFile sf in s.SoundFiles)
                        {
                            if (sf.Filename == file)
                            {
                                sf.Disabled = false;
                            }
                        }
                    }
                }
            }
        }

        private void SetDisabledSounds()
        {
            foreach (Sound s in allSounds.Sounds)
            {
                foreach (SoundFile sf in s.SoundFiles)
                {
                    foreach (string disabledSound in Config.Instance.disabledSounds)
                    {
                        if (sf.Filename == disabledSound)
                        {
                            sf.Disabled = true;
                        }
                    }
                }
            }
        }

        private void ShowLogInStatus(object sender, GamelogEventArgs gamelogEventArgs)
        {
            this.InvokeIfRequired(() => Status(gamelogEventArgs.Line));
        }

        private void SoundCenSeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DF != null)
            {
                DF.Stop();
            }
            if (LL != null)
            {
                LL.Stop();
            }
            Config.Save("Configuration.json");
        }

        private void SoundCenSeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            closing = true;
            e.Cancel = false;
        }

        private void SoundDisabled(object sender, DisableSoundEventArgs disableSoundEventArgs)
        {
            if (!Config.Instance.disabledSounds.Contains(disableSoundEventArgs.Filename))
            {
                lbDisabledSounds.Items.Add(disableSoundEventArgs.Filename);
                Config.Instance.disabledSounds.Add(disableSoundEventArgs.Filename);
                foreach (Sound s in allSounds.Sounds)
                {
                    foreach (SoundFile sf in s.SoundFiles)
                    {
                        if (sf.Filename == disableSoundEventArgs.Filename)
                        {
                            sf.Disabled = true;
                        }
                    }
                }
            }
        }

        private void Status(string line)
        {
            statusStrip.InvokeIfRequired(() =>
            {
                if (statusStrip.Visible && toolStripStatusLabel1.Visible)
                {
                    toolStripStatusLabel1.Text = line;
                }
            });
        }

        private void timer1Tick(object sender, EventArgs e)
        {
            soundPanel.Tick();
            lock (PackDownloaderMessageQueue)
            {
                while (PackDownloaderMessageQueue.Count > 0)
                {
                    listBoxUpdateMessages.Items.Add(PackDownloaderMessageQueue.Dequeue());
                }
            }
            int visibleItems = listBoxUpdateMessages.ClientSize.Height/listBoxUpdateMessages.ItemHeight;
            listBoxUpdateMessages.TopIndex = Math.Max(listBoxUpdateMessages.Items.Count - visibleItems + 1, 0);

            fmodChannelSound.ReleaseSounds();

            RESULT r = FmodSystem.System.update();
        }

        private void UpdateFinished(object sender, UpdateFinishedEventArgs updateFinishedEventArgs)
        {
            EnableControls(true);
            lock (PackDownloaderMessageQueue)
            {
                PackDownloaderMessageQueue.Enqueue("Update finished");
            }
            this.InvokeIfRequired(() => toolStripProgressBar1.Visible = false);
            DF.Start();
        }

        private void VolumeChanged(object sender, ChannelVolumeEventArgs channelVolumeEventArgs)
        {
            if (closing)
            {
                return;
            }
            string ch = channelVolumeEventArgs.Channel.ToLower().StartsWith("sfx")
                ? "sfx"
                : channelVolumeEventArgs.Channel.ToLower();
            fmodPlayer.Instance.VolumeChannel(ch, channelVolumeEventArgs.Volume);
            Config.Instance.SetChannelVolume(ch, channelVolumeEventArgs.Volume);
        }
    }
}