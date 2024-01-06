
namespace Trivial
{
    partial class FormConsultas
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
            this.infoLabel = new System.Windows.Forms.Label();
            this.respGridView = new System.Windows.Forms.DataGridView();
            this.askLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.respGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.Location = new System.Drawing.Point(40, 74);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(0, 20);
            this.infoLabel.TabIndex = 0;
            // 
            // respGridView
            // 
            this.respGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.respGridView.Location = new System.Drawing.Point(44, 132);
            this.respGridView.Name = "respGridView";
            this.respGridView.RowHeadersWidth = 51;
            this.respGridView.RowTemplate.Height = 24;
            this.respGridView.Size = new System.Drawing.Size(479, 196);
            this.respGridView.TabIndex = 1;
            // 
            // askLbl
            // 
            this.askLbl.AutoSize = true;
            this.askLbl.Font = new System.Drawing.Font("Times New Roman", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.askLbl.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.askLbl.Location = new System.Drawing.Point(39, 37);
            this.askLbl.Name = "askLbl";
            this.askLbl.Size = new System.Drawing.Size(70, 25);
            this.askLbl.TabIndex = 2;
            this.askLbl.Text = "label1";
            // 
            // RespuestaConsultas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGreen;
            this.ClientSize = new System.Drawing.Size(593, 365);
            this.Controls.Add(this.askLbl);
            this.Controls.Add(this.respGridView);
            this.Controls.Add(this.infoLabel);
            this.Name = "RespuestaConsultas";
            this.Text = "RespuestaConsultas";
            ((System.ComponentModel.ISupportInitialize)(this.respGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.DataGridView respGridView;
        private System.Windows.Forms.Label askLbl;
    }
}