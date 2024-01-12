using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticSharingForGarticPhone
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = @"C:\XXXX\";
            watcher.Filter = "*.gif";
            watcher.Changed += new FileSystemEventHandler(CopyToDropBox);
            watcher.EnableRaisingEvents = true;
        }

        private void CopyToDropBox(object source, FileSystemEventArgs e)
        {
            var CopyToDirectory = $@"C:\XXXXX\{e.Name}";
            // 重複実行されるためコピー先の存在確認
            if (!File.Exists(CopyToDirectory))
            {
                // DropBox管理下にコピーする
                File.Move(e.FullPath, CopyToDirectory);
            }
        }
    }
}
