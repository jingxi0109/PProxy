using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
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
            Build_Conn ();
            upload_Baseinfo ();

          //  CPUStrees ();


           // mkdir_format_mount ();
            // Console.ReadLine ();
            //////......................

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

        }
        static void CPUStrees () {
            var res = Command_P ("stressapptest");
            foreach (var ss in res.Res_RAW_List)
                Console.WriteLine (ss);
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
            var res = disklist_old ();
            Console.WriteLine (res.Blockdevices.Count ());

            if (res.Blockdevices.Count () > 1) {
                string tm = "";
                var rr = res.Blockdevices.Where (z => z.Type == "disk" && z.Name != "sda");
                Console.WriteLine (rr.Count ());
                foreach (var act in rr) {
                    Console.WriteLine (act.Name);
                    Directory.CreateDirectory ("test/_" + act.Name);
                    FFmart (act.Name);
                    M_disk ("/dev/" + act.Name + " " + "test/_" + act.Name);
                    tm = tm + "test/_" + act.Name + "/_tt" + act.Size + " ";

                }
                Console.WriteLine (tm);
                IOZ (" -i 0 -i 1 -i " + rr.Count ().ToString () + " -r 1024k -s 1G -t 2 -F " + tm);
            } else {
                var bct = res.Blockdevices.Where (z => z.Type == "disk" && z.Name == "sda").SingleOrDefault ();
                Directory.CreateDirectory ("test/_" + bct.Name);
                //   FFmart (act.Name);
                //     M_disk ("/dev/" + act.Name + " " + "test/_" + act.Name);
                IOZ (" -i 0 -i 1 -i 2 -r 1024k -s 1G -t 1 -F " + "test/_" + bct.Name + "/_tt" + bct.Size);
            }

        }
        static void Build_Conn () {
            fc.HostName = "192.168.7.12";
            fc.UserName = "stressinfo";
            fc.Password = "Developer200";
            fc.Port = Protocols.DefaultProtocol.DefaultPort;
            fc.ClientProvidedName = "exec_server" + GetLocalIPAddress ();
            // ClientProvidedName = "cmd_Server"

            ic = fc.CreateConnection ();
        }
        static string GetLocalIPAddress () {
            //string ipaddr = "";
            var firstUpInterface = NetworkInterface.GetAllNetworkInterfaces ()
                .OrderBy (c => c.Speed)
                .FirstOrDefault (c => c.NetworkInterfaceType != NetworkInterfaceType.Loopback && c.OperationalStatus == OperationalStatus.Up);
            // Console.WriteLine ();

            var slist = new List<string> ();

            foreach (var res in firstUpInterface.GetIPProperties ().UnicastAddresses) {

                slist.Add (res.Address.ToString ());
            }
            slist.Add (firstUpInterface.GetPhysicalAddress ().ToString ());
            //    ipaddr= firstUpInterface.GetIPProperties().UnicastAddresses.ToJson();

            //     if (firstUpInterface != null) {
            //         var props = firstUpInterface.GetIPProperties ();
            //         // get first IPV4 address assigned to this interface
            //         var firstIpV4Address = props.UnicastAddresses
            //             .Where (c => c.Address.AddressFamily == AddressFamily.InterNetwork)
            //             .Select (c => c.Address)
            //             .FirstOrDefault ();
            //     //    ipaddr = firstIpV4Address.ToString ();
            //     }
            // Console.WriteLine(host.HostName);
            //    Console.WriteLine(host.AddressList.Count());

            return slist.ToJson (); //firstIpV4Address.ToString();
            //throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        static void upload_Baseinfo () {
            var p = buildPackage ();
            Console.WriteLine (p.Product_Name);
            Console.WriteLine (p.Produc_SN);
             Console.WriteLine (p.ipmi_IP);
            Console.WriteLine (p.Exec_Datetime);
            Console.WriteLine (p.cmd_List.Count);
            //string res = Newtonsoft.Json.JsonConvert.SerializeObject (Srv_Factory ());
            var client = new RestClient ("http://192.168.7.10:8088/support/api/ipmi");
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

            foreach (var item in cmd_List.Where (z => z.Cmd_type == "info")) {
                //    Console.WriteLine (item.Cmd_EXEFile + " " + item.Cmd_Args);

                item.Res_RAW_List = cmd_Excution (item).Res_RAW_List;

                // foreach (var str in item.Res_RAW_List) {
                //     Console.WriteLine (str);
                // }
                //   

            }

            
            var disk=disklist_old();
            foreach (var item in disk.Blockdevices.Where(z=>z.Type=="disk"))
            {
                var cmd=cmd_List.Where(z=>z.Cmd_type=="sub").FirstOrDefault();

              //  cmd.Cmd_name="";
                cmd.Cmd_Args=cmd.Cmd_Args.Trim()+item.Name;
                cmd.Cmd_type="hdd";
                cmd.Res_RAW_List=cmd_Excution(cmd).Res_RAW_List;
                System.Console.WriteLine( cmd.Cmd_Args);
                cmd_List.Add(cmd);
            }

            //   Console.WriteLine (cmd_List.ToJson ());
            return cmd_List;
        }
        static ServerDisk disklist () {

            var res = Command_P ("lsblk");
            string s = "";
            foreach (var item in res.Res_RAW_List) {
                s = s + item;

            }
            ServerDisk serverDisk = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerDisk> (s);
 
            return serverDisk;

        }
        static ServerDisk disklist_old () {

            var res = Command_P ("lsblk","  -n " ,"old");
            // string s = "";
            ServerDisk disk = new ServerDisk ();
            disk.Blockdevices = new List<Blockdevice> ();
            foreach (var item in res.Res_RAW_List) {
                var s = item.Split (" ", StringSplitOptions.RemoveEmptyEntries);
                disk.Blockdevices.Add (new Blockdevice () {
                    Name = s[0], Size = s[3], Type = s[5]

                });

            //    Console.WriteLine (s[0] + "  " + s[3] + " " + s[5]);

            }
            // foreach (var re in disk.Blockdevices) {
            //     Console.WriteLine (re.Type);
            // }
            //res.Res_RAW_List.Add()

            return disk;

        }
        static Command_obj Command_P (string Cmd_name) {

            List<Command_obj> cmd_List = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Command_obj>> (File.ReadAllText (@"./cmd.json"));

            var item = cmd_List.Where (z => z.Cmd_name == Cmd_name).SingleOrDefault ();
            //    Console.WriteLine (item.Cmd_EXEFile + " " + item.Cmd_Args);

            item.Res_RAW_List = cmd_Excution (item).Res_RAW_List;

            //   Console.WriteLine(str);

            //   Console.WriteLine (cmd_List.ToJson ());
            return item;
        }
        static Command_obj Command_P (string Cmd_name,string p, string des) {

            List<Command_obj> cmd_List = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Command_obj>> (File.ReadAllText (@"./cmd.json"));

            var item = cmd_List.Where (z => z.Cmd_name == Cmd_name&&z.Cmd_Args==p && z.Cmd_Description == des).FirstOrDefault ();
            //    Console.WriteLine (item.Cmd_EXEFile + " " + item.Cmd_Args);

            item.Res_RAW_List = cmd_Excution (item).Res_RAW_List;

            //   Console.WriteLine(str);

            //   Console.WriteLine (cmd_List.ToJson ());
            return item;
        }
        static  Command_obj Command_P (string Cmd_name, string P) {

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




        static Commnand_Pack buildPackage () {
            var clist = ReadFromJson ();
            string SN = "";
            string PN = "";
            List<string> ipl = new List<string> ();

            var res = clist.Where (z => z.Cmd_name == "dmidecode" && z.Cmd_Args == " -t system ").SingleOrDefault ();
            SN = res.Res_RAW_List.Where (z => z.Contains ("Serial Number:")).SingleOrDefault ().Split (':', StringSplitOptions.RemoveEmptyEntries).Last ();
            //Console.WriteLine(str);
            PN = res.Res_RAW_List.Where (z => z.Contains ("Product Name:")).SingleOrDefault ().Split (':', StringSplitOptions.RemoveEmptyEntries).Last ();
            var res_ipmi = clist.Where (z => z.Cmd_name == "ipmitool" && z.Cmd_Args == " -c  lan print 1 ").SingleOrDefault ();
            var sip = res_ipmi.Res_RAW_List.Where (z => z.Contains ("IP Address") && !z.Contains ("IP Address Source")).FirstOrDefault ().Split (':', StringSplitOptions.RemoveEmptyEntries).Last ();
            var res_ip = clist.Where (z => z.Cmd_name == "bash" && z.Cmd_Args == " -c ifconfig ").SingleOrDefault ().Res_RAW_List;
            var list = res_ip.Where (z => !z.StartsWith (" ") && !z.Contains ("lo"));
            foreach (var re in list) {
                //    Console.WriteLine (re);
                int index = res_ip.IndexOf (re);
                var str = res_ip[index + 1];
                var resz = str.Split (' ', StringSplitOptions.RemoveEmptyEntries);
                if (resz[0] == "inet") {
                    ipl.Add (resz[1]);
                }

                //       GGG.IP.Add(res[1]);

            }

            // foreach (var p in sip)
            // {
            //     Console.Write(p+":");
            // }
            var pack = new Commnand_Pack () {
                Exec_Datetime = DateTime.Today.ToShortDateString (),
                Produc_SN = SN,
                Product_Name = PN,
                ipmi_IP = sip,
                ip = ipl,
                cmd_List = clist

            };

            return pack;
        }
    }
}
