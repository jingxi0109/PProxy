using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PProxy {
    class Cmd_Excutor {

        public static List<string> common_cmd (string cmd, string filename) {

            string command = cmd; //"write your command here";
            string filenamel = filename;
            string result = "";
            if (File.Exists (filenamel)) {

                // using (System.Diagnostics.Process proc = new System.Diagnostics.Process ()) {
                //     proc.StartInfo.FileName = filenamel; //"/bin/bash";
                //     proc.StartInfo.Arguments = "  " + command; //"-c  " + command;// + @" -a | grep -i  'inet ' ";
                //     proc.StartInfo.UseShellExecute = false;
                //     proc.StartInfo.RedirectStandardOutput = true;
                //     proc.StartInfo.RedirectStandardError = true;
                //     proc.Start ();

                //     result += proc.StandardOutput.ReadToEnd ();
                //     result += proc.StandardError.ReadToEnd ();

                //     proc.WaitForExit ();
                // }
                result = execcmd (filenamel, command);
            } else {
                filenamel = "/usr" + filename;
                if (File.Exists (filenamel)) {

                    // using (System.Diagnostics.Process proc = new System.Diagnostics.Process ()) {
                    //     proc.StartInfo.FileName = filenamel; //"/bin/bash";
                    //     proc.StartInfo.Arguments = "  " + command; //"-c  " + command;// + @" -a | grep -i  'inet ' ";
                    //     proc.StartInfo.UseShellExecute = false;
                    //     proc.StartInfo.RedirectStandardOutput = true;
                    //     proc.StartInfo.RedirectStandardError = true;
                    //     proc.Start ();

                    //     result += proc.StandardOutput.ReadToEnd ().Trim ();
                    //     result += proc.StandardError.ReadToEnd ().Trim ();

                    //     proc.WaitForExit ();
                    // }
                    result = execcmd (filenamel, command);

                } else {
                    throw new Exception (filenamel);
                }

            }
            var sslist = new List<string> ();
            var res = result.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList ();

            sslist.AddRange (res);

            return sslist;

        }
        static string execcmd (string fname, string cmd) {
            string res = "";

            using (System.Diagnostics.Process proc = new System.Diagnostics.Process ()) {
                proc.StartInfo.FileName = fname; //"/bin/bash";
                proc.StartInfo.Arguments = "  " + cmd; //"-c  " + command;// + @" -a | grep -i  'inet ' ";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start ();

                res += proc.StandardOutput.ReadToEnd ().Trim ();
                res += proc.StandardError.ReadToEnd ().Trim ();

                proc.WaitForExit ();
            }
            return res;
        }

        public static void UID_ON () {
            common_cmd (" raw 0x30 0x0d", "/bin/ipmitool");
        }
        public static void UID_OFF () {
            common_cmd (" raw 0x30 0x0e", "/bin/ipmitool");
        }
    }
    public class Command_obj {
        public string Cmd_name { get; set; }
        public string Cmd_EXEFile { set; get; }
        public string Cmd_type { get; set; }
        public string Cmd_Args { get; set; }
        public string Cmd_Description { set; get; }

        public List<string> Filed_List { set; get; }
        public List<string> Res_RAW_List { set; get; }
    }
    public class Commnand_Pack {

        public string Exec_Datetime { set; get; }
        public string Produc_SN { set; get; }
        public string Product_Name { set; get; }
        public string ipmi_IP { set; get; }
        public List<string> ip { set; get; }

        public List<Command_obj> cmd_List { set; get; }
    }
    // public class Disk_lsblk{
    //     public string  Name { get; set; }
    //     public string MAJ_MIN { get; set; }
    //     public int  RM     { get; set; }     
    // public string Size { get; set; }
    // public int RO { get; set; }
    // public string Type { get; set; }
    // public  string Mount_Point{set;get;}

    // }
    public partial class ServerDisk {
        public List<Blockdevice> Blockdevices { get; set; }
    }

    public partial class Blockdevice {
        public string Name { get; set; }
        public string MajMin { get; set; }
        public bool Rm { get; set; }
        public string Size { get; set; }
        public bool Ro { get; set; }
        public string Type { get; set; }
        public string Mountpoint { get; set; }
        public List<Blockdevice> Children { get; set; }
    }

    // public enum TypeEnum { Disk, Loop, Part }
}