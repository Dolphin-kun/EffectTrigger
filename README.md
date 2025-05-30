# 「エフェクトトリガー」プラグイン

### [最新バージョンをダウンロード](https://github.com/Dolphin-kun/EffectTrigger/releases/latest)
## 概要
指定した条件を満たすときにエフェクトを実行させる映像エフェクトプラグインです。  
エフェクトの入れ子部分に簡易カーニングのコードを参考にさせていただきました。

エフェクトを適用しているアイテムが、条件を満たした場合にエフェクトを適用することができます。

## 使い方
「描画」から「エフェクトトリガー」を追加してください。

## 効果
### 条件
ifの部分です。
「Xが0pxに等しいとき」のように表示がされるので、それを見ながら調整してください。

### 対象
現在指定できる対象は以下の通りです。

|対象||
|-|-|
|X|描画位置（横方向）|
|Y|描画位置（縦方向）|
|Z|描画位置（奥行き）|
|不透明度|不透明度|
|拡大率X|拡大率（横方向）|
|拡大率Y|拡大率（横方向）|
|回転角X|縦方向、X軸に対する回転角|
|回転角Y|横方向、Y軸に対する回転角|
|回転角Z|平面方向、Z軸に対する回転角|


### リセット
条件を満たすたびにエフェクトが適用されます。  
登場退場も使用可能です。

## 注
全てのエフェクトが問題なく動作するかどうかは未検証です。  
もしエラー・バグ等発生しましたらDMやDiscordまでご連絡ください。


製作者がC#初心者のためよくわからんコードで書いているかもしれません。
参考にはならないと思います。🙇‍♂️


## 更新履歴
2025/5/25 v1.0 公開
