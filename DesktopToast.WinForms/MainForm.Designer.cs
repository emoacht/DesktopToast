namespace DesktopToast.WinForms
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Button_ShowToast = new System.Windows.Forms.Button();
            this.TextBox_ToastResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Button_ShowToast
            // 
            this.Button_ShowToast.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button_ShowToast.Location = new System.Drawing.Point(0, 0);
            this.Button_ShowToast.Name = "Button_ShowToast";
            this.Button_ShowToast.Size = new System.Drawing.Size(304, 30);
            this.Button_ShowToast.TabIndex = 0;
            this.Button_ShowToast.Text = "Show a toast";
            this.Button_ShowToast.UseVisualStyleBackColor = true;
            this.Button_ShowToast.Click += new System.EventHandler(this.Button_ShowToast_Click);
            // 
            // TextBox_ToastResult
            // 
            this.TextBox_ToastResult.BackColor = System.Drawing.Color.White;
            this.TextBox_ToastResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBox_ToastResult.Cursor = System.Windows.Forms.Cursors.Default;
            this.TextBox_ToastResult.Location = new System.Drawing.Point(1, 30);
            this.TextBox_ToastResult.Name = "TextBox_ToastResult";
            this.TextBox_ToastResult.ReadOnly = true;
            this.TextBox_ToastResult.Size = new System.Drawing.Size(302, 27);
            this.TextBox_ToastResult.TabIndex = 1;
            this.TextBox_ToastResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(304, 61);
            this.Controls.Add(this.TextBox_ToastResult);
            this.Controls.Add(this.Button_ShowToast);
            this.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "DesktopToast WinForms Sample";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_ShowToast;
        private System.Windows.Forms.TextBox TextBox_ToastResult;
    }
}

