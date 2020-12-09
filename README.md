# Play Around with Images 2
### Image conversion software [ English version is editing ]
#### It supports the following functions
- Rotation
- left and right reversal (Mirror)
- Change  Resolution
- Change the image ratio
- Convert to Grayscale
- Copy of the images
- Setting the background color (transparent)
- Gamma correction
- Specify the file size (JPEG)


### 日本語説明 
## 概要
  本ソフトはイラスト制作で必要な様々な形式をサポートし，かつ簡単な操作で画像を変換することを目的としたソフトです．
  また，先代ソフト"Play Around with Images"の後継ソフトウェアとなります．
  
## プログラム構成
  - ImageMagickのc#版であるMagick.NET(Q16HDR版)を中心に構成されています．本来は200種類を超える画像をサポートしておりますが，メジャーなファイル形式のみに制限しています．
  - 非同期並列処理及びOpenCLによる高速処理をサポートしています．
 
***デモ***
 
![デモ](https://raw.githubusercontent.com/falxala/Play-Around-with-Images-2/master/Convert/Resources/SS1.png)
 
## 機能

  #### 変換機能
  - ファイル形式の変換
  - ファイルコピー
  - 回転
  - 反転
  - 解像度の変更
  - 画像比率変更
  - グレースケール化
  - 透過をサポートしている画像フォーマットにおいて背景色の設定
  - ガンマ補正
  - JPEG容量指定（自動品質設定）
  - PNG圧縮品質設定
  - ラスタライズ解像度指定
  - トリミング（試験的機能）

  #### アプリ機能
  - 簡易画像ビュー機能
  - リストビューによるサムネイル表示
  - 正規表現によるファイルフィルター
  - OpenCLサポート（GPU）
  - マルチコアサポートによる複数ファイル同時変換
  - スクリーンショット自動取り込み機能(クリップボードから画像を取得します)

 
## 必要条件 
- __Windows10（Windows8以下では動作確認を行っていません）__
- __64ビットOS（32ビットOSでは動作しません）__
- .NetFramework4.7.2以上（Windows7, 8.1, 10, 10 Creators Update）
- HD解像度(1280×720)以上のディスプレイ

## 推奨要件
- 8GBメモリ
- 4コア以上のCPU
 
## 対応フォーマット
  #### 読み込み
  -  Jpeg, Png, Webp, HEIC, Bmp, Gif, Tiff, Ai, Psd, Ico, Eps, PDF, EXR, HDR, ZIP(自動展開)
  #### 書き出し
  -  Jpeg, Png, Webp, Bmp, Gif, Tiff, Ai, Psd, Ico, Eps, EXR, HDR

## インストール/起動
[リリースページ](https://github.com/falxala/Play-Around-with-Images-2/releases)から最新バージョンのZIPファイルをダウンロードし任意の場所に展開，exeファイルを実行します
どこに配置しても問題ありませんが以下のファイルを同じフォルダ内に置いてください．（ZIPファイルをそのまま展開すると同じ構成になります）
[ファイル構成]
PAWI2
├ gs　[フォルダ]（Ghostscript同梱版のみ）
├ Magick.Native-Q16-HDRI-OpenMP-x64.dll [ファイル]
└ PlayAroundwithImages2.exe [ファイル]

## 使い方
詳しい操作方法については後日更新します
1. リストビュー(灰色の領域)へドロップ，コピー＆ペースト，ダブルクリック，右クリックいずれかの操作でファイルを入力します．
2. オプションを設定します．（初期設定：Jpeg，長辺2048px，5MB以下，Quality75，exeファイルがあるフォルダoutputsに画像を保存）
3. 変換したい画像を選択しCONVERTボタンを押下します．
4. 画像が変換されます．（4枚以下の場合はクリップボードにもデータがコピーされます）
 
## その他
   #### 操作
   - ファイルの複数選択　Multipleトグルを有効にするか，リストビュー上でCtrl+クリック, Shiht+クリック
   - リストビュー下の白いバーを上へスライドさせると変換ログが表示されます
   - 解像度スライドバーを一番左へ動かすとオリジナル解像度を保持します
   - ファイルサイズスライドバーを一番左へ動かすと制限を行いません
   - Convertボタン右にある小さな領域をクリックするとファイルコピーモードになります
   - メイン画面2列目にある白いテキスト入力枠は正規表現フィルタです．例えば「 .png|.jpg 」と入力すると拡張子がpngとjpgのものだけが表示されます．
   - 便利なショートカット機能
     - __リストビュー拡大縮小__ リストビューの上でCtrl＋マウスホイール
     - __画像のペースト入力__　アプリが選択されている状態でCtrl+V
     - __画像を外部アプリで開く__　リストビュー上の画像をダブルクリック
     - __変換したファイルがあるフォルダを開く__　変換ログをダブルクリックor設定ウィンドウ内OutputsDirectoryの文字をダブルクリック
     - __数値の変更__　スライドバーまたは入力ボックスの上でマウスホイールを回転(altキーを押すと1ずつ増減)
     - __プレビュー画像(画面右上) の拡大縮小__　画像上でマウスホイール回転
     - __プレビュー画像(画面右上) の拡大縮小リセット__　画像上でダブルクリック
     - __アプリレイアウトのリセット__　プレビュー画面で右クリック/レイアウトリセット
   #### 仕様
   - プログラムの仕様上多数のファイルを同時に変換すると大量のメモリを消費します．エラーが発生した場合は設定/Advancedタブparallels max(同時変換数)を下げてご使用ください．
   - フォルダの入力はドラッグ＆ドロップまたは，コピー＆ペーストでのみ可能です．
   - 変換時にメモリが不足した場合，1GBを上限にディスクを使用します（超えた場合エラーとなります）．この際パフォーマンスが大きく低下し変換に時間がかかります．
   - フォーマット"Ico"を選択時257px以上を指定するとエラーとなります．256px以内で指定してください．
   - OpenCLにおいてGPUを選択するとプロファイル(パフォーマンス測定)が作成されます．削除したい場合は”OpenAppLocal”ボタンからフォルダを開き削除してください．
   #### 既知の不具合
    多数のファイル(100ファイル以上)を読み込むと本アプリが終了してしまったり，ソートフィルタ機能が動作しなくなる
 
## 作者
 
[@1_421A](https://twitter.com/1_421A)
 
## ライセンス
 
[MIT](https://raw.githubusercontent.com/falxala/Play-Around-with-Images-2/master/LICENSE)</blockquote>
