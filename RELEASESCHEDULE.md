# Developed but not added to CHANGELOG.md

- AccelerationMovePatternを作成（速度変化を指定）
- ファイル構造の整理
    Assets
    ├── Core
    ├── Games
    │   └── DanmakuGame
    │       ├── Prefabs
    │       ├── ScriptableObjects
    │       │   ├── Bullet
    │       │   ├── Enemy
    │       │   ├── Pattern
    │       │   └── Player
    │       └── Scripts
    │           ├── Entity
    │           │   ├── Bullet
    │           │   ├── Enemy
    │           │   └── Player
    │           ├── GameParameter
    │           │   └── Editor
    │           ├── Pattern
    │           │   ├── Calculate
    │           │   ├── Composite
    │           │   ├── Emission
    │           │   ├── Move
    │           │   └── Shoot
    │           ├── State
    │           └── Utility

- AccelerationMovePatternをフレームに依存しないように修正
- AccelerationMovePatternのロジックを、_durationの間に_accelerationだけ変化するように修正
- CalculatedParameter.cs と CalculatedParameter.cs.meta ファイルを削除
- GameParameterに対して、GameParameterReferenceに対応したScriptableObjectを作成
- GameParameterのScriptableObjectを作成するために、専用のクラスを作成

# Developing

- 「TriggeredPattern」を作成。呼び出されたとき、トリガー条件（時間、オブジェクト衝突、オブジェクトからの距離、ライフタイム終了時、壁衝突時）を満たしたときに処理を実行する
    - これは「条件を満たしたときに処理を実行する」という責務
    - オプションとして、条件を満たしていなかった場合の処理も追加できる
    - If文に相当
    - GameParameterやいくつかの特殊な値（衝突の有無や時間、オブジェクトからの距離）を引数として、条件式をmXparserで評価する

## v0.1～実装予定

- AngleParameterをGameParameterに統合
- シーンをリロードしたときにGameParameterをinitializeの値で初期化するようにする
    - 現在は保持されるようになっている

- MovementStateのVelocityは、複数のものに対応できるようにして、例えば直進しつつsin波で上下に動くなど、複数の移動を組み合わせられるようにする
    - 例えば、直進しつつsin波で上下に動くなど、複数の移動を組み合わせられるようにする

- GameStateParameterを実装する

- 数値型でない型に対してもCalculatePatternを実装できるようにする
    - 例えば、PatternBaseの具象クラスや、Entityなど。
    - パラメータとしてこれを用意しておいて、これをCalculatePatternの引数に渡すことで、CalculatePatternの計算結果をパラメータとして使用できるなど

- privateメンバの命名規則（_をつける）や、[SerializeField]の改行の統一
- RotatingShotPattern、MultiWayPatternでもともと使用していたGameParameter（旧SharedResource）の記述を削除（不必要になったため）

- CalculatedParameterで、次元数ごとに型を1まとめにできるようにする

- PatternBase具象クラスの命名規則の統一
    - ShootPatternBaseの具象クラスは名称にShootPatternをつける
    - MovePatternBaseの具象クラスは名称にMovePatternを付ける

- ShootPatternの具象クラスから、点・線・面の弾を撃つことを補助するためのShotUtilityを作成する
    - ShotUtilityクラスでは、与えられたEmissionDataとshooterによって、初期設定を行い、shootメソッドで撃つ
    - ShotUtilityクラスの責務は、1セットのパターンを点・線・面で撃つかの補助を行うこと
    - ShootPatternは、Shootメソッドを発火させるだけでよく、各EmissionDataに基づいてShotUtilityが発射処理を行う
    - ShotUtilityクラスのコンストラクタに提供するのは、EmissionData、IShootable、IMovableの3つ
    - Shootメソッドに提供するのは、「撃つ玉」と「撃ち方（角度計算方法・回数・頻度）」の2つ
    - これによって各ShootPatternは点・線・面であることを意識せずに撃つことができるようになる

- AnimatePatternを抽象クラスを追加する（ShootPatternは撃つ方法の責務、MovePatternは移動の方法の責務、AnimatePatternは見た目変化の方法の責務）
    - GameEntity変更、大きさ変更、色変更、回転変更等
- EntityControllerにAnimateを司る部分を追加

以下はv0.1ではなく、v0.2以降に実装予定

- LaserShootPatternを作成（レーザー弾）
    - LaserEntity、LaserController、LaserPropertyを作成（レーザーの挙動はMovePatternを使用）
    - 頭、胴体、尾の3つの部分からなり、ひとつ前を追跡する（連結）

- SatelliteMovePatternでフーリエ変換や楕円を指定可能に

- WinderMovePatternを作成（巻きつけ弾）
- ReflectionMovePatternを作成（反射弾）


- ノードベースで弾幕パターンを作成可能に（Graph Editorの作成）
- Patternをノードで作成可能にして、同一の設定項目を表示
    - PatternBase具象クラスと全く同じような設計になるようにする
    - 生成したNodeはScriptableObjectで、既存の具象クラスと同じように入れ子でも使えるようにする（追加する際に、Patternの一覧から選べるようにする）
    - Pattern Graphでは、ルートノード（ParallelPattern）であるStartを初期配置して、そこからつなげる
        - すべてのPatternBase、ShootPatternBase、MovePatternBaseのサブクラスが追加・入れ子可能
- PatternBaseを保管するライブラリを作成

## v0.2～実装予定
- オブジェクトプーリングの実装
- 自機ライフの実装
- 敵機複数弾幕の実装（体力がなくなると次の弾幕に移行）
- ローディングシーン、タイトルシーン（スタート画面、トップ画面）を設定
- トップ画面にGame Start、Quitを設定
- ステージコントローラー（道中とボス）の作成
- パワー、アイテム、ボムを実装
- スコア、グレイズを実装
- 難易度、オプションを実装
- 雑魚敵、中ボスなどを実装
- ゲームクリア、ゲームオーバーを実装
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