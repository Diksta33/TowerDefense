namespace TowerDefense
{
    partial class Game
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
            this.Grid = new System.Windows.Forms.PictureBox();
            this.Legend = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Legend)).BeginInit();
            this.SuspendLayout();
            // 
            // Grid
            // 
            this.Grid.BackColor = System.Drawing.Color.Transparent;
            this.Grid.Location = new System.Drawing.Point(12, 12);
            this.Grid.Name = "Grid";
            this.Grid.Size = new System.Drawing.Size(1015, 601);
            this.Grid.TabIndex = 1;
            this.Grid.TabStop = false;
            this.Grid.Paint += new System.Windows.Forms.PaintEventHandler(this.Grid_Paint);
            this.Grid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseClick);
            this.Grid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseMove);
            // 
            // Legend
            // 
            this.Legend.Location = new System.Drawing.Point(1033, 12);
            this.Legend.Name = "Legend";
            this.Legend.Size = new System.Drawing.Size(175, 515);
            this.Legend.TabIndex = 2;
            this.Legend.TabStop = false;
            this.Legend.Paint += new System.Windows.Forms.PaintEventHandler(this.Legend_Paint);
            this.Legend.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Legend_MouseClick);
            // 
            // Game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1221, 771);
            this.Controls.Add(this.Legend);
            this.Controls.Add(this.Grid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Game";
            this.Text = "Game";
            this.Load += new System.EventHandler(this.Game_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Legend)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox Grid;
        private System.Windows.Forms.PictureBox Legend;
    }
}