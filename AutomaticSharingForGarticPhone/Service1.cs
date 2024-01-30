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
using System.Windows.Media.Imaging;
using System.Drawing;
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
            Debugger.Launch();
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = @"C:XXXX\";
            watcher.Filter = "*.gif";
            watcher.Changed += new FileSystemEventHandler(CopyToDropBox);
            watcher.EnableRaisingEvents = true;
        }

        private void CopyToDropBox(object source, FileSystemEventArgs e)
        {           
            var dropBox = $@"C:\XXXX\";
            string dir;
            string partitionDir;

            // フォルダ作成
            dir = CreateDirectory(dropBox);
            partitionDir = CreateDirectory(dropBox, 2);

            // 重複実行されるためコピー先の存在確認
            if (File.Exists(dir + e.Name)) return;

            FileStream img = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapDecoder decoder = BitmapDecoder.Create(img, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            // 偶数フレームの数を取得する(端数が出る想定はしない)
            int frameCount = decoder.Frames.Count;
            for (int i = 0; i < frameCount; i++)
            {
                BitmapFrame bmpFrame = decoder.Frames[i];
                //BmpBitmapEncoderを作成する
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                //フレームに追加する
                encoder.Frames.Add(bmpFrame);

                //画像をMemoryStreamに保存する
                MemoryStream ms = new MemoryStream();
                encoder.Save(ms);

                //Bitmapを作成する
                Image img2 = Image.FromStream(ms);
                var file = e.Name.Replace(@".gif", string.Empty) + $@"_{i}.jpg";
                img2.Save(partitionDir + file);
                ms.Close();
            }

            // リソース解放
            img.Dispose();

            // DropBox管理下にコピーする
            File.Move(e.FullPath, dir + e.Name);
        }

        private string CreateDirectory(string path, int target = 1)
        {
            string dt = System.DateTime.Today.ToString("yyyy-MM-dd");
            path = path + dt;
            if (target == 2) path = path + "_partition";

            if (!System.IO.Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path + @"\";
        }
    }
}
