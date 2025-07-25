Developing

- ノードベースで弾幕パターンを作成可能に

- 弾幕に対してトリガー条件（時間、オブジェクト衝突、オブジェクトからの距離、ライフタイム終了時）を追加できるように

- 射撃位置にMoverが適用できるようにするかを検討
- ワインダーパターンを作成

- SatelliteMovePatternでフーリエ変換や楕円を指定可能に

- 時間発狂やHP発狂を作れるように
- ShootPatternやそれを継承したクラスにおけるBulletの表示をShotに変更
- IMovableとIShootableの作成
- BulletとEnemyをShootable、Movable Interfaceにまとめられるように
- 壁面、発射口、弾自身などから現れる弾幕を実現できるように
- オブジェクトプーリングの実装
- SharedResourceを使用し、すべてのint型もしくはfloat型のSerializeFieldを置換する
- SharedResourceに発射ごとに1ずつ変わる、特定の関数を動く、のような処理を追加する
    - これによって更に柔軟な出現位置を実現できる

- 最終リファクタリング
    - Enemy, Bullet, PlayerをGameEntity基本単位として表現（PatternBaseをこれに含めるかの是非を決める）
    - EnemyPropertyとBulletPropertyをEntityPropertyとしてまとめ、Playerもこれで管理できるように
    - MoverとShooterをActionControllerに統合し、GameEntiry全体の行動制御を担うように。PlayerController, EnemyControllerは不要に
    - ActionControllerにAnimator機能を追加する

- PatternBaseを保管するライブラリを作成
- Override Bulletの対象をEntityに変更
    - Override Bulletに弾幕パターンを入れられるように（柔軟性）

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