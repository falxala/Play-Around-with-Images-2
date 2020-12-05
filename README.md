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
  これは多様な形式をサポートした簡易画像変換ソフトです．
  また，先代ソフト"Play Around with Images"の後継ソフトウェアとなります．
 
 
***デモ***
 
![デモ](https://raw.githubusercontent.com/falxala/Play-Around-with-Images-2/master/Convert/Resources/Preview2.jpg)
 
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

 
## 必要要件
 
- Windows10（Windows8以下では動作確認を行っていません）
- 64ビットOS（32ビットOSでは動作しません）
- .NetFramework4.7.2以上（Windows7, 8.1, 10, 10 Creators Update）
- HD解像度(1280×720)以上のディスプレイ

## 推奨条件
- 8GBメモリ
- 4コア以上のCPU
 
## 対応フォーマット
  #### 読み込み
  -  Jpeg, Png, Webp, HEIC, Bmp, Gif, Tiff, Ai, Psd, Ico, Eps, PDF, EXR, HDR
  #### 書き出し
  -  Jpeg, Png, Webp, Bmp, Gif, Tiff, Ai, Psd, Ico, Eps, EXR, HDR

## インストール/起動
[リリースページ](https://github.com/falxala/Play-Around-with-Images-2/releases)から最新バージョンのZIPファイルをダウンロードし任意の場所に展開，exeファイルを実行

## 使い方
詳しい操作方法については後日更新します
1. ドラッグ＆ドロップウィンドウ(灰色の領域)へドロップ，コピー＆ペースト，ダブルクリック，右クリックいずれかの操作でファイルを入力します．
2. オプションを設定します．（初期設定：Jpeg，長辺2048px，5MB以下，exeファイルがあるフォルダ/outputsに画像を保存）
3. サムネイルが表示されるので変換したい画像を選択しCONVERTボタンを押下します．
4. 画像が変換されます．（4枚以下の場合はクリップボードにもデータがコピーされます）
 
## その他
   #### 操作
   - ドラッグ＆ドロップウィンドウ下の白いバーを上へスライドさせると変換ログが表示されます
   - 解像度スライドバーを一番左へ動かすとオリジナル解像度を保持します
   - ファイルサイズスライドバーを一番左へ動かすとファイルサイズを制限しません
   #### 既知の不具合
    多数のファイル(100ファイル以上)を読み込むと本アプリが終了してしまったり，ソートフィルタ機能が動作しなくなる
 
## 作者
 
[@1_421A](https://twitter.com/1_421A)
 
## ライセンス
 
[MIT](https://raw.githubusercontent.com/falxala/Play-Around-with-Images-2/master/LICENSE)</blockquote>
