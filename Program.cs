using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;

namespace PProxy {
    class Program {
        static ConnectionFactory fc = new ConnectionFactory ();
        static IConnection ic; //=fc.CreateConnection();
        static void Main (string[] args) {
            Console.WriteLine ("Hello World!");
            // var cmd=cmd_Excution( buildcmd());

            // foreach (var item in cmd.Res_RAW_List)
            // {
            //     Console.WriteLine(item  );
            // }

            // Console.WriteLine(p.cmd_List.ToJson());
            //     foreach (var i in p.cmd_List)
            //     {
            //         i.Res_RAW_List=cmd_Excution(i).Res_RAW_List;
            //     }

            //     // foreach (var m in p.cmd_List)
            //     // {
            //     //     foreach (var it in m.Res_RAW_List)
            //     //     {
            //     //         Console.WriteLine(it);
            //     //     }
            //     // }
            //    string str= p.ToJson();
            //Console.WriteLine(p.ToJson());
            //Build_Conn();
            //upload_Baseinfo ();
            //Console.ReadLine();

            //   foreach (var item in res)
            //   {
            //     foreach (var it in item.Res_RAW_List)
            //     {
            //           Console.WriteLine(it);
            //     }

            //   }
            // Console.WriteLine(dlist.ToJson());
            //  disklist();
            // mkdir ();
            //   FFmart();


               mkdir_format_mount();

        }
        static void FFmart (string diskname) {
            var res = Command_P ("mkfs.xfs", " -f /dev/" + diskname);

            foreach (var ss in res.Res_RAW_List)
                Console.WriteLine (ss);

        }
        static void IOZ (string P) {
            var res = Command_P ("iozone", P);
            foreach (var ss in res.Res_RAW_List)
                Console.WriteLine (ss);
        }
        static void M_disk (string P) {
            var res = Command_P ("mount", P);
            foreach (var ss in res.Res_RAW_List)
                Console.WriteLine (ss);
        }
        static void mkdir_format_mount () {
            //Command_P("mkdir"," -p /"+"abccc"+" ");
            var res = disklist ();
            if (res.Blockdevices.Count () > 1) {
                string tm="";
                var rr=res.Blockdevices.Where (z => z.Type == TypeEnum.Disk && z.Name != "sdc" && z.Name != "nvme0n1");
                foreach (var act in rr) {
                    Directory.CreateDirectory ("test/_" + act.Name);
                   FFmart (act.Name);
                    M_disk ("/dev/" + act.Name + " " + "test/_" + act.Name);
                   tm=tm+"test/_" + act.Name+"/_tt"+act.Size+" ";

                }
                Console.WriteLine(tm);
                IOZ(" -i 0 -i 1 -i "+rr.Count().ToString()+" -r 1024k -s 1G -t 2 -F "+tm);
            } else {
                var bct=res.Blockdevices[0];
                 Directory.CreateDirectory ("test/_" + bct.Name);
                 //   FFmart (act.Name);
               //     M_disk ("/dev/" + act.Name + " " + "test/_" + act.Name);
IOZ(" -i 0 -i 1 -i 2 -r 1024k -s 1G -t 1 -F "+"test/_" + bct.Name+"/_tt"+bct.Size);
            }

        }
        static void Build_Conn () {
            fc.HostName = "sqlsrv01.ezdc.gwsc";
            fc.UserName = "jingxi";
            fc.Password = "Developer200";

            ic = fc.CreateConnection ();
        }
        static void upload_Baseinfo () {
            var p = buildPackage ();
            Console.WriteLine (p.Product_Name);
            Console.WriteLine (p.Produc_SN);
            Console.WriteLine (p.Exec_Datetime);
            Console.WriteLine (p.cmd_List.Count);
            //string res = Newtonsoft.Json.JsonConvert.SerializeObject (Srv_Factory ());
            var client = new RestClient ("http://app.chinasupercloud.com:8088/support/api/ipmi");
            client.Timeout = -1;
            var request = new RestRequest (Method.POST);
            request.AddHeader ("Content-Type", "application/json");
            request.AddParameter ("application/json", p.ToJson (), ParameterType.RequestBody);
            IRestResponse response = client.Execute (request);
            Console.WriteLine (response.Content);

        }
        static void EX () {
            using (var channel = ic.CreateModel ()) {
                channel.QueueDeclare ("hello", false, false, false, null);
                string message = "here is message..."; //GetMessage(args);
                var properties = channel.CreateBasicProperties ();
                properties.DeliveryMode = 2;

                var body = Encoding.UTF8.GetBytes (message);
                channel.BasicPublish ("", "hello", properties, body);
                Console.WriteLine (" set {0}", message);
            }
        }
        static List<Command_obj> ReadFromJson () {

            List<Command_obj> cmd_List = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Command_obj>> (File.ReadAllText (@"./cmd.json"));

            foreach (var item in cmd_List.Where (z => z.Cmd_type != "STRESS")) {
                //    Console.WriteLine (item.Cmd_EXEFile + " " + item.Cmd_Args);

                item.Res_RAW_List = cmd_Excution (item).Res_RAW_List;

                //   Console.WriteLine(str);

            }
            //   Console.WriteLine (cmd_List.ToJson ());
            return cmd_List;
        }
        static ServerDisk disklist () {

            var res = List_Command ("lsblk");
            string s = "";
            foreach (var item in res.Res_RAW_List) {
                s = s + item;

            }
            ServerDisk serverDisk = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerDisk> (s);
            //  Console.WriteLine (serverDisk.Blockdevices.Where (z => z.Type == TypeEnum.Disk).Count ());

            // var dlist = new List<Disk_lsblk> ();
            // res.Res_RAW_List.RemoveAt(0);
            // foreach (var item in res.Res_RAW_List.Where(z=>z.Contains("disk"))) {
            //     var str = item.Split (' ', StringSplitOptions.RemoveEmptyEntries);
            //     // foreach (var st in str)
            //     // {
            //     // Console.WriteLine(st);
            //     dlist.Add (new Disk_lsblk () {
            //         Name = str[0],
            //             MAJ_MIN = str[1],
            //             RM = int.Parse (str[2]),
            //             Size = str[3],//?null:"",
            //             RO = int.Parse (str[4]),
            //             Type = str[5],
            //             Mount_Point = ""
            //     }

            // );

            //     //  }
            // }
            return serverDisk;

        }
        static Command_obj List_Command (string Cmd_name) {

            List<Command_obj> cmd_List = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Command_obj>> (File.ReadAllText (@"./cmd.json"));

            var item = cmd_List.Where (z => z.Cmd_name == "lsblk").SingleOrDefault ();
            //    Console.WriteLine (item.Cmd_EXEFile + " " + item.Cmd_Args);

            item.Res_RAW_List = cmd_Excution (item).Res_RAW_List;

            //   Console.WriteLine(str);

            //   Console.WriteLine (cmd_List.ToJson ());
            return item;
        }
        static Command_obj Command_P (string Cmd_name, string P) {

            List<Command_obj> cmd_List = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Command_obj>> (File.ReadAllText (@"./cmd.json"));

            var item = cmd_List.Where (z => z.Cmd_name == Cmd_name).SingleOrDefault ();
            item.Cmd_Args = P;
            //    Console.WriteLine (item.Cmd_EXEFile + " " + item.Cmd_Args);

            item.Res_RAW_List = cmd_Excution (item).Res_RAW_List;

            //   Console.WriteLine(str);

            //   Console.WriteLine (cmd_List.ToJson ());
            return item;
        }

        static Command_obj cmd_Excution (Command_obj command_) {
            command_.Res_RAW_List = Cmd_Excutor.common_cmd (command_.Cmd_Args, command_.Cmd_EXEFile);

            return command_;

        }

        static Command_obj buildcmd () {
            var cmd1 = new Command_obj () {
                Cmd_name = "dmidecoe",
                Cmd_EXEFile = "/sbin/dmidecode",
                Cmd_Args = " -t BIOS ",
                Cmd_Description = "Show Bios info",
                Cmd_type = "show info"
                //, Res_RAW_List=new System.Collections.Generic.List<string>()

            };
            return cmd1;
        }
        static Commnand_Pack buildPackage () {
            var clist = ReadFromJson ();
            string SN = "";
            string PN = "";
            var res = clist.Where (z => z.Cmd_name == "dmidecode" && z.Cmd_Args == " -t system ").SingleOrDefault ();
            SN = res.Res_RAW_List.Where (z => z.Contains ("Serial Number:")).SingleOrDefault ().Split (':', StringSplitOptions.RemoveEmptyEntries).Last ();
            //Console.WriteLine(str);
            PN = res.Res_RAW_List.Where (z => z.Contains ("Product Name:")).SingleOrDefault ().Split (':', StringSplitOptions.RemoveEmptyEntries).Last ();

            var pack = new Commnand_Pack () {
                Exec_Datetime = DateTime.Today.ToShortDateString (),
                Produc_SN = SN,
                Product_Name = PN,
                cmd_List = clist

            };

            return pack;
        }
    }
}