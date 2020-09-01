using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using RestSharp;

namespace PProxy {
    class Program {
        static void Main (string[] args) {
            Console.WriteLine ("Hello World!");
            // var cmd=cmd_Excution( buildcmd());

            // foreach (var item in cmd.Res_RAW_List)
            // {
            //     Console.WriteLine(item  );
            // }

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
         request.AddParameter ("application/json", p.ToJson(), ParameterType.RequestBody);
         IRestResponse response = client.Execute (request);
         Console.WriteLine (response.Content);
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

        }
        static List<Command_obj> ReadFromJson () {

            List<Command_obj> cmd_List = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Command_obj>> (File.ReadAllText (@"./cmd.json"));

            foreach (var item in cmd_List) {
                //    Console.WriteLine (item.Cmd_EXEFile + " " + item.Cmd_Args);

                item.Res_RAW_List = cmd_Excution (item).Res_RAW_List;

                //   Console.WriteLine(str);

            }
            //   Console.WriteLine (cmd_List.ToJson ());
            return cmd_List;
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