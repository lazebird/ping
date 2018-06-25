﻿using System;
using System.Collections;
using System.Windows.Forms;

namespace rabbit
{
    public partial class Form1 : Form
    {
        myping ping;
        httpd httpd;
        mylog pinglog;
        mylog httpdlog;
        Hashtable texthash;
        Hashtable btnhash;
        Hashtable formhash;
        public Form1()
        {
            InitializeComponent();
            AcceptButton = btn_ping;
            //CancelButton = Application.Exit(0);
            CheckForIllegalCrossThreadCalls = false;
            pinglog = new mylog(ping_output);
            httpdlog = new mylog(httpd_output);
            ping = new myping(pinglog, ping_stop_cb);
            httpd = new httpd(httpdlog);
            init_elements();
            readconf();
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
        public void ping_stop_cb()
        {
            ((Form)formhash["form"]).Text = "Rabbit";
            ((Button)btnhash["ping_btn"]).Text = "开始";
        }
        private void init_elements()
        {
            texthash = new Hashtable();
            btnhash = new Hashtable();
            formhash = new Hashtable();
            texthash.Add("ping_addr", text_addr);
            texthash.Add("ping_timeout", text_interval);
            texthash.Add("ping_times", text_count);
            texthash.Add("ping_logfile", text_logpath);
            texthash.Add("http_port", text_port);
            texthash.Add("http_dir", text_dir);
            btnhash.Add("ping_btn", btn_ping);
            btnhash.Add("httpd_btn", btn_httpd);
            formhash.Add("form", this);
        }
        private void readconf()
        {
            foreach (string key in texthash.Keys)
            {
                ((TextBox)texthash[key]).Text = myconf.read(key);
            }
        }
        private void saveconf()
        {
            foreach (string key in texthash.Keys)
            {
                myconf.write(key, ((TextBox)texthash[key]).Text);
            }
        }
        private void button1_Click(object sender, EventArgs evt)
        {
            if (((Button)btnhash["ping_btn"]).Text == "开始")
            {
                try
                {
                    saveconf(); // save empty config to restore default config
                    ((Form)formhash["form"]).Text = ((TextBox)texthash["ping_addr"]).Text;
                    ((Button)btnhash["ping_btn"]).Text = "停止";
                    pinglog.setfile(text_logpath.Text);
                    pinglog.clear();
                    ping.start(((TextBox)texthash["ping_addr"]).Text, int.Parse(((TextBox)texthash["ping_timeout"]).Text), int.Parse(((TextBox)texthash["ping_times"]).Text));
                }
                catch (Exception e)
                {
                    pinglog.write("Error: " + e.Message);
                }
            }
            else
            {
                // stop
                ping.stop();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog filename = new SaveFileDialog();
            //filename.InitialDirectory = Application.StartupPath;
            filename.Filter = "文本文件 (*.txt)|*.txt|All files (*.*)|*.*";
            filename.RestoreDirectory = true;
            filename.CreatePrompt = true;
            filename.OverwritePrompt = true;
            if (filename.ShowDialog() == DialogResult.OK)
            {
                ((TextBox)texthash["ping_logfile"]).Text = filename.FileName;
            }
        }
        private void httpd_click(object sender, EventArgs evt)
        {
            try
            {
                if (((Button)btnhash["httpd_btn"]).Text == "开始")
                {
                    saveconf(); // save empty config to restore default config
                    ((Button)btnhash["httpd_btn"]).Text = "停止";
                    httpd.start(int.Parse(((TextBox)texthash["http_port"]).Text));
                }
                else
                {
                    ((Button)btnhash["httpd_btn"]).Text = "开始";
                    httpd.stop();
                }
            }
            catch (Exception e)
            {
                ((Button)btnhash["httpd_btn"]).Text = "开始";
                httpdlog.write("Error: " + e.Message);
            }
        }
    }
}