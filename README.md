# GreatUma

Get low odds horses from JRA information and purchase tickets of them.

# 開発環境

* Windows 10以上が必要
* Visual Studio 2022以上が必要
* ターゲットフレームワーク.NET 8
  * .NET SDK 8以上が必要
    * https://dotnet.microsoft.com/ja-jp/download

# リリース手順

## タグ打ち

* タグを打つ（リリースページ作成と同時でも良い）

## 成果物の作成

* Visual Studioを開く
* GreatUmaプロジェクトを右クリック
* 「発行」を選択
* 以下のプロファイルを設定
  * 構成: Release| Any CPU
  * ターゲットフレームワーク: net8.0-windows
  * 配置モード: 自己完結
    * 配布時に.NETランタイムが不要なように自己完結とする
  * ターゲット ランタイム: win-x64
* 上記のプロファイルでpublishする
* publishした結果をzipに圧縮する

## リリースページを作成する

* 変更差分をまとめ、リリースページを作成する
* タグは「タグ打ち」で作成したタグとする。もしくはこの時点で新規作成する。
* 成果物を添付する
