using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Resources;
using System.Reflection;

namespace IQ_DescriptionDecoder
{
    public partial class Form1 : Form
    {

        XmlDocument doc = new XmlDocument();
        // make collection of tags to add tags found into
        Collection<IQ_X_CONTROL> Array_IQ_X_CONTROL = new Collection<IQ_X_CONTROL>();


        public Form1()
        {
            InitializeComponent();

            // instantiate XmlDocument and load XML from file

        }

        string pathFile, Outfolder, pathCSV;

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                pathFile = file.FileName;
                label4.Text = pathFile;
            }
        }

        public class IQ_X_CONTROL
        {
            public string Name { get; set; }
            public string DescPLC { get; set; }
            public string DataType { get; set; }

            public IQ_X_CONTROL() { }
            public IQ_X_CONTROL(string nameIn, string descPLCIn, string dataTypeIn)
            {
                Name = nameIn;
                DescPLC = descPLCIn;
                DataType = dataTypeIn;
            }
        }

        public string cdataWrap(string stringIn)
        {
            return "<![CDATA['" + stringIn + "']]>";
        }

        public string alphaNumSpace(string stringIn)
        {
            return Regex.Replace(stringIn, "(?<=[0-9])(?=[A-Za-z])|(?<=[A-Za-z])(?=[0-9])", " ");
        }


        private void button3_Click(object sender, EventArgs e)
        {


            // get a list of nodes
            //XmlNodeList aNodes = doc.SelectNodes("Tags/Tag");

            // loop through all AID nodes
            foreach (XmlNode aNode in doc.GetElementsByTagName("Tag"))
            {
                // grab "x" attribute
                XmlAttribute currentDataType = aNode.Attributes["DataType"];

                // check if that attribute even exists...
                if (currentDataType != null)
                {
                    // if yes - read its current value
                    string currentDataTypeValue = currentDataType.Value;

                    // check if IQ_DO_CONTROL datatype and grab info
                    if (currentDataTypeValue.Equals("IQ_DO_CONTROL") | currentDataTypeValue.Equals("IQC_DO_CONTROL") | currentDataTypeValue.Equals("IQC_MOTOR_CONTROL") | currentDataTypeValue.Equals("IQC_VALVE_DISCRETE") | currentDataTypeValue.Equals("IQC_VALVE_MODULATION_CONTROL") | currentDataTypeValue.Equals("IQ_VFD_CONTROL") | currentDataTypeValue.Equals("IQ_AI_CONTROL") | currentDataTypeValue.Equals("IQ_FLOW_METER"))
                    {

                        string tag = aNode.Attributes["Name"].Value;
                        if (chkSpaceTags.Checked) { tag = alphaNumSpace(tag); }

                        string description = "";
                        XmlNode descriptionNode = aNode.SelectSingleNode("Description");
                        if (descriptionNode != null)
                        {
                            description = descriptionNode.InnerText;//.Substring(9);
                        }
                        string dataType = currentDataTypeValue;


                        if (currentDataTypeValue.Equals("IQ_DO_CONTROL") | currentDataTypeValue.Equals("IQC_DO_CONTROL") | currentDataTypeValue.Equals("IQC_VALVE_DISCRETE") | currentDataTypeValue.Equals("IQC_MOTOR_CONTROL") | currentDataTypeValue.Equals("IQC_VALVE_MODULATION_CONTROL"))
                        {
                            // check for comments for HOA DINT
                            XmlNode comments = aNode.SelectSingleNode("Comments");
                            if(comments != null)
                            {
                            XmlNode commentsNew = doc.CreateElement("Comments");
                            commentsNew.InnerXml = Properties.Resources.TextFile2.ToString().Replace("<![CDATA[", "<![CDATA[" + tag);
                            aNode.PrependChild(commentsNew);
                            }  

                            // Move Description Up
                            aNode.RemoveChild(aNode.SelectSingleNode("Description"));
                            aNode.PrependChild(descriptionNode);
                        }


                        // write to tag len
                        try
                        {
                            string stringOff = "Off";
                            string stringOn = "On";
                            foreach(string csvTag in listBox2.Items)
                            {
                                string[] splitted = csvTag.Split('|');
                                if (splitted[0].Equals(tag)){
                                    stringOn = splitted[1];
                                    stringOff = splitted[2];
                                };
                            }

                            //aNode.SelectSingleNode("Data/Structure/StructureMember[@Name='Tag']/DataValueMember[@Name='LEN']").Attributes["Value"].Value = tag.Length.ToString();
                            //aNode.SelectSingleNode("Data/Structure/StructureMember[@Name='Tag']/DataValueMember[@Name='DATA']").InnerXml = cdataWrap(tag);
                            //aNode.SelectSingleNode("Data/Structure/StructureMember[@Name='Description']/DataValueMember[@Name='LEN']").Attributes["Value"].Value = description.Length.ToString();
                            //aNode.SelectSingleNode("Data/Structure/StructureMember[@Name='Description']/DataValueMember[@Name='DATA']").InnerXml = cdataWrap(description);

                            aNode.SelectSingleNode("Data/Structure/StructureMember[@Name='WWOnString']/DataValueMember[@Name='LEN']").Attributes["Value"].Value = stringOn.Length.ToString();
                            aNode.SelectSingleNode("Data/Structure/StructureMember[@Name='WWOnString']/DataValueMember[@Name='DATA']").InnerXml = cdataWrap(stringOn);

                            aNode.SelectSingleNode("Data/Structure/StructureMember[@Name='WWOffString']/DataValueMember[@Name='LEN']").Attributes["Value"].Value = stringOff.Length.ToString();
                            aNode.SelectSingleNode("Data/Structure/StructureMember[@Name='WWOffString']/DataValueMember[@Name='DATA']").InnerXml = cdataWrap(stringOff);
                        }
                        catch (NullReferenceException p)
                        {
                            Console.WriteLine(p.Message);
                        }
                    }
                }
            }
            foreach (IQ_X_CONTROL x in Array_IQ_X_CONTROL)
            {
                Console.WriteLine(x.Name + " | " + x.DescPLC);
                // adds spaces between alpha and num
                //listBox1.Items.Add(Regex.Replace(x.Name, "(?<=[0-9])(?=[A-Za-z])|(?<=[A-Za-z])(?=[0-9])"," ") + " | " + x.DataType + " | " + x.DescPLC);
                listBox1.Items.Add(x.Name + " | " + x.DataType + " | " + x.DescPLC);
            }
            // save the XmlDocument back to disk
            doc.Save(Outfolder + "\\" + "XMLFileOut.xml");
        }

        private void btnReadFile_Click(object sender, EventArgs e)
        {
            doc.Load(pathFile);

            // loop through all AID nodes
            foreach (XmlNode aNode in doc.GetElementsByTagName("Tag"))
            {
                // grab "x" attribute
                XmlAttribute currentDataType = aNode.Attributes["DataType"];

                // check if that attribute even exists...
                if (currentDataType != null)
                {
                    // if yes - read its current value
                    string currentValue = currentDataType.Value;
                    if (!checkedListBox1.Items.Contains(currentValue))
                    {
                        checkedListBox1.Items.Add(currentValue);
                    }

                    // check if IQ_DO_CONTROL datatype and grab info
                    if (currentValue.Equals("IQ_DO_CONTROL") | currentValue.Equals("IQC_DO_CONTROL") | currentValue.Equals("IQC_VALVE_DISCRETE") | currentValue.Equals("IQC_MOTOR_CONTROL") | currentValue.Equals("IQC_AO_CONTROL") | currentValue.Equals("IQ_DI_CONTROL") | currentValue.Equals("IQ_AO_CONTROL") | currentValue.Equals("IQ_VFD_CONTROL") | currentValue.Equals("IQ_AI_CONTROL") | currentValue.Equals("IQ_FLOW_METER"))
                    {
                        //Console.WriteLine(aNode.Attributes["Name"].Value);
                        //string tag = Regex.Replace(aNode.Attributes["Name"].Value, "(?<=[0-9])(?=[A-Za-z])|(?<=[A-Za-z])(?=[0-9])", " "); // adds spaces between alpha and num
                        string tag = aNode.Attributes["Name"].Value;
                        string description = "";
                        XmlNode descriptionNode = aNode.SelectSingleNode("Description");
                        if (descriptionNode != null)
                        {
                            description = descriptionNode.InnerText;//.Substring(9);
                        }
                        string dataType = currentValue;
                        //description = description.Substring(0, description.Length - 3);
                        Array_IQ_X_CONTROL.Add(new IQ_X_CONTROL(tag, description, dataType));
                    }
                }
            }
            foreach (IQ_X_CONTROL x in Array_IQ_X_CONTROL)
            {
                Console.WriteLine(x.Name + " | " + x.DescPLC);
                // adds spaces between alpha and num
                //listBox1.Items.Add(Regex.Replace(x.Name, "(?<=[0-9])(?=[A-Za-z])|(?<=[A-Za-z])(?=[0-9])"," ") + " | " + x.DataType + " | " + x.DescPLC);
                listBox1.Items.Add(x.Name + " | " + x.DataType + " | " + x.DescPLC);
            }

            // read csv to array
            StreamReader sr = new StreamReader(pathCSV);
            string line;
            string[] row = new string[2];
            while ((line = sr.ReadLine()) != null)
            {
                row = line.Split(',');

                // for every csv line, make file
                string text = "";
                text = text.Replace("TAG_HOLDER", row[0].Replace("XX", "  "));
                text = text.Replace("DESC_HOLDER", row[1].Replace(" ", " "));
                //File.WriteAllText(folder + "\\" + row[0] + ".par", text, Encoding.Unicode);
                listBox2.Items.Add(row[2].Replace('-', '_') + "|" + row[16] + "|" + row[17]);
            }

        }

        private void btnHOA_Click(object sender, EventArgs e)
        {
            string hoaOut = "";
            int count = 10;
            Regex theRegex = new Regex("Rung Number=.?."); // not catching last double quote, . can be 0
            foreach (IQ_X_CONTROL x in Array_IQ_X_CONTROL)
            {
                hoaOut += theRegex.Replace(Properties.Resources.HOA.ToString().Replace("VFD800", x.Name), delegate (Match thisMatch) { return "Rung Number=\"" + count++.ToString(); });
            }
            File.WriteAllText(Outfolder + "\\" + "HOAout.txt", hoaOut, Encoding.Unicode);
        }

        private void btnAlarms_Click(object sender, EventArgs e)
        {
            string alarmOut = "";
            int count = 0;
            Regex theRegex = new Regex("Rung Number=.?."); // not catching last double quote, . can be 0
            foreach (IQ_X_CONTROL x in Array_IQ_X_CONTROL)
            {
                alarmOut += theRegex.Replace(Properties.Resources.AlarmRungsExport.ToString().Replace("TAG_1", x.Name), delegate (Match thisMatch) { return "Rung Number=\"" + count++.ToString(); });
            }
            File.WriteAllText(Outfolder + "\\" + "ALARMSout.txt", alarmOut, Encoding.Unicode);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                pathCSV = file.FileName;
                textBox2.Text = pathCSV;
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog file = new FolderBrowserDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                Outfolder = file.SelectedPath;
                label5.Text = Outfolder;
            }
        }
    }
}
