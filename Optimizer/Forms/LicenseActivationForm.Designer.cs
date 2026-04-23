namespace Optimizer
{
    partial class LicenseActivationForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Name = "LicenseActivationForm";
            this.Text = "License Activation";
            this.Load += new System.EventHandler(this.LicenseActivationForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LicenseActivationForm_FormClosing);
            this.ResumeLayout(false);
        }
    }
}
