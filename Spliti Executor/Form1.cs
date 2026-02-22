using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spliti_Executor
{
    public partial class Form1 : Form
    {
        // Add missing fields for designer controls
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.RichTextBox consoleBox;

        // Add missing field for webView21
        private void InitializeWebView21()
        {
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.webView21.Location = new System.Drawing.Point(12, 86);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(776, 262);
            this.webView21.TabIndex = 10;
            this.Controls.Add(this.webView21);
        }

        // Add toggle switches to the form in the constructor
        private void InitializeToggleSwitches()
        {
            this.guna2ToggleSwitch1 = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.guna2ToggleSwitch2 = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.guna2ToggleSwitch1.Location = new System.Drawing.Point(600, 370);
            this.guna2ToggleSwitch2.Location = new System.Drawing.Point(700, 370);
            this.guna2ToggleSwitch1.Name = "guna2ToggleSwitch1";
            this.guna2ToggleSwitch2.Name = "guna2ToggleSwitch2";
            this.guna2ToggleSwitch1.Size = new System.Drawing.Size(50, 25);
            this.guna2ToggleSwitch2.Size = new System.Drawing.Size(50, 25);
            this.guna2ToggleSwitch1.CheckedChanged += new System.EventHandler(this.guna2ToggleSwitch1_CheckedChanged);
            this.guna2ToggleSwitch2.CheckedChanged += new System.EventHandler(this.guna2ToggleSwitch2_CheckedChanged);
            this.Controls.Add(this.guna2ToggleSwitch1);
            this.Controls.Add(this.guna2ToggleSwitch2);
        }

        public Form1()
        {
            InitializeComponent();
            InitializeWebView21();
            InitializeToggleSwitches();
            this.consoleBox = this.richTextBox1;
        }
        private async void Attachwithapi()
        {

            if (RKOAPI_Xeno.RobloxUtils.IsRobloxRunning())
            {
                LogToConsole("Roblox Has been Detected", LogType.INFO);
                RKOAPI_Xeno.Modules.Inject();
                await ExecuteScriptAsync();

                LogToConsole("Sent Injection!", LogType.Success);
            }
            else
            {
                LogToConsole("Roblox not detected!", LogType.ERROR);
            }
        }





        private async Task CleaWhiteitor()
        {
            await webView21.CoreWebView2.ExecuteScriptAsync("window.celestiaEditor && window.celestiaEditor.clearText && window.celestiaEditor.clearText();");

        }

        private async Task SaveTextAsLua()
        {
            try
            {

                string jsResult = await webView21.CoreWebView2.ExecuteScriptAsync("window.celestiaEditor && window.celestiaEditor.editor && window.celestiaEditor.editor.getValue();");


                if (jsResult.Length >= 2 && jsResult[0] == '"' && jsResult[jsResult.Length - 1] == '"')
                {
                    jsResult = jsResult.Substring(1, jsResult.Length - 2);
                }

                string editorContent = jsResult
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\r")
                    .Replace("\\t", "\t")
                    .Replace("\\\"", "\"")
                    .Replace("\\\\", "\\");

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Lua Files (*.lua)|*.lua|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                    sfd.Title = "Save Script File";
                    sfd.FileName = "Script.lua";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, editorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadTextFromFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "All Files (*.*)|*.*|Lua Files (*.lua)|*.lua|Text Files (*.txt)|*.txt";
                ofd.Title = "Open Script File";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string fileContent = File.ReadAllText(ofd.FileName);

                        string escapedContent = fileContent
                            .Replace("\\", "\\\\")
                            .Replace("\"", "\\\"")
                            .Replace("\r", "")
                            .Replace("\n", "\\n");

                        if (webView21.CoreWebView2 != null)
                        {

                            await webView21.CoreWebView2.ExecuteScriptAsync($"window.celestiaEditor && window.celestiaEditor.setText && window.celestiaEditor.setText(\"{escapedContent}\");");
                        }
                        else
                        {
                            MessageBox.Show("Editor is not ready yet.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private async Task<string> GetEditorContent()
        {
            await webView21.EnsureCoreWebView2Async();
            string script = "monaco.editor.getModels()[0].getValue()";
            string result = await webView21.ExecuteScriptAsync(script);
            return JsonConvert.DeserializeObject<string>(result);
        }

        private async Task ExecuteScriptAsync()
        {
            string scriptcontent = await GetEditorContent();
            RKOAPI_Xeno.Modules.ExecuteScript(scriptcontent);

        }

        private async void Execute()
        {
            if (RKOAPI_Xeno.RobloxUtils.IsRobloxRunning())
            {


                if (RKOAPI_Xeno.Modules.InjectionCheck() == 1)
                {
                    await ExecuteScriptAsync();
                    LogToConsole("Sent Execution!", LogType.Success);
                }
                else if (RKOAPI_Xeno.Modules.InjectionCheck() == 1)
                {
                    LogToConsole("Not Injected!", LogType.ERROR);
                }
            }
            else
            {
                LogToConsole("Roblox Not Detected!", LogType.ERROR);
            }
        }

        // Add fields for toggle switches
        private Guna.UI2.WinForms.Guna2ToggleSwitch guna2ToggleSwitch1;
        private Guna.UI2.WinForms.Guna2ToggleSwitch guna2ToggleSwitch2;

        // Add event handlers for toggle switches
        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2ToggleSwitch1.Checked == true)
            {
                guna2ToggleSwitch2.Checked = false;
                File.WriteAllText("settings.txt", "Nezur = True\nXeno = False");
                LogToConsole("Switched To Nezur!", LogType.Success);
            }
        }
        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2ToggleSwitch2.Checked == true)
            {
                guna2ToggleSwitch1.Checked = false;
                File.WriteAllText("settings.txt", "Nezur = False\nXeno = True");
                LogToConsole("Switched To Xeno!", LogType.Success);
            }
        }

        // Add Xeno notifier and options setup to Startup
        private void SetupXenoOptions()
        {
            RKOAPI_Xeno.Modules.SetDefualtNotifier(true, "Title", "Text");
            RKOAPI_Xeno.Modules.UseAutoInject(true);
            RKOAPI_Xeno.Modules.UseExecutionNotifier();
            RKOAPI_Xeno.Modules.UseInjectionNotifier();
            RKOAPI_Xeno.Modules.UseCustomnNotifier("Title", "Text");
        }

        // Call SetupXenoOptions in Startup
        private async void Startup()
        {
            SetupXenoOptions();
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.Navigate(System.Windows.Forms.Application.StartupPath + "\\bin\\MonacoWithTabs\\monaco.html");
            try
            {
                string Exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string dir = Path.GetDirectoryName(Exe);
                var files = Directory.GetFiles(dir);
                string Executorn = Path.GetFileName(Exe);
                foreach (string file in files)
                {
                    string filename = Path.GetFileName(file);
                    if (filename.Equals(Executorn, StringComparison.OrdinalIgnoreCase))
                        continue;
                    try
                    {
                        File.SetAttributes(file, File.GetAttributes(file) | FileAttributes.Hidden);
                    }
                    catch { MessageBox.Show("Error on File configuration"); }
                }
            }
            catch { MessageBox.Show("Error on fetching files"); }
        }


        private void Minimize()
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void Exit()
        {
            System.Windows.Forms.Application.Exit();
        }

        public enum LogType
        {
            INFO,
            ERROR,
            Warning,
            Success
        }

        public static class ConsoleLogger
        {
            public static Color GetLogColor(LogType type)
            {
                if (type == LogType.INFO)
                    return Color.White;
                if (type == LogType.ERROR)
                    return Color.Red;
                if (type == LogType.Warning)
                    return Color.Yellow;
                if (type == LogType.Success)
                    return Color.Green;

                return Color.White;
            }
        }

        private void LogToConsole(string message, LogType type)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            Color logColor = ConsoleLogger.GetLogColor(type);

            consoleBox.SelectionStart = consoleBox.TextLength;
            consoleBox.SelectionLength = 0;


            consoleBox.SelectionColor = logColor;
            consoleBox.AppendText($"[ {type} ]");


            consoleBox.SelectionColor = Color.White;
            consoleBox.AppendText($" [{timestamp}] ");


            consoleBox.SelectionColor = logColor;
            consoleBox.AppendText($"{message}\n");

            consoleBox.ScrollToCaret();
        }

        // Add stubs for missing event handlers
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Minimize();
        }
        private void Attach_Click(object sender, EventArgs e)
        {
            Attachwithapi();
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Optional: Logging or further logic
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            CleaWhiteitor();
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Execute();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {


        }
    }
}
