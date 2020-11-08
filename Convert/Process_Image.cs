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
        }

        /// <summary>
        /// 与えられたファイルを変換します
        /// </summary>
        /// <param name="item"ファイルパス></param>
        /// <returns>出力ファイル名</returns>
        public async Task<String> Convert(Model.drop_Image item, ConvertOptions option, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (option.Passthrough == true)
            {
                FileInfo file = new FileInfo(item.Image_path);
                string destinationPath = option.SaveDirectory + "\\" + System.IO.Path.GetFileName(item.Image_path);
                Console.WriteLine(destinationPath);
                file.CopyTo(destinationPath);
                return (destinationPath);
            }

            var Result = await Task.Run(() =>
            {
            var myMagickSettings = new ImageMagick.Formats.Jpeg.JpegWriteDefines();
                myMagickSettings.Extent = (int)(option.Filesize*1024);
            string outputPath = "";
            string outputDir = option.SaveDirectory + "\\";
            Directory.CreateDirectory(outputDir);

            string output_Num = "Cnv_";
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

            outputPath = outputDir + output_Num + output_name + "." + f;

                int Count = 2;
                if (!option.Overwrite)
                    while (true)
                    {
                        if (!option.Overwrite && File.Exists(outputPath))
                        {
                            output_Num = "Cnv_" + Count + "_";
                            outputPath = outputDir + output_Num + output_name + "." + f;
                            Count++;
                        }
                        else
                        {
                            break;
                        }
                    }

                using (var myMagicks = new ImageMagick.MagickImageCollection(item.Image_path, myMagicReadkSettings))
                {
                    for (int i = 0; i < myMagicks.Count; i++)
                    {

                        ImageMagick.MagickImage myMagick = (ImageMagick.MagickImage)myMagicks[i];

                        if ((long)option.Crop[2] * (long)option.Crop[3] > 0)
                        {
                            MagickGeometry geometry = new MagickGeometry(option.Crop[0], option.Crop[1], option.Crop[2], option.Crop[3]);
                            myMagick.Crop(geometry, ImageMagick.Gravity.Center);
                        }
                        myMagick.Format = option.Format;

                        //quality = 0は75になる
                        myMagick.Quality = ++option.Quality;

                        myMagick.BackgroundColor = option.BackgroundColor;

                        if (myMagick.Format == ImageMagick.MagickFormat.Jpeg)
                        {
                            if (option.BackgroundColor == new ImageMagick.MagickColor("transparent"))
                                myMagick.BackgroundColor = new ImageMagick.MagickColor("white");
                            myMagick.Alpha(ImageMagick.AlphaOption.Remove);
                        }

                        if (option.BackgroundColor != new ImageMagick.MagickColor("transparent"))
                            myMagick.Alpha(ImageMagick.AlphaOption.Remove);

                        //グレースケール
                        if (option.GrayScale)
                            myMagick.Grayscale();

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
                        else if (option.Size.Width == option.Size.Height && option.Size.Width != 0)
                        {
                            if (!option.NotScaleUp)
                            {
                                myMagick.Resize(option.Size.Width, option.Size.Height);
                            }
                            else if (option.Size.Width * option.Size.Height < myMagick.Width * myMagick.Height)
                                myMagick.Resize(option.Size.Width, option.Size.Height);
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

                        token.ThrowIfCancellationRequested();

                        //「ファイル」へ書き出し  
                        if (i == 0)
                            myMagick.Write(outputPath, myMagickSettings);
                        else
                            myMagick.Write(outputDir + output_Num + output_name + "_" + i + "." + f, myMagickSettings);

                        token.ThrowIfCancellationRequested();
                    }

                    return outputPath;
                }
            });
            return Result;
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
