﻿namespace SoundCenSe.GUI
{
    partial class SoundPanelEntry
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelFile = new System.Windows.Forms.Label();
            this.labelLength = new System.Windows.Forms.Label();
            this.labelChannel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnFastForward = new System.Windows.Forms.Button();
            this.btnMute = new System.Windows.Forms.CheckBox();
            this.VolumeBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.VolumeBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Length:";
            // 
            // labelFile
            // 
            this.labelFile.AutoSize = true;
            this.labelFile.Location = new System.Drawing.Point(63, 7);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(0, 13);
            this.labelFile.TabIndex = 2;
            // 
            // labelLength
            // 
            this.labelLength.AutoSize = true;
            this.labelLength.Location = new System.Drawing.Point(63, 23);
            this.labelLength.Name = "labelLength";
            this.labelLength.Size = new System.Drawing.Size(0, 13);
            this.labelLength.TabIndex = 3;
            // 
            // labelChannel
            // 
            this.labelChannel.AutoSize = true;
            this.labelChannel.Location = new System.Drawing.Point(63, 44);
            this.labelChannel.Name = "labelChannel";
            this.labelChannel.Size = new System.Drawing.Size(0, 13);
            this.labelChannel.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Channel:";
            // 
            // btnFastForward
            // 
            this.btnFastForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFastForward.Image = global::SoundCenSe.Properties.Resources.FastForward;
            this.btnFastForward.Location = new System.Drawing.Point(356, 3);
            this.btnFastForward.Name = "btnFastForward";
            this.btnFastForward.Size = new System.Drawing.Size(38, 38);
            this.btnFastForward.TabIndex = 6;
            this.btnFastForward.UseVisualStyleBackColor = true;
            this.btnFastForward.Click += new System.EventHandler(this.FastForwardClick);
            // 
            // btnMute
            // 
            this.btnMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMute.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnMute.AutoSize = true;
            this.btnMute.Image = global::SoundCenSe.Properties.Resources.Mute;
            this.btnMute.Location = new System.Drawing.Point(356, 44);
            this.btnMute.Name = "btnMute";
            this.btnMute.Size = new System.Drawing.Size(38, 38);
            this.btnMute.TabIndex = 8;
            this.btnMute.UseVisualStyleBackColor = true;
            this.btnMute.CheckedChanged += new System.EventHandler(this.MuteClick);
            // 
            // VolumeBar
            // 
            this.VolumeBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.VolumeBar.Location = new System.Drawing.Point(93, 3);
            this.VolumeBar.Maximum = 100;
            this.VolumeBar.Name = "VolumeBar";
            this.VolumeBar.Size = new System.Drawing.Size(257, 45);
            this.VolumeBar.TabIndex = 9;
            this.VolumeBar.TickFrequency = 10;
            this.VolumeBar.ValueChanged += new System.EventHandler(this.VolumeBarValueChanged);
            // 
            // SoundPanelEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.VolumeBar);
            this.Controls.Add(this.btnMute);
            this.Controls.Add(this.btnFastForward);
            this.Controls.Add(this.labelChannel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelLength);
            this.Controls.Add(this.labelFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "SoundPanelEntry";
            this.Size = new System.Drawing.Size(397, 90);
            ((System.ComponentModel.ISupportInitialize)(this.VolumeBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelFile;
        private System.Windows.Forms.Label labelLength;
        private System.Windows.Forms.Label labelChannel;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.CheckBox btnMute;
        internal System.Windows.Forms.TrackBar VolumeBar;
        internal System.Windows.Forms.Button btnFastForward;
    }
}
