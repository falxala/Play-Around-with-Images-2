using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Text.RegularExpressions;
using Color = System.Drawing.Color;
using System.Drawing.Imaging;
using ImageMagick;

namespace PlayAroundwithImages2
{
    public class Process_Image
    {
        static ImageConverter imgconv = new ImageConverter();
        public static BitmapSource Create_thumbnail(string imagePath)
        {
            var myMagickSettings = new ImageMagick.MagickReadSettings();

            //PDF、EPS、AIのラスタライズ時の設定
            //ラスタライズ時ICCプロファイル
            myMagickSettings.ColorSpace = ImageMagick.ColorSpace.sRGB;
            //ラスタライズ時解像度
            myMagickSettings.Density = new ImageMagick.Density(72);
            //ラスタライズ時カラータイプ
            myMagickSettings.ColorType = ImageMagick.ColorType.TrueColor;

            //必ずソースとなる「Jpeg画像」より、縮小予定のPixel数が半分以下である事
            myMagickSettings.SetDefine(ImageMagick.MagickFormat.Jpg, "size", "255x255");


            using (var myMagick = new ImageMagick.MagickImage(imagePath, myMagickSettings))
            {
                myMagick.AutoOrient();
                myMagick.Strip();
                myMagick.Thumbnail(255, 255);
                var source = new BitmapImage();
                MemoryStream ms = new MemoryStream(myMagick.ToByteArray(ImageMagick.MagickFormat.Bmp));
                using (Stream stream = ms)
                {
                    source.BeginInit();
                    source.StreamSource = stream;
                    source.CacheOption = BitmapCacheOption.OnLoad;
                    source.EndInit();
                    source.Freeze();
                }
                return source;
            }
        }

        /// <summary>
        /// 変換オプション
        /// </summary>
        public struct ConvertOptions
        {
            /// <summary>
            /// 変換後ファイルサイズ上限(MB)
            /// </summary>
            public double Filesize { get; set; }

            /// <summary>
            /// 指定サイズで長辺リサイズ
            /// </summary>
            public System.Drawing.Size Size { get; set; }

            public System.Drawing.Size longSide { get; set; }

            /// <summary>
            /// 同一ファイル名があったとき上書きするかどうか
            /// </summary>
            public bool Overwrite { get; set; }

            /// <summary>
            /// 変換フォーマット(ImageMagick)
            /// </summary>
            public ImageMagick.MagickFormat Format { get; set; }

            public ImageMagick.Percentage WidthPercentage { get; set; }

            public ImageMagick.Percentage HeightPercentage { get; set; }

            public bool LiquidRescale { get; set; }

            public bool Mirror { get; set; }

            public int Rotate { get; set; }

            /// <summary>
            /// Jpeg Png Option Quality[0-100]
            /// </summary>
            public int Quality { get; set; }

            /// <summary>
            /// 自由変形フラグ
            /// </summary>
            public bool Transform { get; set; }

            /// <summary>
            /// 保存ディレクトリパス
            /// </summary>
            public string SaveDirectory { get; set; }

            /// <summary>
            /// 変換せずにコピー
            /// </summary>
            public bool Passthrough { get; set; }

            /// <summary>
            /// グレースケール
            /// </summary>
            public bool GrayScale { get; set; }

            /// <summary>
            /// ガンマ値
            /// </summary>
            public double Gamma { get; set; }

            public bool NotScaleUp { get; set; }

            public ImageMagick.MagickColor BackgroundColor { get; set; }

            public int DPI { get; set; }

            public int[] Crop { get; set; }

            public string BlenderPath { get; set; }

            public string ColorMode { get; set; }

            public bool ColorSeparation { get; set; }
        }

        /// <summary>
        /// 与えられたファイルを変換します
        /// </summary>
        /// <param name="item">ファイルパス</param>
        /// <param name="option"></param>
        /// <param name="token"></param>
        /// <returns>出力ファイルパス</returns>
        public async Task<String> Convert(Model.drop_Image item, ConvertOptions option, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (option.Passthrough == true)
            {
                FileInfo file = new FileInfo(item.Image_path);
                string destinationPath = option.SaveDirectory + "\\" + System.IO.Path.GetFileName(item.Image_path);
                Console.WriteLine(destinationPath);
                file.CopyTo(destinationPath);

                DateTime LastWriteTime, CreationTime;
                //更新日時の取得
                LastWriteTime = System.IO.File.GetLastWriteTime(item.Image_path);
                //作成日時の取得
                CreationTime = System.IO.File.GetCreationTime(item.Image_path);
                //作成・更新日時の設定
                File.SetCreationTime(destinationPath, CreationTime);
                File.SetLastWriteTime(destinationPath, LastWriteTime);

                return (destinationPath);
            }

            var Result = await Task.Run(() =>
            {
                var myMagickSettings = new ImageMagick.Formats.JpegWriteDefines();
                myMagickSettings.Extent = (int)(option.Filesize * 1024);
                string outputPath = "";
                string outputDir = option.SaveDirectory + "\\";
                Directory.CreateDirectory(outputDir);

                string output_Num = "Cnv";
                string output_name = System.IO.Path.GetFileNameWithoutExtension(item.Image_path);

                //PDF、EPS、AIのラスタライズ時の設定
                var myMagicReadkSettings = new ImageMagick.MagickReadSettings();
                //ラスタライズ時ICCプロファイル
                myMagicReadkSettings.ColorSpace = ImageMagick.ColorSpace.sRGB;
                //ラスタライズ時解像度
                myMagicReadkSettings.Density = new ImageMagick.Density(option.DPI);
                //ラスタライズ時カラータイプ
                myMagicReadkSettings.ColorType = ImageMagick.ColorType.TrueColor;


                //ファイル名
                string f = Regex.Replace(option.Format.ToString().ToLowerInvariant(), "[^a-z]+", string.Empty);

                outputPath = outputDir +  output_name +"_"+ output_Num + "." + f;

                int Count = 2;
                if (!option.Overwrite)
                    while (true)
                    {
                        if (!option.Overwrite && File.Exists(outputPath))
                        {
                            output_Num = "_Cnv" + Count;
                            outputPath = outputDir + output_name + output_Num + "." + f;
                            Count++;
                        }
                        else
                        {
                            break;
                        }
                    }
                if (item.Format.ToLower() == "pdf" || item.Format.ToLower() == "ai")
                    using (var myMagicks = new ImageMagick.MagickImageCollection(item.Image_path, myMagicReadkSettings))
                    {
                        for (int i = 0; i < myMagicks.Count; i++)
                        {
                            _process(option, token, (MagickImage)myMagicks[i]);

                            //「ファイル」へ書き出し  
                            if (i == 0)
                                myMagicks[i].Write(outputPath, myMagickSettings);
                            else
                                myMagicks[i].Write(outputDir + output_name + "_" + output_Num + "_" + i + "." + f, myMagickSettings);

                            myMagicks[i].Dispose();

                            token.ThrowIfCancellationRequested();
                        }

                        return outputPath;
                    }
                else
                {
                    using (var myMagick = new ImageMagick.MagickImage(item.Image_path, myMagicReadkSettings))
                    {
                        _process(option, token, myMagick);

                        if (option.ColorSeparation == true)
                        {
                            var clone = myMagick.Clone();
                            clone.ColorMatrix(new ImageMagick.MagickColorMatrix(6, ColorMatrix.Elemets.RC));
                            clone.Write(outputDir + output_name + output_Num + "_RC"+"." + f, myMagickSettings);

                            clone = myMagick.Clone();
                            clone.ColorMatrix(new ImageMagick.MagickColorMatrix(6, ColorMatrix.Elemets.GM));
                            clone.Write(outputDir + output_name + output_Num + "_GM" + "." + f, myMagickSettings);

                            clone = myMagick.Clone();
                            clone.ColorMatrix(new ImageMagick.MagickColorMatrix(6, ColorMatrix.Elemets.BY));
                            clone.Write(outputDir + output_name + output_Num + "_BY" + "." + f, myMagickSettings);

                            if (myMagick.ColorSpace == ColorSpace.CMYK)
                            {
                                clone = myMagick.Clone();
                                clone.ColorMatrix(new ImageMagick.MagickColorMatrix(6, ColorMatrix.Elemets.Bk));
                                clone.Write(outputDir + output_name + output_Num + "_Bk" + "." + f, myMagickSettings);
                            }

                            token.ThrowIfCancellationRequested();
                            return outputPath;
                        }
                        else
                            myMagick.Write(outputPath, myMagickSettings);

                        DateTime LastWriteTime, CreationTime;
                        //更新日時の取得
                        LastWriteTime = System.IO.File.GetLastWriteTime(item.Image_path);
                        //作成日時の取得
                        CreationTime = System.IO.File.GetCreationTime(item.Image_path);
                        //作成・更新日時の設定
                        File.SetCreationTime(outputPath, CreationTime);
                        File.SetLastWriteTime(outputPath, LastWriteTime);

                        token.ThrowIfCancellationRequested();
                        return outputPath;
                    }
                }

            });
            return Result;
        }

        public void _process(ConvertOptions option, CancellationToken token, MagickImage myMagick)
        {
            //myMagick.AutoOrient();
            if (option.Crop == null)
            {
                option.Crop = new int[] { 0, 0, 0, 0 };
            }
            if ((long)option.Crop[2] * (long)option.Crop[3] > 0)
            {
                MagickGeometry geometry = new MagickGeometry(option.Crop[0], option.Crop[1], option.Crop[2], option.Crop[3]);
                myMagick.Crop(geometry, ImageMagick.Gravity.Center);
            }
            myMagick.Format = option.Format;

            //quality
            myMagick.Quality = ++option.Quality;

            myMagick.BackgroundColor = option.BackgroundColor;

            if (myMagick.Format == ImageMagick.MagickFormat.Jpg)
            {
                if (option.BackgroundColor == new ImageMagick.MagickColor("transparent"))
                    myMagick.BackgroundColor = new ImageMagick.MagickColor("white");
                myMagick.Alpha(ImageMagick.AlphaOption.Remove);
            }

            if (option.BackgroundColor != new ImageMagick.MagickColor("transparent"))
                myMagick.Alpha(ImageMagick.AlphaOption.Remove);

            //リサイズ・変形
            if (option.Transform)
            {
                if (option.LiquidRescale)
                {
                    //自然なリサイズを試行
                    var WidthPer = new ImageMagick.Percentage((double)option.Size.Width / myMagick.Width * 100);
                    var HeightPer = new ImageMagick.Percentage((double)option.Size.Height / myMagick.Height * 100);
                    myMagick.LiquidRescale(WidthPer, HeightPer);
                }
                else
                {
                    var param = new double[]
                    {
                            0,0,                                //入力_左上座標
                            0,0,                                //出力_左上座標
                            0,myMagick.Height,                  //入力_左下座標
                            0,option.Size.Height,               //出力_左下座標
                            myMagick.Width,myMagick.Height,     //入力_右下座標
                            option.Size.Width,option.Size.Height,               //出力_右下座標
                            myMagick.Width,0,                   //入力_右上座標
                            option.Size.Width,0                         //出力_右上座標
                    };
                    //自由変形
                    //指定したサイズにフィットするようにカンバスサイズを変形する設定
                    ImageMagick.DistortSettings distortSettings = new ImageMagick.DistortSettings();
                    distortSettings.Bestfit = true;
                    distortSettings.Viewport = new ImageMagick.MagickGeometry(option.Size.Width, option.Size.Height);
                    myMagick.Distort(ImageMagick.DistortMethod.BilinearForward, distortSettings, param);
                    myMagick.Crop(option.Size.Width, option.Size.Height);
                    myMagick.RePage();
                }
            }
            else if (option.longSide.Width == option.longSide.Height && option.longSide.Width != 0)
            {
                if (!option.NotScaleUp)
                {
                    myMagick.Resize(option.longSide.Width, option.longSide.Height);
                }
                else if (option.Size.Width * option.Size.Height < myMagick.Width * myMagick.Height)
                    myMagick.Resize(option.longSide.Width, option.longSide.Height);
            }

            //回転
            if (option.Rotate != 0)
            {
                myMagick.Rotate(option.Rotate);

            }

            //反転
            if (option.Mirror)
                myMagick.Flop();

            if (option.Gamma != 1)
                myMagick.Level(new ImageMagick.Percentage(0.0), new ImageMagick.Percentage(100.0), (option.Gamma), ImageMagick.Channels.All);

            if (option.ColorMode != "Inherit")
            {
                //「マッチング方法」→「Perceptual」知覚的（Photoshopデフォルト）
                myMagick.RenderingIntent = ImageMagick.RenderingIntent.Perceptual;
                //「黒点の補正を使用」
                myMagick.BlackPointCompensation = true;

                //「ICCプロファイル」が埋め込まれていなければ
                if (myMagick.GetColorProfile() == null)
                {
                    myMagick.SetProfile(ImageMagick.ColorProfile.SRGB);
                }

                switch (option.ColorMode)
                {
                    case "Default":
                        break;

                    case "CMYK":
                        //埋め込まれた「ICCプロファイル」（RGB）から
                        //他の「ICCプロファイル」（CMYK）へ変換
                        myMagick.SetProfile(ImageMagick.ColorProfile.CoatedFOGRA39);
                        //「背景」にする(アルファを白くします)
                        myMagick.ColorAlpha(ImageMagick.MagickColors.White);
                        break;

                    case "sRGB":
                        myMagick.SetProfile(ImageMagick.ColorProfile.SRGB);
                        break;

                    case "Remove":
                        //プロファイルの削除
                        myMagick.RemoveProfile("icc");
                        break;

                    default:
                        myMagick.SetProfile(new ColorProfile(option.ColorMode));
                        break;
                }
            }

            //グレースケール
            if (option.GrayScale)
            {
                if (ColorSpace.CMYK == myMagick.ColorSpace)
                    myMagick.Negate();
                myMagick.Grayscale();
                myMagick.RemoveProfile("icc");
            }


            token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// [0]ファイル名,[1]色空間,[2]横pixel,[3]縦pixel,[4]画像形式,[5]横解像度,[6]縦解像度,[7]解像度の単位,[8]ICCプロファイル.[9]フォーマット
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public string[] GetDetail(string filepath)
        {
            //「画像情報」取得準備
            ImageMagick.MagickImageInfo myMagickInfo = new ImageMagick.MagickImageInfo(filepath);
            //「画像情報」取得
            string[] strs = new string[11];
            strs[0] = (myMagickInfo.FileName);
            strs[1] = (myMagickInfo.ColorSpace.ToString());
            strs[2] = (myMagickInfo.Width.ToString());
            strs[3] = (myMagickInfo.Height.ToString());
            strs[4] = (myMagickInfo.Format.ToString());
            strs[5] = (myMagickInfo.Density.X.ToString());
            strs[6] = (myMagickInfo.Density.Y.ToString());
            strs[7] = (myMagickInfo.Density.Units.ToString());
            strs[8] = (myMagickInfo.Compression.ToString());
            
            //var myMagickSettings = new ImageMagick.MagickReadSettings();

            //jpeg読み込み設定

            //myMagickSettings.SetDefine(ImageMagick.MagickFormat.Jpg, "size", "1x1");
            using (var myMagick = new ImageMagick.MagickImage(filepath))
            {
                if (myMagick.GetColorProfile() != null)
                {
                    if (!String.IsNullOrWhiteSpace(myMagick.GetColorProfile().Description))
                        strs[8] = (myMagick.GetColorProfile().Description);
                    else
                        strs[8] = ("NULL");
                }
                else
                {
                    strs[8] = ("NULL");
                }
                //strs[9] = myMagick.Gamma.ToString();
                //strs[10] = myMagick.Depth.ToString();
            }

            return strs;
        }
    }
}
