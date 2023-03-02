using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFileWatch
{
    class Program
    {
        public static bool move = true;
        public static string target = string.Empty;

        public static Dictionary<string, string> argDic = new Dictionary<string, string>();
        static void ParseArgs(string[] args) {
            if (args.Length % 2 == 0 && args.Length > 0)
            {

                for (var i = 0; i < args.Length; i = i + 2)
                {
                    argDic[args[i]] = args[i + 1];
                }
            }
        }

        static void Help() {
            Console.WriteLine("Usage: SimpleFileWatch -path <directory1,directory2,...> -filter <filter> -target <target> -move <true|false>");
        }

        static async Task StartWatch(string path , string filter) {
            await Console.Out.WriteLineAsync("[*] INFO : Start Watcher : " + path);
            var watcher = new FileSystemWatcher(path);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            watcher.Filter = filter;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            watcher.Created += async (sender, eventArgs) =>
            {
                await Console.Out.WriteLineAsync($"[*] INFO :{eventArgs.FullPath} was created.");
            };

            watcher.Changed += onChange;

            watcher.Deleted += async (sender, eventArgs) =>
            {
                await Console.Out.WriteLineAsync($"[*] INFO :{eventArgs.FullPath} was deleted.");
            };

            watcher.Renamed += async (sender, eventArgs) =>
            {
                await Console.Out.WriteLineAsync($"[*] INFO :{eventArgs.OldFullPath} was renamed to {eventArgs.FullPath}.");
            };
            watcher.Error += async (sender, eventArgs) =>
            {
                await Console.Out.WriteLineAsync("ERROR");
            };
            await Task.Delay(-1);
        }

        static async void onChange(object sender, FileSystemEventArgs eventArgs) {
            await Console.Out.WriteLineAsync($"[*] INFO :{eventArgs.FullPath} was changed.");
            if (move) {
                if (File.Exists(eventArgs.FullPath)){

                    var targetDir = Directory.CreateDirectory(target);

                    var destFileName = Path.Combine(targetDir.FullName,eventArgs.Name);
                    if (File.Exists(destFileName)) {
                        File.Delete(destFileName);
                    }

                    await Console.Out.WriteLineAsync($"[*] INFO :{eventArgs.FullPath} was move to {destFileName}.");

                    File.Copy(eventArgs.FullPath, destFileName);
                }
                
            }
        }
        static async Task RunAsync() {

            if (!argDic.ContainsKey("-path")) {
                throw new Exception("[-] ERROR : Must set path . ");
            }

            var path = argDic["-path"];
            var pathList = path.Split(',');
            var filter = "*.*";
            if (argDic.ContainsKey("-filter")) {
                filter = argDic["-filter"];
                await Console.Out.WriteLineAsync("[*] INFO : Filer : " + filter);
                
            }

            if (argDic.ContainsKey("-move")) {
                if (argDic["-move"] == "true")
                {
                    move = true;
                    await Console.Out.WriteLineAsync("[*] INFO : Move : " + move);
                    if (!argDic.ContainsKey("-target"))
                    {
                        target = Environment.ExpandEnvironmentVariables(@"%TEMP%\target");
                    }
                    else
                    {
                        target = argDic["-target"];
                    }
                    await Console.Out.WriteLineAsync("[*] INFO : Target : " + target);
                }
                else {
                    move = false;
                }
            }

            var taskList = new List<Task>();

            for (int i = 0; i < pathList.Length; i++) {


                taskList.Add(StartWatch(pathList[i], filter));
            }

            await Task.WhenAll();
        }


        static void Main(string[] args)
        {
            if (args.Length <= 0) {
                Help();
                return;
            }
            ParseArgs(args);
            try {
                RunAsync().GetAwaiter().GetResult();
                Console.ReadLine();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Help();
            }


        }
    }
}
