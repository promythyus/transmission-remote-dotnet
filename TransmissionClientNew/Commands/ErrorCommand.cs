﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TransmissionClientNew.Commmands
{
    public class ErrorCommand : TransmissionCommand
    {
        private static readonly int MAX_MESSAGE_LENGTH = 300;
        private string title;
        private string body;

        public ErrorCommand(string title, string body)
        {
            this.title = title;
            this.body = body;
        }

        public ErrorCommand(Exception ex)
        {
            this.title = ex.GetType().ToString();
            this.body = ex.Message;
        }

        private delegate void ExecuteDelegate();
        public void Execute()
        {
            Form1 form = Program.form;
            if (form.InvokeRequired)
            {
                form.Invoke(new ExecuteDelegate(this.Execute));
            }
            else
            {
                Program.uploadArgs = null;
                if (!Program.Connected)
                {
                    form.toolStripStatusLabel1.Text = "Unable to connect (" + this.title + ")";
                    MessageBox.Show(this.body, this.title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (++Program.failCount > LocalSettingsSingleton.Instance.retryLimit)
                {
                    Program.Connected = false;
                    form.toolStripStatusLabel1.Text = "Disconnected. Exceeded maximum number of failed requests.";
                    MessageBox.Show(this.body.Length > MAX_MESSAGE_LENGTH ? this.body.Substring(0, MAX_MESSAGE_LENGTH) : this.body, this.title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    form.toolStripStatusLabel1.Text = "Failed request #" + Program.failCount + ": " + this.title;
                }
            }
        }
    }
}
