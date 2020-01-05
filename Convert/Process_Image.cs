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

namespace PlayAroundwithImages2
{
    public class Process_Image
    {
        public BitmapSource Create_thumbnail(string imagePath)
        {
            Bitmap myBitmap;
            var myMagickSettings = new ImageMagick.MagickReadSettings();

            //PDF、EPS、AIのラスタライズ時の設定
            //ラスタライズ時ICCプロファイル
            myMagickSettings.ColorSpace = ImageMagick.ColorSpace.sRGB;
            //ラスタライズ時解像度
            myMagickSettings.Density = new ImageMagick.Density(96);
            //ラスタライズ時カラータイプ
            myMagickSettings.ColorType = ImageMagick.ColorType.TrueColor;

            //必ずソースとなる「Jpeg画像」より、縮小予定のPixel数がが半分以下である事
            myMagickSettings.SetDefine(ImageMagick.MagickFormat.Jpg, "size", "250x250");
            using (var myMagick = new ImageMagick.MagickImage(imagePath,myMagickSettings))
            {
                myMagick.Alpha(ImageMagick.AlphaOption.Remove);
                myMagick.Strip();
                myMagick.Thumbnail(250, 250);
                myBitmap = myMagick.ToBitmap();
            }

            using (var ms = new System.IO.MemoryStream())
            {
                // MemoryStreamに書き出す
                myBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                // MemoryStreamをシーク
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                // MemoryStreamからBitmapFrameを作成
                System.Windows.Media.Imaging.BitmapSource bitmapSource =
                    System.Windows.Media.Imaging.BitmapFrame.Create(
                        ms,
                        System.Windows.Media.Imaging.BitmapCreateOptions.None,
                        System.Windows.Media.Imaging.BitmapCacheOption.OnLoad
                    );
                return bitmapSource;
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

            /// <summary>
            /// JpegOption Quality[0-100]
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

        }

        /// <summary>
        /// 与えられたファイル変換します
        /// </summary>
        /// <param name="filepath"ファイルパス></param>
        /// <param name="filesize">ファイルサイズ[width,height]</param>
        /// <param name="longside">長辺サイズ</param>
        /// <param name="overwrite">ファイル上書き</param>
        /// <returns>出力ファイル名</returns>
        public async Task<String> Convert(string filepath,ConvertOptions option)
        {
            var Result = await Task.Run(() =>
            {
                var myMagickSettings = new ImageMagick.JpegWriteDefines();
                myMagickSettings.Extent = (int)(option.Filesize * 1024);

                string outputPath = "";
                string outputDir = option.SaveDirectory + "\\";
                Directory.CreateDirectory(outputDir);

                string output_Num = "Cnv_";
                string output_name = System.IO.Path.GetFileNameWithoutExtension(filepath);

                //PDF、EPS、AIのラスタライズ時の設定
                var myMagicReadkSettings = new ImageMagick.MagickReadSettings();
                //ラスタライズ時ICCプロファイル
                myMagicReadkSettings.ColorSpace = ImageMagick.ColorSpace.sRGB;
                //ラスタライズ時解像度
                myMagicReadkSettings.Density = new ImageMagick.Density(350);
                //ラスタライズ時カラータイプ
                myMagicReadkSettings.ColorType = ImageMagick.ColorType.TrueColor;

                using (var myMagick = new ImageMagick.MagickImage(filepath,myMagicReadkSettings))
                {
                    
                    if(myMagick.Format == ImageMagick.MagickFormat.Jpeg)
                    {
                        myMagick.Alpha(ImageMagick.AlphaOption.Remove);
                    }
                    else
                    {
                        myMagick.Alpha(ImageMagick.AlphaOption.Activate);
                    }

                    //リサイズ・変形
                    if (!option.Transform)
                    {
                        myMagick.Resize(option.Size.Width, option.Size.Height);
                    }
                    else
                    {
                        //後で実装
                        //var WidthPer = new ImageMagick.Percentage((double)option.size.Width / myMagick.Width * 100);
                        //var HeightPer = new ImageMagick.Percentage((double)option.size.Height / myMagick.Height * 100);
                        //myMagick.LiquidRescale(WidthPer, HeightPer);

                        var param = new double[]
                        {
                            0,0,                                //入力_左上座標
                            0,0,                                //出力_左上座標
                            0,myMagick.Height,                  //入力_左下座標
                            0,option.Size.Height,               //出力_左下座標
                            myMagick.Width,myMagick.Height,     //入力_右下座標
                            option.Size.Width,option.Size.Height,               //出力_右下座標
                            myMagick.Width,0,                   //入力_右上座標
                            option.Size.Width,0                         //出力_右上座標
                        };
                        //自由変形
                        //指定したサイズにフィットするようにカンバスサイズを変形する設定
                        ImageMagick.DistortSettings distortSettings = new ImageMagick.DistortSettings();
                        distortSettings.Bestfit = true;
                        distortSettings.Viewport = new ImageMagick.MagickGeometry(option.Size.Width,option.Size.Height);
                        myMagick.Distort(ImageMagick.DistortMethod.BilinearForward, distortSettings, param);
                        myMagick.Crop(option.Size.Width, option.Size.Height);
                        myMagick.RePage();
                    }

                    myMagick.Format = option.Format;
                    //myMagick.Quality = 10;

                    outputPath = outputDir + output_Num + output_name + "." + myMagick.Format.ToString();

                    //「ファイル」へ書き出し  
                    int Count = 2;
                    if (!option.Overwrite)
                        while (true)
                        {
                            if (!option.Overwrite && File.Exists(outputPath))
                            {
                                output_Num = "Cnv_" + Count + "_";
                                outputPath = outputDir + output_Num + output_name + "." + myMagick.Format.ToString();
                                Count++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    myMagick.Write(outputPath, myMagickSettings);

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
            string[] strs = new string[9];
            strs[0] = (myMagickInfo.FileName);
            strs[1] = (myMagickInfo.ColorSpace.ToString());
            strs[2] = (myMagickInfo.Width.ToString());
            strs[3] = (myMagickInfo.Height.ToString());
            strs[4] = (myMagickInfo.Format.ToString());
            strs[5] = (myMagickInfo.Density.X.ToString());
            strs[6] = (myMagickInfo.Density.Y.ToString());
            strs[7] = (myMagickInfo.Density.Units.ToString());
            strs[8] = (myMagickInfo.Compression.ToString());

            var myMagickSettings = new ImageMagick.MagickReadSettings();

            //jpeg読み込み設定
            myMagickSettings.SetDefine(ImageMagick.MagickFormat.Jpg, "size", "1x1");
            using (var myMagick = new ImageMagick.MagickImage(filepath,myMagickSettings))
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
            }
            return strs;
        }
    }
}
