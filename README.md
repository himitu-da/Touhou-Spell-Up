Developing

- 移動システムの共通化
    - IMovable Interface、MovePatternBase、Mover Componentの作成
    - 敵機や敵弾に関する動的で複雑な「移動」ふるまいを実現する（敵機、敵機弾の双方）
    - 射撃システムとの連携（射撃が一巡してから移動等）
- 射撃システムの共通化
    - IShootable Interface、Shooter Componentの作成、ShootPatternBaseの修正
    - ShootPatternBaseにShooterを追加
    - 敵機以外から現れる弾幕を実現する（ShootPatternBaseに発射ポイントを設定：絶対位置、敵の相対位置、自機の相対位置、壁面、発射口、弾自身など）
- BasicShotPattern、MultiWayPattern、ScatteringPatternに対して、向きのオフセットを指定できるように
    - 敵機完全固定弾を作れるように
- ワインダーパターンを作成
- へにょり弾を打てるように
- 時間発狂やHP発狂を作れるように
- ChainedShotPatternを実装し、弾が次の弾や弾幕を打てるように
    - 最初に撃つ弾
    - トリガー条件（時間、オブジェクト衝突、オブジェクトからの距離、ライフタイム終了時
- LoopPatternでループ回数の指定ができるように
- ShootPatternやそれを継承したクラスにおけるBulletの表示をShotに変更

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