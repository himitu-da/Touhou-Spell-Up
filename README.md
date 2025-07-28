Developing

---

- 最終リファクタリング
    - Enemy, Bullet, PlayerをGameEntity基本単位として表現
    - EnemyPropertyとBulletPropertyをEntityPropertyとしてまとめ、Playerもこれで管理できるように
    - ActorをActionControllerに統合し、GameEntiry全体の行動制御を担うように。PlayerController, EnemyControllerはサブクラスに
    - ActionControllerにAnimator機能を追加する

- 弾幕に対してトリガー条件（時間、オブジェクト衝突、オブジェクトからの距離、ライフタイム終了時）を追加できるように

- 射撃位置にMoverが適用できるようにするかを検討
- ワインダーパターンを作成

- SatelliteMovePatternでフーリエ変換や楕円を指定可能に

- 時間発狂やHP発狂を作れるように
- ShootPatternやそれを継承したクラスにおけるBulletの表示をShotに変更
- BulletとEnemyをShootable、Movable Interfaceにまとめられるように
- オブジェクトプーリングの実装
- SharedResourceを使用し、すべてのint型もしくはfloat型のSerializeFieldを置換する
- SharedResourceに発射ごとに1ずつ変わる、特定の関数を動く、のような処理を追加する
    - これによって更に柔軟な出現位置を実現できる

- PatternBaseを保管するライブラリを作成
- Override Bulletの対象をEntityに変更
    - Override Bulletに弾幕パターンを入れられるように（柔軟性）

後回し：

- ノードベースで弾幕パターンを作成可能に（Graph Editorの作成）
- Patternをノードで作成可能にして、同一の設定項目を表示
    - PatternBase具象クラスと全く同じような設計になるようにする
    - 生成したNodeはScriptableObjectで、既存の具象クラスと同じように入れ子でも使えるようにする（追加する際に、Patternの一覧から選べるようにする）
    - Pattern Graphでは、ルートノード（ParallelPattern）であるStartを初期配置して、そこからつなげる
        - すべてのPatternBase、ShootPatternBase、MovePatternBaseのサブクラスが追加・入れ子可能

Bullet・BulletProperty・
Bullet、BulletProperty、
    - Bullet Graphでは、「Prefab」と「BulletProperty」を指定するための2つのノードを初期設定して、そこからつなげる
    - BulletProperty Graphでは、Patternを指定するためのルートノード（ParallelPattern）を初期設定して、そこからつなげる

- ShootPatternの具象クラスから、点・線・面の弾を撃つことを補助するためのShotUtilityを作成する
    - ShotUtilityクラスでは、与えられたEmissionDataとshooterによって、初期設定を行い、shootメソッドで撃つ
    - ShotUtilityクラスの責務は、1セットのパターンを点・線・面で撃つかの補助を行うこと
    - ShootPatternは、Shootメソッドを発火させるだけでよく、各EmissionDataに基づいてShotUtilityが発射処理を行う
    - ShotUtilityクラスのコンストラクタに提供するのは、EmissionData、IShootable、IMovableの3つ
    - Shootメソッドに提供するのは、「撃つ玉」と「撃ち方（角度計算方法・回数・頻度）」の2つ
    - これによって各ShootPatternは点・線・面であることを意識せずに撃つことができるようになる

# v0.2～実装予定
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

# v0.3～実装予定
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