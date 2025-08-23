# Developed but not added to CHANGELOG.md


# Developing

## v0.1～実装予定

- PatternBaseにおいて、親のGameEntityを参照できるようにする。
    - SatellitePatternで特殊な実装をしていたものを、平準化する

- ShootPatternの具象クラスから、点・線・面の弾を撃つことを補助するためのShotUtilityを作成する
    - ShotUtilityクラスでは、与えられたEmissionDataとshooterによって、初期設定を行い、shootメソッドで撃つ
    - ShotUtilityクラスの責務は、1セットのパターンを点・線・面で撃つかの補助を行うこと
    - ShootPatternは、Shootメソッドを発火させるだけでよく、各EmissionDataに基づいてShotUtilityが発射処理を行う
    - ShotUtilityクラスのコンストラクタに提供するのは、EmissionData、IShootable、IMovableの3つ
    - Shootメソッドに提供するのは、「撃つ玉」と「撃ち方（角度計算方法・回数・頻度）」の2つ
    - これによって各ShootPatternは点・線・面であることを意識せずに撃つことができるようになる

- オブジェクトプーリングの実装

- GameEntityStateとGameEntityPropertyについて、それぞれ動的な状態と静的な状態を保持するような責務に分ける
- GameEntityControllerはGameEntityStateとGameEntityPropertyを持つようにし、これ自体が状態を保有しないようにする
- GameEntityStateを継承したPlayerState、EnemyState、BulletStateを作成
- BulletStateが残りの寿命を保持するようにし、BulletControllerからcurrentLifeTimeを削除する
- GameEntityStateのScaleはScaleMultiplierに名称変更し、初期スケールに対する倍率として機能するようにする
- EnemyPropertyにmaxHealthを追加し、EnemyControllerからmaxHealthを削除する
- initialScaleはGameEntityStateが保有するように変更
- GameEntityControllerに対して、Stateの値を取得するためのフィールドを用意する
    - これはGameParameterとして共通化できるようにする

以下はv0.1ではなく、v0.2以降に実装予定

- Assets/Games以下の全コードのprivateフィールドにおいて、命名規則（_をつける）の統一
- Assets/Games以下の全コードのフィールドにおいて、[SerializeField]後の改行はしないことで統一
- Assets/Games以下の全コードのフィールドにおいて、publicフィールドは使用せず、[SerializeField]もしくはプロパティを使用することで統一
- Assets/Games以下の全コードにおいて、`if`, `for`, `while` などの制御構文では、処理が1行であっても必ず波括弧を使用することで統一

- Reference型のメンバで、シークバーで指定できるようにする（Rangeが指定されている場合）

- PatternBase具象クラスの命名規則の統一
    - ShootPatternBaseの具象クラスは名称にShootPatternをつける
    - MovePatternBaseの具象クラスは名称にMovePatternを付ける

- CalculatedParameterで、次元数ごとに型を1まとめにできるようにする

- MovementStateのVelocityは、複数のものに対応できるようにして、例えば直進しつつsin波で上下に動くなど、複数の移動を組み合わせられるようにする
    - 例えば、直進しつつsin波で上下に動くなど、複数の移動を組み合わせられるようにする

- LaserShootPatternを作成（レーザー弾）
    - LaserEntity、LaserController、LaserPropertyを作成（レーザーの挙動はMovePatternを使用）
    - 頭、胴体、尾の3つの部分からなり、ひとつ前を追跡する（連結）

- SatelliteMovePatternでフーリエ変換や楕円を指定可能に

- WinderMovePatternを作成（巻きつけ弾）
- ReflectionMovePatternを作成（反射弾）
- ループ弾幕（直線または曲線を指定し、その間を移動する弾幕）

- ノードベースで弾幕パターンを作成可能に（Graph Editorの作成）
- Patternをノードで作成可能にして、同一の設定項目を表示
    - PatternBase具象クラスと全く同じような設計になるようにする
    - 生成したNodeはScriptableObjectで、既存の具象クラスと同じように入れ子でも使えるようにする（追加する際に、Patternの一覧から選べるようにする）
    - Pattern Graphでは、ルートノード（ParallelPattern）であるStartを初期配置して、そこからつなげる
        - すべてのPatternBase、ShootPatternBase、MovePatternBaseのサブクラスが追加・入れ子可能
- PatternBaseを保管するライブラリを作成

## v0.2～実装予定
- ゲーム進行管理系
    - ゲームモード管理システムの作成
    - ステージ進行システムの作成
    - ローディングシーン、タイトルシーン（スタート画面、トップ画面）を設定
    - トップ画面にGame Start、Quitを設定
    - 難易度、オプションを実装
    - ステージコントローラー（道中とボス）の作成
    - ゲームクリア、ゲームオーバーを実装
- プレイヤーシステム系
    - 自機ライフの実装
    - 被弾処理
    - ボムシステム、パワーシステムの実装
- スコアと収集
    - スコア、グレイズの実装
    - アイテムの収集システムの実装
- 敵システム系
    - 敵機複数弾幕の実装（体力がなくなると次の弾幕に移行）
    - 雑魚敵、中ボスなどを実装
- 特殊ルールシステムの試作
- 問題データ管理のシステムを作成
- 弾幕と音楽の同期システムを作成

## v0.3～実装予定
- ゲームのデザインコンセプトの検討
- セリフ、キャラクター台詞を実装
- Optionを実装し、キーコンフィグや音量調節ができるように
- 背景演出機能を実装
- 効果音周りの機能

# 全体の流れ
1. ゲーム全体のプロトタイプの作成と学習コンテンツの作成
    1. 弾幕ゲームのプロトタイプの作成
    1. ミニゲームのプロトタイプの作成
    1. ショートストーリーのプロトタイプの作成
    1. エクササイズ、ボキャビル、月まで届け英語学習のプロトタイプの作成
1. デザインの作成
1. マニュアルの作成

# ゲーム項目の一覧
## 弾幕ゲーム系
Game Start
Extra Start
Endless Start
## ミニゲーム系
Mini-Game Start
Short Story
## 演習系
Exercise
Vocabulary
## 達成項目系
The Load to the Moon
Achievements
## その他
Option
Manual
Quit