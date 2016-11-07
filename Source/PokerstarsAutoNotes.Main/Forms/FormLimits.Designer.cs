namespace PokerstarsAutoNotes.Forms
{
    partial class FormLimits
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonStandard = new System.Windows.Forms.Button();
            this.textBoxZoom = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonLoad.Location = new System.Drawing.Point(184, 92);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(100, 32);
            this.buttonLoad.TabIndex = 0;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.ButtonLoadClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.Location = new System.Drawing.Point(25, 92);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 32);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // buttonStandard
            // 
            this.buttonStandard.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonStandard.Location = new System.Drawing.Point(25, 53);
            this.buttonStandard.Name = "buttonStandard";
            this.buttonStandard.Size = new System.Drawing.Size(259, 23);
            this.buttonStandard.TabIndex = 2;
            this.buttonStandard.Text = "Save Selection as Standard";
            this.buttonStandard.UseVisualStyleBackColor = true;
            this.buttonStandard.Click += new System.EventHandler(this.ButtonStandardClick);
            // 
            // textBoxZoom
            // 
            this.textBoxZoom.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBoxZoom.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxZoom.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxZoom.Location = new System.Drawing.Point(25, 5);
            this.textBoxZoom.Multiline = true;
            this.textBoxZoom.Name = "textBoxZoom";
            this.textBoxZoom.Size = new System.Drawing.Size(259, 42);
            this.textBoxZoom.TabIndex = 4;
            this.textBoxZoom.Text = "Note: Speed is the Fastplay, Pokerstars calls it Zoom, Partypoker calls it Fast-F" +
    "orward.";
            // 
            // FormLimits
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 136);
            this.Controls.Add(this.textBoxZoom);
            this.Controls.Add(this.buttonStandard);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonLoad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLimits";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Game Selection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonStandard;
        private System.Windows.Forms.TextBox textBoxZoom;

    }
}